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

namespace Ypsilon.Assembler
{
    public partial class Parser
    {
        Dictionary<string, Func<string[], int, ParserState, ushort[]>> m_Opcodes;
        Dictionary<string, ushort> m_Registers;

        bool m_IsNextLineData = false;
        

        public Parser()
        {
            m_Opcodes = new Dictionary<string, Func<string[], int, ParserState, ushort[]>>();
            m_Registers = new Dictionary<string, ushort>();

            InitOpcodeDictionary();
            InitRegisterDictionary();
        }

        public byte[] Parse(string[] lines, string working_directory)
        {
            ParserState state = new ParserState();
            state.m_Directory = working_directory;

            int indexOfCurrentLine = 0;

            try
            {
                foreach (var line in lines)
                {
                    indexOfCurrentLine++;

                    string currentLine = line.Trim();
                    if (currentLine.Length < 1 || line[0] == ';')
                        continue;

                    currentLine = StripComments(currentLine);
                    if (currentLine.Trim().Length == 0)
                        continue;

                    AssembleLine(currentLine, state);
                }

                if (state.m_Scopes.OpenScopes())
                    throw new Exception("Unclosed scope.");

                SetLabelAddressReferences(state);
                SetBranchLabelAddressReferences(state);
                SetDataFieldLabelAddressReferences(state);

                return state.machineCode.ToArray();
            }
            catch (Exception ex)
            {
                AddMessageLine(string.Format("Line {0}: {1}", indexOfCurrentLine, ex.Message));
                return null;
            }
        }

        string StripComments(string line)
        {
            string clearedLine = line;
            int commentIndex = line.IndexOf(";");

            if (commentIndex == 0)
                return string.Empty;
            else if (commentIndex > 0)
                clearedLine = line.Substring(0, commentIndex).Trim();

            return clearedLine;
        }

        void AssembleLine(string line, ParserState state)
        {
            line = line.Trim();

            if (m_IsNextLineData != false)
            {
                m_IsNextLineData = this.ParseData16(line, state);
                return;
            }

            if (RegEx.MatchLabel(line) || RegEx.MatchLabelLocal(line))
            {
                bool local = RegEx.MatchLabelLocal(line);
                // parse label and determine if there is anything else to parse on this line.
                int remaiderLineContentIndex = ParseLabel(line, local, state);
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

            if (ParsePragma(line, opcode, tokens, state))
            {
                // Successfully parsed a pragma, no need to continue with this line.
                return;
            }

            // get the assembler for this opcode. If no assembler exists, throw error.
            Func<string[], int, ParserState, ushort[]> assembler;
            if (!m_Opcodes.TryGetValue(opcode.ToLower(), out assembler))
            {
                throw new Exception(string.Format("Undefined cpu opcode in line {0}", line));
            }

            // get the parameters
            List<string> param = new List<string>();
            for (int i = 1; i < tokens.Length; i++)
                param.Add(tokens[i].Trim());

            // pass the params to the opcode's assembler. If no output, throw error.
            ushort[] code = assembler(param.ToArray(), bit_width, state);
            if (code == null)
                throw new Exception(string.Format("Error assembling line {0}", line));

            // add the output of the assembler to the machine code output. 
            for (int i = 0; i < code.Length; i++)
            {
                ushort this_opcode = code[i];
                state.machineCode.Add((byte)(this_opcode & 0x00ff));
                state.machineCode.Add((byte)((this_opcode & 0xff00) >> 8));
            }
        }

        bool ParseBitWidth(string value, out int bit_width)
        {
            bit_width = 0;
            if (!int.TryParse(value, out bit_width))
                return false;
            if (bit_width != 8 && bit_width != 16 && bit_width != 32)
                return false;
            return true;
        }

        string[] Tokenize(string data)
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

        bool ParseData8(string line, ParserState state)
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

            GenerateInstructionsForDataFields(dataFields, DataFieldTypes.Int8, state);

            return line.EndsWith(",") ? true : false;
        }

        bool ParseData16(string line, ParserState state)
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

            GenerateInstructionsForDataFields(dataFields, DataFieldTypes.Int16, state);

            return line.EndsWith(",") ? true : false;
        }

        private void GenerateInstructionsForDataFields(IList<string> dataFields, DataFieldTypes dataType, ParserState state)
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
                                state.machineCode.Add((byte)c);
                                break;
                            case DataFieldTypes.Int16:
                                state.machineCode.Add((byte)((ushort)c & 0x00ff));
                                state.machineCode.Add((byte)((ushort)(c & 0xff00) >> 8));
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
                        state.DataFields.Add((ushort)state.machineCode.Count, valStr);
                    }

                    switch (dataType)
                    {
                        case DataFieldTypes.Int8:
                            if ((val > byte.MaxValue) || (val < byte.MinValue))
                                throw new Exception(string.Format("Included byte value '{0}' cannot be expressed in an 8-bit value.", data));
                            state.machineCode.Add((byte)val);
                            break;
                        case DataFieldTypes.Int16:
                            if ((val > ushort.MaxValue) || (val < ushort.MinValue))
                                throw new Exception(string.Format("Included ushort value '{0}' cannot be expressed in a 16-bit value.", data));
                            state.machineCode.Add((byte)((ushort)val & 0x00ff));
                            state.machineCode.Add((byte)((ushort)(val & 0xff00) >> 8));
                            break;
                    }
                }
            }
        }
    }
}
