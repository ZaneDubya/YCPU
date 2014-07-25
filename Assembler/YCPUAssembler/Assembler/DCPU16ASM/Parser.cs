/*
 * DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

/**
 * TODOs: 
 * - Far Far Better error checking (provide useful messages on compile failure
 * - 'safer' parsing. Speed coding this has resulted in some bad things
 * - Allow compile time arithmetic, for things like this: 
 *   :dataInMemory  0x9000 
 *   
 *      SET I, [dataInMemory + 0x1] 
 *   Right now you can only do something like this if you throw 0x1 into a seperate register :/ 
 */

namespace YCPU.Assembler.DCPU16ASM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Parser
    {
        public delegate ushort[] OpcodeAssembler(string[] param, int bit_width);

        protected List<byte> m_MachineCode;
        protected Dictionary<ushort, string> m_LabelReferences;
        protected Dictionary<string, OpcodeAssembler> m_OpcodeAssemblers;
        protected readonly Dictionary<ushort, string> m_LabelDataFieldReferences;
        protected Dictionary<string, ushort> m_RegisterDictionary;
        protected bool m_DataNextLine = false;

        protected string m_DefaultTarget = "ycpu";
        protected int m_DefaultBitWidth = 16;

        public Parser()
        {
            m_LabelReferences = new Dictionary<ushort, string>();

            m_MachineCode = new List<byte>();
            m_LabelDataFieldReferences = new Dictionary<ushort, string>();

            m_OpcodeAssemblers = new Dictionary<string, OpcodeAssembler>();
            m_RegisterDictionary = new Dictionary<string, ushort>();

            InitOpcodeDictionary();
            InitRegisterDictionary();
        }

        protected virtual void InitOpcodeDictionary()
        {
            // Initialize the DCPU opcodes
        }

        protected virtual void InitRegisterDictionary()
        {
            // Register dictionary, We'll only include the most common ones in here, others have to be constructed.
        }

        public string MessageOuput { get; protected set; }
        public int LineCounter { get; protected set; }

        public virtual byte[] Parse(string[] lines, string working_directory)
        {
            return null;
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
            line = line.ToLower().Trim();

            if (m_DataNextLine != false)
            {
                m_DataNextLine = this.ParseData16(line);
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
            int bit_width = m_DefaultBitWidth;

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

            if (!this.m_OpcodeAssemblers.ContainsKey(opcode))
            {
                throw new Exception(string.Format("Undefined cpu opcode in line {0}", line));
            }

            // get the parameters
            List<string> param = new List<string>();
            for (int i = 1; i < tokens.Length; i++)
                param.Add(tokens[i].Trim());
            // get the assembler for this opcode
            OpcodeAssembler assembler = m_OpcodeAssemblers[opcode];
            // pass the params to the opcode's assembler
            ushort[] code = assembler(param.ToArray(), bit_width);
            if (code == null)
                throw new Exception(string.Format("Error assembling line {0}", line));
            for (int i = 0; i < code.Length; i++)
            {
                ushort this_opcode = code[i];
                m_MachineCode.Add((byte)(this_opcode & 0x00ff));
                m_MachineCode.Add((byte)((this_opcode & 0xff00) >> 8));
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

        protected virtual bool ParsePragma(string line, string opcode, string[] tokens)
        {
            return false;
        }

        protected virtual int ParseLabel(string line, bool local)
        {
            return -1;
        }

        private string[] Tokenize(string data)
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

            string lineData = m_DataNextLine != true ? line.Substring(6, line.Length - 6).Trim() : line.Trim();
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

            string lineData = m_DataNextLine != true ? line.Substring(6, line.Length - 6).Trim() : line.Trim();
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
                    string asciiLine = data.Replace("\"", string.Empty).Trim();
                    foreach (char c in asciiLine)
                    {
                        switch (dataType)
                        {
                            case DataFieldTypes.Int8:
                                m_MachineCode.Add((byte)c);
                                break;
                            case DataFieldTypes.Int16:
                                m_MachineCode.Add((byte)((ushort)c & 0x00ff));
                                m_MachineCode.Add((byte)((ushort)(c & 0xff00) >> 8));
                                break;
                            case DataFieldTypes.Int32:
                                m_MachineCode.Add((byte)((ushort)c & 0x00ff));
                                m_MachineCode.Add((byte)((ushort)(c & 0xff00) >> 8));
                                m_MachineCode.Add(0x00);
                                m_MachineCode.Add(0x00);
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
                        m_LabelDataFieldReferences.Add((ushort)m_MachineCode.Count, valStr);
                    }

                    switch (dataType)
                    {
                        case DataFieldTypes.Int8:
                            if ((val > byte.MaxValue) || (val < byte.MinValue))
                                throw new Exception(string.Format("Included byte value '{0}' cannot be expressed in an 8-bit value.", data));
                            m_MachineCode.Add((byte)val);
                            break;
                        case DataFieldTypes.Int16:
                            if ((val > ushort.MaxValue) || (val < ushort.MinValue))
                                throw new Exception(string.Format("Included ushort value '{0}' cannot be expressed in a 16-bit value.", data));
                            m_MachineCode.Add((byte)((ushort)val & 0x00ff));
                            m_MachineCode.Add((byte)((ushort)(val & 0xff00) >> 8));
                            break;
                        case DataFieldTypes.Int32:
                            if ((val > uint.MaxValue) || (val < uint.MinValue))
                                throw new Exception(string.Format("Included uint value '{0}' cannot be expressed in a 32-bit value.", data));
                            m_MachineCode.Add((byte)((uint)val & 0x00ff));
                            m_MachineCode.Add((byte)((uint)(val & 0x0000ff00) >> 8));
                            m_MachineCode.Add((byte)((uint)(val & 0x00ff0000) >> 16));
                            m_MachineCode.Add((byte)((uint)(val & 0xff000000) >> 24));
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

        protected virtual void SetLabelAddressReferences()
        {

        }

        protected virtual void SetDataFieldLabelAddressReferences()
        {

        }

        protected void AddMessageLine(string input)
        {
            this.MessageOuput += string.Format("{0}\r\n", input);
        }

        protected void AddMessage(string input)
        {
            this.MessageOuput += input;
        }
    }
}
