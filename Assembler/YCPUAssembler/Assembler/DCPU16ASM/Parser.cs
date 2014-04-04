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
        protected List<ushort> m_MachineCode;
        protected Dictionary<ushort, string> m_LabelReferences;
        protected Dictionary<string, OpcodeAssembler> m_OpcodeAssemblers;
        protected readonly Dictionary<ushort, string> m_LabelDataFieldReferences;
        protected Dictionary<string, ushort> m_RegisterDictionary;
        protected bool m_DataNextLine = false;

        public Parser()
        {
            m_LabelReferences = new Dictionary<ushort, string>();

            m_MachineCode = new List<ushort>();
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

        public virtual ushort[] Parse(string[] lines)
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
                m_DataNextLine = this.ParseData(line);
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
            ushort[] code = assembler(param.ToArray());
            if (code == null)
                throw new Exception(string.Format("Error assembling line {0}", line));
            for (int i = 0; i < code.Length; i++)
                m_MachineCode.Add(code[i]);
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

        protected bool ParseData(string line)
        {
            List<string> dataFields = new List<string>();

            string lineData = m_DataNextLine != true ? line.Substring(4, line.Length - 4).Trim() : line.Trim();
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
                        this.m_MachineCode.Add(t);
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
                        this.m_LabelDataFieldReferences.Add((ushort)this.m_MachineCode.Count, valStr);
                    }

                    this.m_MachineCode.Add(val);
                }
            }
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
