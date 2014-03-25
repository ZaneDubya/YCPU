﻿/*
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
        private readonly List<ushort> machineCode;
        private readonly Dictionary<ushort, string> labelReferences;
        protected Dictionary<string, OpcodeAssembler> m_OpcodeAssemblers;
        private readonly Dictionary<string, ushort> labelAddressDitionary;
        private readonly Dictionary<ushort, string> labelDataFieldReferences;
        protected Dictionary<string, ushort> m_RegisterDictionary;
        private bool dataNextLine = false;

        public Parser()
        {
            this.labelAddressDitionary = new Dictionary<string, ushort>();
            this.labelReferences = new Dictionary<ushort, string>();
            this.machineCode = new List<ushort>();
            this.labelDataFieldReferences = new Dictionary<ushort, string>();

            m_OpcodeAssemblers = new Dictionary<string, OpcodeAssembler>();
            m_RegisterDictionary = new Dictionary<string, ushort>();

            InitOpcodeDictionary();
            InitRegisterDictionary();
        }

        protected virtual void InitOpcodeDictionary()
        {
            // Initialize the DCPU opcodes
            {
                // non basic instructions
                m_OpcodeAssemblers.Add("jsr", AssembleJSR);
                // basic instructions
                m_OpcodeAssemblers.Add("set", AssembleSET);
                m_OpcodeAssemblers.Add("add", AssembleADD);
                m_OpcodeAssemblers.Add("sub", AssembleSUB);
                m_OpcodeAssemblers.Add("mul", AssembleMUL);
                m_OpcodeAssemblers.Add("div", AssembleDIV);
                m_OpcodeAssemblers.Add("mod", AssembleMOD);
                m_OpcodeAssemblers.Add("shl", AssembleSHL);
                m_OpcodeAssemblers.Add("shr", AssembleSHR);
                m_OpcodeAssemblers.Add("and", AssembleAND);
                m_OpcodeAssemblers.Add("bor", AssembleBOR);
                m_OpcodeAssemblers.Add("xor", AssembleXOR);
                m_OpcodeAssemblers.Add("ife", AssembleIFE);
                m_OpcodeAssemblers.Add("ifn", AssembleIFN);
                m_OpcodeAssemblers.Add("ifg", AssembleIFG);
                m_OpcodeAssemblers.Add("ifb", AssembleIFB);
            };
        }

        protected virtual void InitRegisterDictionary()
        {
            // Register dictionary, We'll only include the most common ones in here, others have to be constructed.
            m_RegisterDictionary.Add("a", (ushort)dcpuRegisterCodes.A);
            m_RegisterDictionary.Add("b", (ushort)dcpuRegisterCodes.B);
            m_RegisterDictionary.Add("c", (ushort)dcpuRegisterCodes.C);
            m_RegisterDictionary.Add("x", (ushort)dcpuRegisterCodes.X);
            m_RegisterDictionary.Add("y", (ushort)dcpuRegisterCodes.Y);
            m_RegisterDictionary.Add("z", (ushort)dcpuRegisterCodes.Z);
            m_RegisterDictionary.Add("i", (ushort)dcpuRegisterCodes.I);
            m_RegisterDictionary.Add("j", (ushort)dcpuRegisterCodes.J);
            m_RegisterDictionary.Add("[a]", (ushort)dcpuRegisterCodes.A_Mem);
            m_RegisterDictionary.Add("[b]", (ushort)dcpuRegisterCodes.B_Mem);
            m_RegisterDictionary.Add("[c]", (ushort)dcpuRegisterCodes.C_Mem);
            m_RegisterDictionary.Add("[x]", (ushort)dcpuRegisterCodes.X_Mem);
            m_RegisterDictionary.Add("[y]", (ushort)dcpuRegisterCodes.Y_Mem);
            m_RegisterDictionary.Add("[z]", (ushort)dcpuRegisterCodes.Z_Mem);
            m_RegisterDictionary.Add("[i]", (ushort)dcpuRegisterCodes.I_Mem);
            m_RegisterDictionary.Add("[j]", (ushort)dcpuRegisterCodes.J_Mem);
            m_RegisterDictionary.Add("pop", (ushort)dcpuRegisterCodes.POP);
            m_RegisterDictionary.Add("peek", (ushort)dcpuRegisterCodes.PEEK);
            m_RegisterDictionary.Add("push", (ushort)dcpuRegisterCodes.PUSH);
            m_RegisterDictionary.Add("sp", (ushort)dcpuRegisterCodes.SP);
            m_RegisterDictionary.Add("pc", (ushort)dcpuRegisterCodes.PC);
            m_RegisterDictionary.Add("o", (ushort)dcpuRegisterCodes.O);
            m_RegisterDictionary.Add("[+a]", (ushort)dcpuRegisterCodes.A_NextWord);
            m_RegisterDictionary.Add("[+b]", (ushort)dcpuRegisterCodes.B_NextWord);
            m_RegisterDictionary.Add("[+c]", (ushort)dcpuRegisterCodes.C_NextWord);
            m_RegisterDictionary.Add("[+x]", (ushort)dcpuRegisterCodes.X_NextWord);
            m_RegisterDictionary.Add("[+y]", (ushort)dcpuRegisterCodes.Y_NextWord);
            m_RegisterDictionary.Add("[+z]", (ushort)dcpuRegisterCodes.Z_NextWord);
            m_RegisterDictionary.Add("[+i]", (ushort)dcpuRegisterCodes.I_NextWord);
            m_RegisterDictionary.Add("[+j]", (ushort)dcpuRegisterCodes.J_NextWord);
        }

        public string MessageOuput { get; private set; }
        public int LineCounter { get; private set; }

        public ushort[] Parse(string[] lines)
        {
            try
            {
                this.machineCode.Clear();
                this.labelReferences.Clear();
                this.MessageOuput = string.Empty;

                foreach (var line in lines)
                {
                    LineCounter++;

                    var currentLine = line.Trim();

                    if (currentLine.Length < 1 || line[0] == ';')
                    {
                        continue;
                    }

                    currentLine = this.RemoveLineComments(line);

                    if (currentLine.Trim().Length < 1)
                    {
                        continue;
                    }

                    this.AssembleLine(currentLine);
                }

                this.SetLabelAddressReferences();
                SetDataFieldLabelAddressReferences();

                var count = 1;

                foreach (var code in this.machineCode)
                {
                    this.AddMessage(string.Format("{0:X4} ", code));
                    count++;
                }

                this.MessageOuput = this.MessageOuput.Substring(0, this.MessageOuput.Length - 2);

                return this.machineCode.ToArray();
            }
            catch (Exception ex)
            {
                this.AddMessageLine(string.Format("Line {0}: {1}", LineCounter, ex.Message));
                return null;
            }
        }

        private string RemoveLineComments(string line)
        {
            var clearedLine = line;

            var commentIndex = line.IndexOf(";");

            if (commentIndex > 0)
            {
                clearedLine = line.Substring(0, commentIndex).Trim();
            }

            return clearedLine;
        }

        private void AssembleLine(string line)
        {
            line = line.ToLower().Trim();

            if (dataNextLine != false)
            {
                dataNextLine = this.ParseDat(line);
                return;
            }

            if (RegEx.MatchLabel(line))
            {
                var remaiderLineContentIndex = this.ParseLabel(line);

                if (remaiderLineContentIndex <= 0)
                {
                    return;
                }

                line = line.Remove(0, remaiderLineContentIndex).Trim();

                if (line.Length < 1)
                {
                    return;
                }
            }

            string[] tokens = this.Tokenize(line);
            string token = tokens[0].Trim();

            if (token.ToLower() == "dat")
            {
                dataNextLine = this.ParseDat(line);
                return;
            }

            if (!this.m_OpcodeAssemblers.ContainsKey(token))
            {
                throw new Exception(string.Format("Illegal cpu opcode --> {0}", tokens[0]));
            }

            OpcodeAssembler assembler = this.m_OpcodeAssemblers[token];
            string param = tokens[1].Trim();
            string param1 = (tokens.Length > 2) ? tokens[2].Trim() : string.Empty;
            ushort[] code = assembler(param, param1);
            for (int i = 0; i < code.Length; i++)
                this.machineCode.Add(code[i]);
        }

        private int ParseLabel(string line)
        {
            int index1 = line.IndexOf(' ');
            int index2 = line.IndexOf('\t');
            int index = index1 < index2 || index2 == -1 ? index1 : index2 < index1 || index1 != -1 ? index2 : -1;

            int colon_pos = line.IndexOf(':');
            string labelName;
            if (colon_pos == 0)
                labelName = index > 1 ? line.Substring(1, index - 1) : line.Substring(1, line.Length - 1);
            else
                labelName = line.Substring(0, colon_pos);
                
            if (this.labelAddressDitionary.ContainsKey(labelName))
            {
                throw new Exception(string.Format("Error! Label '{0}' already exists!", labelName));
            }

            this.labelAddressDitionary.Add(labelName.Trim(), (ushort)this.machineCode.Count);

            return index;
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

        private bool ParseDat(string line)
        {
            var dataFields = new List<string>();

            var lineData = dataNextLine != true ? line.Substring(3, line.Length - 3).Trim() : line.Trim();
            foreach (var field in lineData.Split(','))
            {
                if (field.Trim() == string.Empty) continue;
                if (dataFields.Count == 0)
                {
                    dataFields.Add(field);
                }
                else
                {
                    var count = 0;
                    var last = -1;
                    var lastStr = dataFields[dataFields.Count - 1];

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

            GenerateInstructionsForDataFields(dataFields);

            return line.EndsWith(",") ? true : false;
        }

        private void GenerateInstructionsForDataFields(IList<string> dataFields)
        {
            foreach (var dat in dataFields)
            {
                var valStr = dat.Trim();
                if (valStr.IndexOf('"') > -1)
                {
                    var asciiLine = dat.Replace("\"", string.Empty).Trim();
                    foreach (var t in asciiLine)
                    {
                        this.machineCode.Add(t);
                    }
                }
                else
                {
                    var val = (ushort)0;

                    if (valStr.Contains("0x") != false)
                    {
                        val = Convert.ToUInt16(valStr, 16);
                    }
                    else if (valStr.All(x => char.IsDigit(x)))
                    {
                        val = Convert.ToUInt16(valStr, 10);
                    }
                    else
                    {
                        this.labelDataFieldReferences.Add((ushort)this.machineCode.Count, valStr);
                    }

                    this.machineCode.Add(val);
                }
            }
        }

        private void SetLabelAddressReferences()
        {
            foreach (ushort key in this.labelReferences.Keys)
            {
                var labelName = this.labelReferences[key];

                if (!this.labelAddressDitionary.ContainsKey(labelName))
                {
                    throw new Exception(string.Format("Unknown label reference '{0}'", labelName));
                }

                machineCode[key] = labelAddressDitionary[labelName];
            }
        }

        private void SetDataFieldLabelAddressReferences()
        {
            foreach (ushort key in labelDataFieldReferences.Keys)
            {
                string labelName = labelDataFieldReferences[key];

                if (labelAddressDitionary.ContainsKey(labelName) != true)
                {
                    throw new Exception(string.Format("Unknown label '{0}' referenced in data field", labelName));
                }

                machineCode[key] = labelAddressDitionary[labelName];
            }
        }

        private void AddMessageLine(string input)
        {
            this.MessageOuput += string.Format("{0}\r\n", input);
        }

        private void AddMessage(string input)
        {
            this.MessageOuput += input;
        }
    }
}