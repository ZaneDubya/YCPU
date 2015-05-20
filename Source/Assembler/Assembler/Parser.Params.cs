﻿/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;
using System.Linq;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        public ParsedOpcode ParseParam(string param)
        {
            ParsedOpcode ParsedOpcode = new ParsedOpcode();

            param = param.Replace(" ", string.Empty).Trim();

            if (m_Registers.ContainsKey(param))
            {
                ParsedOpcode.OpcodeWord = (ushort)m_Registers[param];
                ParsedOpcode.AddressingMode = AddressingMode.Register;
            }
            else
            {
                if ((param.StartsWith("[") && param.EndsWith("]")) || (param.StartsWith("(") && param.EndsWith(")")))
                {
                    param = param.Substring(1, param.Length - 2).Replace(" ", string.Empty);

                    if (m_Registers.ContainsKey(param))
                    {
                        ParsedOpcode.OpcodeWord = (ushort)m_Registers[param];
                        ParsedOpcode.AddressingMode = AddressingMode.Indirect;
                    }
                    else if (param[param.Length - 1] == '+')
                    {
                        param = param.Substring(0, param.Length - 1);
                        if (m_Registers.ContainsKey(param))
                        {
                            ParsedOpcode.OpcodeWord = (ushort)m_Registers[param];
                        }
                        ParsedOpcode.AddressingMode = AddressingMode.IndirectPostInc;
                    }
                    else if (param[0] == '-')
                    {
                        param = param.Substring(1, param.Length - 1);
                        if (m_Registers.ContainsKey(param))
                        {
                            ParsedOpcode.OpcodeWord = (ushort)m_Registers[param];
                        }
                        ParsedOpcode.AddressingMode = AddressingMode.IndirectPreDec;
                    }
                    else if (param.Contains(','))
                    {
                        string param0 = param.Substring(0, param.IndexOf(',')).Trim();
                        string param1 = param.Substring(param.IndexOf(',') + 1, 
                            param.Length - param.IndexOf(',') - 1).Trim();
                        if (m_Registers.ContainsKey(param0) && m_Registers.ContainsKey(param1))
                        {
                            ParsedOpcode.OpcodeWord = (ushort)(m_Registers[param0] | (m_Registers[param1] << 8));
                            ParsedOpcode.AddressingMode = AddressingMode.IndirectIndexed;
                        }
                        else if (m_Registers.ContainsKey(param0) && CanDecodeLiteral(param1))
                        {
                            ParsedOpcode = ParseLiteralParameter(ParsedOpcode, param1);
                            ParsedOpcode.OpcodeWord = (ushort)(m_Registers[param0]);
                            ParsedOpcode.AddressingMode = AddressingMode.IndirectOffset;
                        }
                        else if (CanDecodeLiteral(param0) && m_Registers.ContainsKey(param1))
                        {
                            ParsedOpcode = ParseLiteralParameter(ParsedOpcode, param0);
                            ParsedOpcode.OpcodeWord = (ushort)(m_Registers[param1]);
                            ParsedOpcode.AddressingMode = AddressingMode.IndirectOffset;
                        }
                        else
                        {
                            ParsedOpcode.IsIllegal = true;
                            return ParsedOpcode;
                        }
                    }
                    else if (CanDecodeLiteral(param))
                    {
                        ParsedOpcode = ParseLiteralParameter(ParsedOpcode, param);
                        ParsedOpcode.AddressingMode = AddressingMode.Absolute;
                    }
                    else
                    {
                        ParsedOpcode.IsIllegal = true;
                        return ParsedOpcode;
                    }
                }
                else
                {
                    ParseLiteralParameter(ParsedOpcode, param);
                }
            }

            return ParsedOpcode;
        }

        public ParsedOpcode ParseMemoryAddressPlusRegisterParameter(ParsedOpcode ParsedOpcode, string param)
        {
            var psplit = param.Split('+');
            if (psplit.Length < 2)
            {
                throw new Exception(string.Format("malformated memory reference '{0}'", param));
            }

            var addressValue = "[+" + psplit[1] + "]";
            if (!m_Registers.ContainsKey(addressValue))
            {
                throw new Exception(string.Format("Invalid register reference in '{0}'", param));
            }

            ParsedOpcode.OpcodeWord = (ushort)m_Registers[addressValue];
            ParsedOpcode.HasImmediateWord = true;

            if (psplit[0].StartsWith("\'") && psplit[0].EndsWith("\'") && psplit[0].Length == 3)
            {
                var val = (ushort)psplit[0][1];
                ParsedOpcode.ImmediateWord = val;
            }
            else if (psplit[0].Contains("0x"))
            {
                ushort val = Convert.ToUInt16(psplit[0].Trim(), 16);
                ParsedOpcode.ImmediateWord = val;
            }
            else if (psplit[0].Trim().All(x => char.IsDigit(x)))
            {
                var val = Convert.ToUInt16(psplit[0].Trim(), 10);
                ParsedOpcode.ImmediateWord = val;
            }
            else
            {
                ParsedOpcode.HasImmediateWord = true;
                ParsedOpcode.LabelName = psplit[0].Trim();
            }

            return ParsedOpcode;
        }

        public ParsedOpcode ParseMemoryAddressParameter(ParsedOpcode ParsedOpcode, string param)
        {
            ParsedOpcode.OpcodeWord = (ushort)0x0000;
            ParsedOpcode.AddressingMode = AddressingMode.Indirect;
            ParsedOpcode.HasImmediateWord = true;

            if (param.StartsWith("\'") && param.EndsWith("\'") && param.Length == 5)
            {
                ushort val = param[1];
                ParsedOpcode.ImmediateWord = val;
            }
            else if (param.Contains("0x"))
            {
                ushort val = Convert.ToUInt16(param.Trim(), 16);
                ParsedOpcode.ImmediateWord = val;
            }
            else if (param.Trim().All(x => char.IsDigit(x)))
            {
                ushort val = Convert.ToUInt16(param.Trim(), 10);
                ParsedOpcode.ImmediateWord = val;
            }
            else
            {
                ParsedOpcode.HasImmediateWord = true;
                ParsedOpcode.LabelName = param.Trim();
            }

            return ParsedOpcode;
        }

        public ParsedOpcode ParseLiteralParameter(ParsedOpcode ParsedOpcode, string param)
        {
            ushort literalValue;

            if (param.StartsWith("\'") && param.EndsWith("\'") && param.Length == 3)
            {
                literalValue = param[1];
            }
            else if (param.Contains("0x"))
            {
                // format: 0x12EF or -0x12EF
                if (param[0] == '-')
                    literalValue = (ushort)(0 - Convert.ToInt16(param.Substring(1, param.Length - 1), 16));
                else
                    literalValue = Convert.ToUInt16(param, 16);
            }
            else if (param.Contains("$"))
            {
                // format: $12EF or -$12EF
                param = param.Replace("$", "0x");
                if (param[0] == '-')
                    literalValue = (ushort)(0 - Convert.ToInt16(param.Substring(1, param.Length - 1), 16));
                else
                    literalValue = Convert.ToUInt16(param, 16);
            }
            else if (param.Trim().All(x => char.IsDigit(x)))
            {
                // format 1234
                literalValue = Convert.ToUInt16(param, 10);
            }
            else if ((param[0] == '-') && (param.Substring(1).Trim().All(x => char.IsDigit(x))))
            {
                // format -1234
                literalValue = (ushort)(0 - Convert.ToInt16(param.Substring(1, param.Length - 1), 10));
            }
            else
            {
                // format LABEL
                ParsedOpcode.OpcodeWord = 0x0000;
                ParsedOpcode.HasImmediateWord = true;
                ParsedOpcode.LabelName = param;
                ParsedOpcode.AddressingMode = AddressingMode.Immediate;
                return ParsedOpcode;
            }

            // unless the parameter is a LABEL, parameter parsing will end with this code:
            ParsedOpcode.AddressingMode = AddressingMode.Immediate;
            ParsedOpcode.HasImmediateWord = true;
            ParsedOpcode.ImmediateWord = literalValue;
            return ParsedOpcode;
        }

        public bool CanDecodeLiteral(string param)
        {
            ParsedOpcode opcode = new ParsedOpcode();
            opcode = ParseLiteralParameter(opcode, param);
            return !opcode.IsIllegal;
        }
    }
}
