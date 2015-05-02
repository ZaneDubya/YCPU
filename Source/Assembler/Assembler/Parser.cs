/*
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace YCPU.Assembler
{
    public partial class Parser
    {
        protected Dictionary<string, Func<string[], int, ushort[]>> m_OpcodeAssemblers;
        protected Dictionary<ushort, string> m_BranchReferences;
        protected Dictionary<ushort, string> m_LabelReferences;
        protected Dictionary<ushort, string> m_LabelDataFieldReferences;
        protected Dictionary<string, ushort> m_RegisterDictionary;

        protected List<byte> m_MachineCodeOutput;

        protected bool m_IsNextLineData = false;

        protected Scopes m_Scopes;
        protected string m_Directory;

        public Parser()
        {
            m_OpcodeAssemblers = new Dictionary<string, Func<string[], int, ushort[]>>();
            m_BranchReferences = new Dictionary<ushort, string>();
            m_LabelReferences = new Dictionary<ushort, string>();
            m_LabelDataFieldReferences = new Dictionary<ushort, string>();
            m_RegisterDictionary = new Dictionary<string, ushort>();
            

            m_MachineCodeOutput = new List<byte>();
            m_Scopes = new Scopes();

            InitOpcodeDictionary();
            InitRegisterDictionary();
        }

        public byte[] Parse(string[] lines, string working_directory)
        {
            int linesParsed = 0;

            m_MachineCodeOutput.Clear();
            m_BranchReferences.Clear();
            m_LabelReferences.Clear();

            m_Directory = working_directory;

            try
            {
                foreach (var line in lines)
                {
                    linesParsed++;

                    string currentLine = line.Trim();
                    if (currentLine.Length < 1 || line[0] == ';')
                        continue;

                    currentLine = RemoveLineComments(currentLine);
                    if (currentLine.Trim().Length == 0)
                        continue;

                    AssembleLine(currentLine);
                }

                if (m_Scopes.OpenScopes())
                    throw new Exception("Unclosed scope.");

                SetLabelAddressReferences();
                SetBranchAddressReferences();
                SetDataFieldLabelAddressReferences();

                var count = 1;

                foreach (ushort code in m_MachineCodeOutput)
                {
                    AddMessage(string.Format("{0:X4} ", code));
                    count++;
                }

                return m_MachineCodeOutput.ToArray();
            }
            catch (Exception ex)
            {
                AddMessageLine(string.Format("Line {0}: {1}", linesParsed, ex.Message));
                return null;
            }
        }

        protected string RemoveLineComments(string line)
        {
            string clearedLine = line;
            int commentIndex = line.IndexOf(";");

            if (commentIndex == 0)
                return string.Empty;
            else if (commentIndex > 0)
                clearedLine = line.Substring(0, commentIndex).Trim();

            return clearedLine;
        }

        protected void AssembleLine(string line)
        {
            line = line.Trim();

            if (m_IsNextLineData != false)
            {
                m_IsNextLineData = this.ParseData16(line);
                return;
            }

            if (RegEx.MatchLabel(line) || RegEx.MatchLabelLocal(line))
            {
                bool local = RegEx.MatchLabelLocal(line);
                // parse label and determine if there is anything else to parse on this line.
                int remaiderLineContentIndex = ParseLabel(line, local);
                if (remaiderLineContentIndex <= 0)
                    return;
                // if there is something left to parse, trim it and then interpret it as its own line.
                line = line.Remove(0, remaiderLineContentIndex).Trim();
                if (line.Length < 1)
                    return;
            }

            string[] tokens = this.Tokenize(line);
            string opcode = tokens[0].Trim();
            int bit_width = 16; // default to operating on 16 bits

            // MATCH: xxx[.]xxx
            if (opcode.Contains('.') && (opcode.IndexOf('.') > 1) && (opcode.Length - opcode.IndexOf('.') - 1 > 0))
            {
                // opcode has a flag
                string flag = opcode.Substring(opcode.IndexOf('.') + 1);
                if (!ParseBitWidth(flag, out bit_width))
                    throw new Exception(string.Format("Unknown bit width flag '{0}' for instruction '{1}'.\nAcceptable bit widths are 8, 16, and 32.", flag, line));
                opcode = opcode.Substring(0, opcode.IndexOf('.'));
            }

            if (ParsePragma(line, opcode, tokens))
            {
                // Successfully parsed a pragma, no need to continue with this line.
                return;
            }

            // get the assembler for this opcode. If no assembler exists, throw error.
            Func<string[], int, ushort[]> assembler;
            if (!this.m_OpcodeAssemblers.TryGetValue(opcode.ToLower(), out assembler))
            {
                throw new Exception(string.Format("Undefined cpu opcode in line {0}", line));
            }

            // get the parameters
            List<string> param = new List<string>();
            for (int i = 1; i < tokens.Length; i++)
                param.Add(tokens[i].Trim());

            // pass the params to the opcode's assembler. If no output, throw error.
            ushort[] code = assembler(param.ToArray(), bit_width);
            if (code == null)
                throw new Exception(string.Format("Error assembling line {0}", line));

            // add the output of the assembler to the machine code output. 
            for (int i = 0; i < code.Length; i++)
            {
                ushort this_opcode = code[i];
                m_MachineCodeOutput.Add((byte)(this_opcode & 0x00ff));
                m_MachineCodeOutput.Add((byte)((this_opcode & 0xff00) >> 8));
            }
        }

        protected virtual bool ParseBitWidth(string value, out int bit_width)
        {
            bit_width = 0;
            if (!int.TryParse(value, out bit_width))
                return false;
            if (bit_width != 8 && bit_width != 16 && bit_width != 32)
                return false;
            return true;
        }

        protected string[] Tokenize(string data)
        {
            string[] tokens = data.Split(new[] { ' ', '\t', ',' });
            for (int i = 0; i < tokens.Length - 1; i++)
                if (((tokens[i].Length > 0) && (tokens[i + 1].Length > 0)) && (tokens[i][0] == '[') && (tokens[i + 1][tokens[i + 1].Length - 1] == ']'))
                {
                    tokens[i] = tokens[i] + ',' + tokens[i + 1];
                    tokens[i + 1] = string.Empty;
                }
            return tokens.Where(t => t.Trim() != string.Empty).ToArray();
        }

        protected bool ParseData8(string line)
        {
            List<string> dataFields = new List<string>();

            string lineData = m_IsNextLineData != true ? line.Substring(6, line.Length - 6).Trim() : line.Trim();
            foreach (var field in lineData.Split(','))
            {
                if (field.Trim() == string.Empty) continue;
                if (dataFields.Count == 0)
                {
                    dataFields.Add(field);
                }
                else
                {
                    int count = 0;
                    int last = -1;
                    string lastStr = dataFields[dataFields.Count - 1];

                    while ((last = lastStr.IndexOf('\"', last + 1)) != -1)
                    {
                        count++;
                    }

                    if (count == 1)
                    {
                        dataFields[dataFields.Count - 1] += "," + field;
                    }
                    else
                    {
                        dataFields.Add(field);
                    }
                }
            }

            GenerateInstructionsForDataFields(dataFields, DataFieldTypes.Int8);

            return line.EndsWith(",") ? true : false;
        }

        protected bool ParseData16(string line)
        {
            List<string> dataFields = new List<string>();

            string lineData = m_IsNextLineData != true ? line.Substring(6, line.Length - 6).Trim() : line.Trim();
            foreach (var field in lineData.Split(','))
            {
                if (field.Trim() == string.Empty) continue;
                if (dataFields.Count == 0)
                {
                    dataFields.Add(field);
                }
                else
                {
                    int count = 0;
                    int last = -1;
                    string lastStr = dataFields[dataFields.Count - 1];

                    while ((last = lastStr.IndexOf('\"', last + 1)) != -1)
                    {
                        count++;
                    }

                    if (count == 1)
                    {
                        dataFields[dataFields.Count - 1] += "," + field;
                    }
                    else
                    {
                        dataFields.Add(field);
                    }
                }
            }

            GenerateInstructionsForDataFields(dataFields, DataFieldTypes.Int16);

            return line.EndsWith(",") ? true : false;
        }

        private void GenerateInstructionsForDataFields(IList<string> dataFields, DataFieldTypes dataType)
        {
            foreach (string data in dataFields)
            {
                string valStr = data.Trim();
                if (valStr.IndexOf('"') > -1)
                {
                    string asciiLine = data.Replace("\"", string.Empty);
                    foreach (char c in asciiLine)
                    {
                        switch (dataType)
                        {
                            case DataFieldTypes.Int8:
                                m_MachineCodeOutput.Add((byte)c);
                                break;
                            case DataFieldTypes.Int16:
                                m_MachineCodeOutput.Add((byte)((ushort)c & 0x00ff));
                                m_MachineCodeOutput.Add((byte)((ushort)(c & 0xff00) >> 8));
                                break;
                            case DataFieldTypes.Int32:
                                m_MachineCodeOutput.Add((byte)((ushort)c & 0x00ff));
                                m_MachineCodeOutput.Add((byte)((ushort)(c & 0xff00) >> 8));
                                m_MachineCodeOutput.Add(0x00);
                                m_MachineCodeOutput.Add(0x00);
                                break;
                        }
                    }
                }
                else
                {
                    uint val = (uint)0;

                    if (valStr.Contains("0x") != false)
                    {
                        val = Convert.ToUInt32(valStr, 16);
                    }
                    else if (valStr.All(x => char.IsDigit(x)))
                    {
                        val = Convert.ToUInt32(valStr, 10);
                    }
                    else
                    {
                        m_LabelDataFieldReferences.Add((ushort)m_MachineCodeOutput.Count, valStr);
                    }

                    switch (dataType)
                    {
                        case DataFieldTypes.Int8:
                            if ((val > byte.MaxValue) || (val < byte.MinValue))
                                throw new Exception(string.Format("Included byte value '{0}' cannot be expressed in an 8-bit value.", data));
                            m_MachineCodeOutput.Add((byte)val);
                            break;
                        case DataFieldTypes.Int16:
                            if ((val > ushort.MaxValue) || (val < ushort.MinValue))
                                throw new Exception(string.Format("Included ushort value '{0}' cannot be expressed in a 16-bit value.", data));
                            m_MachineCodeOutput.Add((byte)((ushort)val & 0x00ff));
                            m_MachineCodeOutput.Add((byte)((ushort)(val & 0xff00) >> 8));
                            break;
                        case DataFieldTypes.Int32:
                            if ((val > uint.MaxValue) || (val < uint.MinValue))
                                throw new Exception(string.Format("Included uint value '{0}' cannot be expressed in a 32-bit value.", data));
                            m_MachineCodeOutput.Add((byte)((uint)val & 0x00ff));
                            m_MachineCodeOutput.Add((byte)((uint)(val & 0x0000ff00) >> 8));
                            m_MachineCodeOutput.Add((byte)((uint)(val & 0x00ff0000) >> 16));
                            m_MachineCodeOutput.Add((byte)((uint)(val & 0xff000000) >> 24));
                            break;
                    }
                }
            }
        }

        public enum DataFieldTypes
        {
            Int8,
            Int16,
            Int32
        }

        protected void SetBranchAddressReferences()
        {
            foreach (ushort index in m_BranchReferences.Keys)
            {
                string labelName = m_BranchReferences[index];

                if (!m_Scopes.ContainsLabel(labelName, index))
                {
                    throw new Exception(string.Format("Unknown label reference '{0}'.", labelName));
                }

                ushort label_address = (ushort)m_Scopes.LabelAddress(labelName, index);
                int delta = label_address - index;
                if ((delta > sbyte.MaxValue) || (delta < sbyte.MinValue))
                    throw new Exception("Branch operation out of range.");
                m_MachineCodeOutput[index] = m_MachineCodeOutput[index]; // same operand; this line may not be necessary.
                m_MachineCodeOutput[index + 1] = (byte)((sbyte)delta);
            }
        }

        protected bool IncludeBinary(string[] tokens)
        {
            if (tokens.Length == 1)
                throw new Exception(string.Format("No file specified for .incbin pragma.", tokens[1]));

            tokens[1] = tokens[1].Replace("\"", string.Empty);

            byte[] data = YCPU.Platform.Common.GetBytesFromFile(m_Directory + @"\" + tokens[1]);
            if (data == null)
                throw new Exception(string.Format("Error loading file '{0}'.", tokens[1]));

            int begin = 0, length = data.Length;
            if (tokens.Length >= 3)
                if (!Int32.TryParse(tokens[2], out begin))
                    throw new Exception(string.Format("Third paramter for incbin must be numeric."));

            if (tokens.Length == 3)
            {
                length = length - begin;
            }

            if (tokens.Length == 4)
                if (!Int32.TryParse(tokens[3], out length))
                    throw new Exception(string.Format("Fourth paramter for incbin must be numeric."));

            if ((begin >= length) || (begin + length > data.Length))
                throw new Exception("Out of bounds for incbin.");

            for (int i = 0; i < length; i++)
                m_MachineCodeOutput.Add(data[i + begin]);
            return true;
        }
    }
}
