/*
 * DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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

    public class Parser
    {
        private readonly List<ushort> machineCode;
        private readonly Dictionary<ushort, string> labelReferences;
        private Dictionary<string, ushort> opcodeDictionary;
        private readonly Dictionary<string, ushort> labelAddressDitionary;
        private readonly Dictionary<ushort, string> labelDataFieldReferences;
        private Dictionary<string, dcpuRegisterCodes> registerDictionary;
        private bool dataNextLine = false;

        public Parser()
        {
            this.labelAddressDitionary = new Dictionary<string, ushort>();
            this.labelReferences = new Dictionary<ushort, string>();
            this.machineCode = new List<ushort>();
            this.labelDataFieldReferences = new Dictionary<ushort, string>();

            InitOpcodeDictionary();
            InitRegisterDictionary();
        }

        protected virtual void InitOpcodeDictionary()
        {
            this.opcodeDictionary = new Dictionary<string, ushort>
            {
                // non basic instructions
                { "jsr", (ushort)dcpuOpCode.JSR_OP },

                // basic instructions
                { "set", (ushort)dcpuOpCode.SET_OP },
                { "add", (ushort)dcpuOpCode.ADD_OP },
                { "sub", (ushort)dcpuOpCode.SUB_OP },
                { "mul", (ushort)dcpuOpCode.MUL_OP },
                { "div", (ushort)dcpuOpCode.DIV_OP },
                { "mod", (ushort)dcpuOpCode.MOD_OP },
                { "shl", (ushort)dcpuOpCode.SHL_OP },
                { "shr", (ushort)dcpuOpCode.SHR_OP },
                { "and", (ushort)dcpuOpCode.AND_OP },
                { "bor", (ushort)dcpuOpCode.BOR_OP },
                { "xor", (ushort)dcpuOpCode.XOR_OP },
                { "ife", (ushort)dcpuOpCode.IFE_OP },
                { "ifn", (ushort)dcpuOpCode.IFN_OP },
                { "ifg", (ushort)dcpuOpCode.IFG_OP },
                { "ifb", (ushort)dcpuOpCode.IFB_OP },
            };
        }

        protected virtual void InitRegisterDictionary()
        {
            // Register dictionary, We'll only include the most common ones in here, others have to be constructred. 
            this.registerDictionary = new Dictionary<string, dcpuRegisterCodes>
            {
                { "a", dcpuRegisterCodes.A },
                { "b", dcpuRegisterCodes.B },
                { "c", dcpuRegisterCodes.C },
                { "x", dcpuRegisterCodes.X },
                { "y", dcpuRegisterCodes.Y },
                { "z", dcpuRegisterCodes.Z },
                { "i", dcpuRegisterCodes.I },
                { "j", dcpuRegisterCodes.J },
                { "[a]", dcpuRegisterCodes.A_Mem },
                { "[b]", dcpuRegisterCodes.B_Mem },
                { "[c]", dcpuRegisterCodes.C_Mem },
                { "[x]", dcpuRegisterCodes.X_Mem },
                { "[y]", dcpuRegisterCodes.Y_Mem },
                { "[z]", dcpuRegisterCodes.Z_Mem },
                { "[i]", dcpuRegisterCodes.I_Mem },
                { "[j]", dcpuRegisterCodes.J_Mem },
                { "pop", dcpuRegisterCodes.POP },
                { "peek", dcpuRegisterCodes.PEEK },
                { "push", dcpuRegisterCodes.PUSH },
                { "sp", dcpuRegisterCodes.SP },
                { "pc", dcpuRegisterCodes.PC },
                { "o", dcpuRegisterCodes.O },
                { "[+a]", dcpuRegisterCodes.A_NextWord },
                { "[+b]", dcpuRegisterCodes.B_NextWord },
                { "[+c]", dcpuRegisterCodes.C_NextWord },
                { "[+x]", dcpuRegisterCodes.X_NextWord },
                { "[+y]", dcpuRegisterCodes.Y_NextWord },
                { "[+z]", dcpuRegisterCodes.Z_NextWord },
                { "[+i]", dcpuRegisterCodes.I_NextWord },
                { "[+j]", dcpuRegisterCodes.J_NextWord }
            };
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

            var tokens = this.Tokenize(line);
            var token = tokens[0].Trim();

            if (token.ToLower() == "dat")
            {
                dataNextLine = this.ParseDat(line);
                return;
            }

            if (!this.opcodeDictionary.ContainsKey(token))
            {
                throw new Exception(string.Format("Illegal cpu opcode --> {0}", tokens[0]));
            }

            var opcode = (uint)this.opcodeDictionary[token];
            var param = tokens[1].Trim();

            if ((opcode & 0xF) > 0x0)
            {
                opcode &= 0xF;
                var param1 = tokens[2];
                GenerateInstruction(opcode, param, param1);
            }
            else
            {
                GenerateInstruction(opcode, param);
            }
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
            var tokens = data.Split(new[] { ' ', '\t', ',' });
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

        void GenerateInstruction(uint opcode, string param1, string param2)
        {
            var p1 = this.ParseParam(param1);
            var p2 = this.ParseParam(param2);

            opcode |= ((uint)p1.Word << 4) & 0x3F0;
            opcode |= ((uint)p2.Word << 10) & 0xFC00;

            this.machineCode.Add((ushort)opcode);

            if (p1.UsesNextWord)
            {
                if (p1.LabelName.Length > 0)
                {
                    this.labelReferences.Add((ushort)this.machineCode.Count, p1.LabelName);
                }

                this.machineCode.Add(p1.NextWord);
            }

            if (p2.UsesNextWord)
            {
                if (p2.LabelName.Length > 0)
                {
                    this.labelReferences.Add((ushort)this.machineCode.Count, p2.LabelName);
                }

                this.machineCode.Add(p2.NextWord);
            }
        }

        void GenerateInstruction(uint opcode, string param)
        {
            ParsedOpcode p1 = this.ParseParam(param);
            opcode |= ((uint)p1.Word << 10) & 0xFC00;

            this.machineCode.Add((ushort)opcode);

            if (p1.UsesNextWord)
            {
                if (p1.LabelName.Length > 0)
                {
                    this.labelReferences.Add((ushort)this.machineCode.Count, p1.LabelName);
                }

                this.machineCode.Add(p1.NextWord);
            }
        }

        private ParsedOpcode ParseParam(string param)
        {
            var ParsedOpcode = new ParsedOpcode();

            var clearedParameter = param.Replace(" ", string.Empty).Trim();

            if (this.registerDictionary.ContainsKey(clearedParameter))
            {
                ParsedOpcode.Word = (ushort)this.registerDictionary[clearedParameter];
            }
            else
            {
                if ((clearedParameter.StartsWith("[") || clearedParameter.StartsWith("(")) && (clearedParameter.EndsWith("]") || clearedParameter.EndsWith(")")))
                {
                    clearedParameter = clearedParameter.Substring(1, clearedParameter.Length - 2).Replace(" ", string.Empty);

                    if (clearedParameter.Contains("+"))
                    {
                        ParsedOpcode = ParseMemoryAddressPlusRegisterParameter(ParsedOpcode, clearedParameter);
                    }
                    else
                    {
                        ParsedOpcode = ParseMemoryAddressParameter(ParsedOpcode, clearedParameter);
                    }
                }
                else
                {
                    ParseLiteralParameter(ParsedOpcode, clearedParameter);
                }
            }

            return ParsedOpcode;
        }

        public ParsedOpcode ParseMemoryAddressPlusRegisterParameter(ParsedOpcode ParsedOpcode, string clearedParameter)
        {
            var psplit = clearedParameter.Split('+');
            if (psplit.Length < 2)
            {
                throw new Exception(string.Format("malformated memory reference '{0}'", clearedParameter));
            }

            var addressValue = "[+" + psplit[1] + "]";
            if (!this.registerDictionary.ContainsKey(addressValue))
            {
                throw new Exception(string.Format("Invalid register reference in '{0}'", clearedParameter));
            }

            ParsedOpcode.Word = (ushort)this.registerDictionary[addressValue];
            ParsedOpcode.UsesNextWord = true;

            if (psplit[0].StartsWith("\'") && psplit[0].EndsWith("\'") && psplit[0].Length == 3)
            {
                var val = (ushort)psplit[0][1];
                ParsedOpcode.NextWord = val;
            }
            else if (psplit[0].Contains("0x"))
            {
                ushort val = Convert.ToUInt16(psplit[0].Trim(), 16);
                ParsedOpcode.NextWord = val;
            }
            else if (psplit[0].Trim().All(x => char.IsDigit(x)))
            {
                var val = Convert.ToUInt16(psplit[0].Trim(), 10);
                ParsedOpcode.NextWord = val;
            }
            else
            {
                ParsedOpcode.UsesNextWord = true;
                ParsedOpcode.LabelName = psplit[0].Trim();
            }

            return ParsedOpcode;
        }

        public ParsedOpcode ParseMemoryAddressParameter(ParsedOpcode ParsedOpcode, string clearedParameter)
        {
            ParsedOpcode.Word = (ushort)dcpuRegisterCodes.NextWord_Literal_Mem;
            ParsedOpcode.UsesNextWord = true;

            if (clearedParameter.StartsWith("\'") && clearedParameter.EndsWith("\'") && clearedParameter.Length == 5)
            {
                ushort val = clearedParameter[1];
                ParsedOpcode.NextWord = val;
            }
            else if (clearedParameter.Contains("0x"))
            {
                ushort val = Convert.ToUInt16(clearedParameter.Trim(), 16);
                ParsedOpcode.NextWord = val;
            }
            else if (clearedParameter.Trim().All(x => char.IsDigit(x)))
            {
                ushort val = Convert.ToUInt16(clearedParameter.Trim(), 10);
                ParsedOpcode.NextWord = val;
            }
            else
            {
                ParsedOpcode.UsesNextWord = true;
                ParsedOpcode.LabelName = clearedParameter.Trim();
            }

            return ParsedOpcode;
        }

        public ParsedOpcode ParseLiteralParameter(ParsedOpcode ParsedOpcode, string clearedParameter)
        {
            ushort literalValue;

            if (clearedParameter.StartsWith("\'") && clearedParameter.EndsWith("\'") && clearedParameter.Length == 3)
            {
                literalValue = clearedParameter[1];
            }
            else if (clearedParameter.Contains("0x"))
            {
                literalValue = Convert.ToUInt16(clearedParameter, 16);
            }
            else if (clearedParameter.Trim().All(x => char.IsDigit(x)))
            {
                literalValue = Convert.ToUInt16(clearedParameter, 10);
            }
            else
            {
                ParsedOpcode.Word = (ushort)dcpuRegisterCodes.NextWord_Literal_Value;
                ParsedOpcode.UsesNextWord = true;
                ParsedOpcode.LabelName = clearedParameter;
                return ParsedOpcode;
            }

            ushort maxValue = 0x1F;

            if (literalValue < maxValue)
            {
                ParsedOpcode.Word = 0x20;
                ParsedOpcode.Word += literalValue;
            }
            else
            {
                ParsedOpcode.Word = (ushort)dcpuRegisterCodes.NextWord_Literal_Value;
                ParsedOpcode.UsesNextWord = true;
                ParsedOpcode.NextWord = literalValue;
            }

            return ParsedOpcode;
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

                this.machineCode[key] = this.labelAddressDitionary[labelName];
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
