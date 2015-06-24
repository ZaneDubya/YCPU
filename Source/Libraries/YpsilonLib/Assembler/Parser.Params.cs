/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        public ParsedOpcode ParseParam(string originalParam)
        {
            if (originalParam == null)
                return null;

            ParsedOpcode ParsedOpcode = new ParsedOpcode();

            // get rid of ALL white space!
            string param = originalParam.Replace(" ", string.Empty).Trim().ToLowerInvariant();

            if (m_Registers.ContainsKey(param))
            {
                // Register: R0
                ParsedOpcode.OpcodeWord = (ushort)m_Registers[param];
                ParsedOpcode.AddressingMode = AddressingMode.Register;
            }
            else if (m_StatusRegisters.ContainsKey(param))
            {
                // Processor Register: Pc
                ParsedOpcode.OpcodeWord = (ushort)m_StatusRegisters[param];
                ParsedOpcode.AddressingMode = AddressingMode.StatusRegister;
            }
            else if (isStackOffset(param))
            {
                string stackOffset = param.Substring(2, param.Length - 3);
                if (CanDecodeLiteral(stackOffset))
                {
                    TryParseLiteralParameter(ParsedOpcode, stackOffset);
                    if (ParsedOpcode.ImmediateWord < 8)
                    {
                        ParsedOpcode.OpcodeWord = ParsedOpcode.ImmediateWord;
                        ParsedOpcode.AddressingMode = AddressingMode.StackAccess;
                    }
                    else
                    {
                        throw new Exception(string.Format("Stack offset exceeds bounds of 0 - 7: '{0}'", originalParam));
                    }
                }
                else
                {
                    throw new Exception(string.Format("Stack offset must be a liternal: '{0}'", originalParam));
                }
            }
            else if (isParamBracketed(param))
            {
                param = removeBrackets(param);

                if (m_Registers.ContainsKey(param))
                {
                    // Indirect: [R0]
                    ParsedOpcode.OpcodeWord = (ushort)m_Registers[param];
                    ParsedOpcode.AddressingMode = AddressingMode.Indirect;
                }
                else if (param.IndexOf('+') == param.Length - 1)
                {
                    // Indirect Post-Increment: [R0+]
                    param = param.Substring(0, param.Length - 1);
                    if (m_Registers.ContainsKey(param))
                    {
                        ParsedOpcode.OpcodeWord = (ushort)m_Registers[param];
                    }
                    ParsedOpcode.AddressingMode = AddressingMode.IndirectPostInc;
                }
                else if (param.IndexOf('-') == 0)
                {
                    // Indirect Pre-Decrement: [-R0]
                    param = param.Substring(1, param.Length - 1);
                    if (m_Registers.ContainsKey(param))
                    {
                        ParsedOpcode.OpcodeWord = (ushort)m_Registers[param];
                    }
                    ParsedOpcode.AddressingMode = AddressingMode.IndirectPreDec;
                }
                else if (param.IndexOf(',') != -1)
                {
                    // Indexed; can be [R0,$0000], [$0000,R0], or [R0,R1].
                    string param0 = param.Substring(0, param.IndexOf(',')).Trim(); // base operand
                    string param1 = param.Substring(param.IndexOf(',') + 1, param.Length - param.IndexOf(',') - 1).Trim(); // index operand

                    if (m_Registers.ContainsKey(param0) && m_Registers.ContainsKey(param1))
                    {
                        // Register is both base and index: [R0,R1].
                        ParsedOpcode.OpcodeWord = (ushort)(m_Registers[param0] | ((m_Registers[param1])<< 8));
                        ParsedOpcode.AddressingMode = AddressingMode.IndirectIndexed;
                    }
                    else if (m_Registers.ContainsKey(param0) && CanDecodeLiteral(param1))
                    {
                        // Value base, Register index: [R0,$0000]
                        TryParseLiteralParameter(ParsedOpcode, param1);
                        ParsedOpcode.OpcodeWord = (ushort)(m_Registers[param0]);
                        ParsedOpcode.AddressingMode = AddressingMode.IndirectOffset;
                    }
                    else if (CanDecodeLiteral(param0) && m_Registers.ContainsKey(param1))
                    {
                        // Value base, Register index: [$0000,R0]
                        TryParseLiteralParameter(ParsedOpcode, param0);
                        ParsedOpcode.OpcodeWord = (ushort)(m_Registers[param1]);
                        ParsedOpcode.AddressingMode = AddressingMode.IndirectOffset;
                    }
                    else
                    {
                        // indexed register is invalid.
                        throw new Exception(string.Format("Invalid operand '{0}'", originalParam));
                    }
                }
                else if (CanDecodeLiteral(param))
                {
                    // Absolute (literal indirect): [$0000]
                    TryParseLiteralParameter(ParsedOpcode, param);
                    ParsedOpcode.AddressingMode = AddressingMode.Absolute;
                }
                else
                {
                    // invlid bracketed operand.
                    throw new Exception(string.Format("Invalid operand '{0}'", originalParam));
                }
            }
            else if (CanDecodeLiteral(param))
            {
                // Literal: $0000
                TryParseLiteralParameter(ParsedOpcode, param);
            }
            else
            {
                // what is this?! not an acceptable operand, that's for certain!
                throw new Exception(string.Format("Invalid operand '{0}'", originalParam));
            }

            return ParsedOpcode;
        }

        bool TryParseLiteralParameter(ParsedOpcode parsedOpcode, string originalParam)
        {
            ushort? literalValue = null;
            ushort value;

            string param = originalParam;

            if (param.Contains("$")) // allow both $ and 0x as indicators of hex numbers. other formats as well?
                param = param.Replace("$", "0x");

            if (param.Contains("0x"))
            {
                // format: 0x12EF or -0x12EF
                if (param.IndexOf('-') == 0)
                {
                    try
                    {
                        literalValue = (ushort)(0 - Convert.ToInt16(param.Substring(1, param.Length - 1), 16));
                    }
                    catch
                    {
                        throw new Exception(string.Format("Could not parse this hexidecimal parameter: '{0}'", originalParam));
                    }
                }
                else
                {
                    try
                    {
                        literalValue = Convert.ToUInt16(param, 16);
                    }
                    catch
                    {
                        throw new Exception(string.Format("Could not parse this hexidecimal parameter: '{0}'", originalParam));
                    }
                }
            }
            else if (ushort.TryParse(param, out value))
            {
                // format 1234
                literalValue = value;
            }
            else if ((param.IndexOf('-') == 0) && (ushort.TryParse(param.Substring(1), out value)))
            {
                // format -1234
                literalValue = (ushort)(0 - value);
            }
            else
            {
                // format LABEL
                parsedOpcode.OpcodeWord = 0x0000;
                parsedOpcode.HasImmediateWord = true;
                parsedOpcode.LabelName = param;
                parsedOpcode.AddressingMode = AddressingMode.Immediate;
                return true;
            }

            // unless the parameter is a LABEL, parameter parsing will end with this code.
            // if literal value has not been set, then we were unable to parse it. fail!
            if (!literalValue.HasValue)
            {
                return false;
            }
            else
            {
                parsedOpcode.AddressingMode = AddressingMode.Immediate;
                parsedOpcode.HasImmediateWord = true;
                parsedOpcode.ImmediateWord = literalValue.Value;
                return true;
            }
        }

        public bool CanDecodeLiteral(string param)
        {
            ParsedOpcode opcode = new ParsedOpcode();
            bool success = TryParseLiteralParameter(opcode, param);
            return success;
        }

        bool isParamBracketed(string param)
        {
            if ((param.StartsWith("[") && param.EndsWith("]")) || (param.StartsWith("(") && param.EndsWith(")")))
                return true;
            else
                return false;
        }

        bool isStackOffset(string param)
        {
            if (param[0].ToString().ToUpperInvariant() == "S" && 
                param.Length >= 4 && param.IndexOf('[') == 1 && 
                param.IndexOf(']') == param.Length - 1)
                return true;
            else
                return false;
        }

        string removeBrackets(string param)
        {
            return param.Substring(1, param.Length - 2);
        }
    }
}
