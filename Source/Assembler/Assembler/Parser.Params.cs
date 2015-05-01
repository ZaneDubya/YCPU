/*
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

using System;
using System.Linq;

namespace YCPU.Assembler
{
    partial class Parser
    {
        public ParsedOpcode ParseParam(string param)
        {
            var ParsedOpcode = new ParsedOpcode();
            var clearedParameter = param.Replace(" ", string.Empty).Trim();

            if (this.m_RegisterDictionary.ContainsKey(clearedParameter))
            {
                ParsedOpcode.Word = (ushort)this.m_RegisterDictionary[clearedParameter];
                ParsedOpcode.AddressingMode = AddressingMode.Register;
            }
            else
            {
                if ((clearedParameter.StartsWith("[") && clearedParameter.EndsWith("]")) || (clearedParameter.StartsWith("(") && clearedParameter.EndsWith(")")))
                {
                    clearedParameter = clearedParameter.Substring(1, clearedParameter.Length - 2).Replace(" ", string.Empty);

                    if (this.m_RegisterDictionary.ContainsKey(clearedParameter))
                    {
                        ParsedOpcode.Word = (ushort)this.m_RegisterDictionary[clearedParameter];
                        ParsedOpcode.AddressingMode = AddressingMode.Indirect;
                    }
                    else if (clearedParameter[clearedParameter.Length - 1] == '+')
                    {
                        clearedParameter = clearedParameter.Substring(0, clearedParameter.Length - 1);
                        if (this.m_RegisterDictionary.ContainsKey(clearedParameter))
                        {
                            ParsedOpcode.Word = (ushort)this.m_RegisterDictionary[clearedParameter];
                        }
                        ParsedOpcode.AddressingMode = AddressingMode.IndirectPostInc;
                    }
                    else if (clearedParameter[0] == '-')
                    {
                        clearedParameter = clearedParameter.Substring(1, clearedParameter.Length - 1);
                        if (this.m_RegisterDictionary.ContainsKey(clearedParameter))
                        {
                            ParsedOpcode.Word = (ushort)this.m_RegisterDictionary[clearedParameter];
                        }
                        ParsedOpcode.AddressingMode = AddressingMode.IndirectPreDec;
                    }
                    else if (clearedParameter.Contains(','))
                    {
                        string param0 = clearedParameter.Substring(0, clearedParameter.IndexOf(',')).Trim();
                        string param1 = clearedParameter.Substring(clearedParameter.IndexOf(',') + 1, 
                            clearedParameter.Length - clearedParameter.IndexOf(',') - 1).Trim();
                        if (m_RegisterDictionary.ContainsKey(param0) && m_RegisterDictionary.ContainsKey(param1))
                        {
                            ParsedOpcode.Word = (ushort)(m_RegisterDictionary[param0] | (m_RegisterDictionary[param1] << 8));
                            ParsedOpcode.AddressingMode = AddressingMode.IndirectIndexed;
                        }
                        else if (m_RegisterDictionary.ContainsKey(param0) && CanDecodeLiteral(param1))
                        {
                            ParsedOpcode = ParseLiteralParameter(ParsedOpcode, param1);
                            ParsedOpcode.Word = (ushort)(m_RegisterDictionary[param0]);
                            ParsedOpcode.AddressingMode = AddressingMode.IndirectOffset;
                        }
                        else if (CanDecodeLiteral(param0) && m_RegisterDictionary.ContainsKey(param1))
                        {
                            ParsedOpcode = ParseLiteralParameter(ParsedOpcode, param0);
                            ParsedOpcode.Word = (ushort)(m_RegisterDictionary[param1]);
                            ParsedOpcode.AddressingMode = AddressingMode.IndirectOffset;
                        }
                        else
                        {
                            ParsedOpcode.Illegal = true;
                            return ParsedOpcode;
                        }
                    }
                    else if (CanDecodeLiteral(clearedParameter))
                    {
                        ParsedOpcode = ParseLiteralParameter(ParsedOpcode, clearedParameter);
                        ParsedOpcode.AddressingMode = AddressingMode.Absolute;
                    }
                    else
                    {
                        ParsedOpcode.Illegal = true;
                        return ParsedOpcode;
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
            if (!this.m_RegisterDictionary.ContainsKey(addressValue))
            {
                throw new Exception(string.Format("Invalid register reference in '{0}'", clearedParameter));
            }

            ParsedOpcode.Word = (ushort)this.m_RegisterDictionary[addressValue];
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
            ParsedOpcode.Word = (ushort)0x0000;
            ParsedOpcode.AddressingMode = AddressingMode.Indirect;
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
                // format: 0x12EF or -0x12EF
                string param = clearedParameter;
                if (param[0] == '-')
                    literalValue = (ushort)(0 - Convert.ToInt16(param.Substring(1, param.Length - 1), 16));
                else
                    literalValue = Convert.ToUInt16(clearedParameter, 16);
            }
            else if (clearedParameter.Contains("$"))
            {
                // format: $12EF or -$12EF
                string param = clearedParameter.Replace("$", "0x");
                if (param[0] == '-')
                    literalValue = (ushort)(0 - Convert.ToInt16(param.Substring(1, param.Length - 1), 16));
                else
                    literalValue = Convert.ToUInt16(param, 16);
            }
            else if (clearedParameter.Trim().All(x => char.IsDigit(x)))
            {
                // format 1234
                literalValue = Convert.ToUInt16(clearedParameter, 10);
            }
            else if ((clearedParameter[0] == '-') && (clearedParameter.Substring(1).Trim().All(x => char.IsDigit(x))))
            {
                // format -1234
                literalValue = (ushort)(0 - Convert.ToInt16(clearedParameter.Substring(1, clearedParameter.Length - 1), 10));
            }
            else
            {
                // format LABEL
                ParsedOpcode.Word = 0x0000;
                ParsedOpcode.UsesNextWord = true;
                ParsedOpcode.LabelName = clearedParameter;
                ParsedOpcode.AddressingMode = AddressingMode.Immediate;
                return ParsedOpcode;
            }

            // unless the parameter is a LABEL, parameter parsing will end with this code:
            ParsedOpcode.AddressingMode = AddressingMode.Immediate;
            ParsedOpcode.UsesNextWord = true;
            ParsedOpcode.NextWord = literalValue;
            return ParsedOpcode;
        }

        public bool CanDecodeLiteral(string param)
        {
            ParsedOpcode opcode = new ParsedOpcode();
            opcode = ParseLiteralParameter(opcode, param);
            return !opcode.Illegal;
        }
    }
}
