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
        public Param ParseParam(string originalParam)
        {
            if (originalParam == null)
                return null;

            Param parsed = new Param();
            bool useExtraDataSegment = false;

            // get rid of ALL white space!
            string param = originalParam.Replace(" ", string.Empty).Trim().ToLowerInvariant();


            // explicit reference to data segment?
            if (param.Length > 3 && param.Substring(0, 3) == "ds[")
            {
                useExtraDataSegment = false;
                param = param.Substring(2);
            }

            // explicit reference to extra segment?
            if (param.Length > 3 && param.Substring(0, 3) == "es[")
            {
                useExtraDataSegment = true;
                param = param.Substring(2);
            }

            if (m_Registers.ContainsKey(param))
            {
                // Register: R0
                parsed.RegisterIndex = m_Registers[param];
                parsed.AddressingMode = AddressingMode.Register;
            }
            else if (m_ControlRegisters.ContainsKey(param))
            {
                // Processor Register: Pc
                parsed.RegisterIndex = m_ControlRegisters[param];
                parsed.AddressingMode = AddressingMode.ControlRegister;
            }
            else if (m_SegmentRegisters.ContainsKey(param))
            {
                // segment Register: cs,ds,es,ss,css,dss,ess,sss,csu,dsu,esu,ssu,is
                parsed.RegisterIndex = m_SegmentRegisters[param];
                parsed.AddressingMode = AddressingMode.SegmentRegister;
            }
            else if (isParamBracketed(param))
            {
                param = removeBrackets(param);

                if (m_Registers.ContainsKey(param))
                {
                    // Indirect: [R0]
                    parsed.RegisterIndex = m_Registers[param];
                    parsed.AddressingMode = AddressingMode.Indirect;
                }
                else if (param.IndexOf(',') != -1)
                {
                    // Indexed; can be [R0,$0000], [$0000,R0], or [R0,R1].
                    string param0 = param.Substring(0, param.IndexOf(',')).Trim(); // base operand
                    string param1 = param.Substring(param.IndexOf(',') + 1, param.Length - param.IndexOf(',') - 1).Trim(); // index operand

                    if (m_Registers.ContainsKey(param0) && m_Registers.ContainsKey(param1))
                    {
                        // Register is both base and index: [R0,R1].
                        ushort reg_a = m_Registers[param0];
                        ushort reg_b = m_Registers[param1];
                        if (reg_a < 4 && reg_b < 4)
                            throw new Exception("With indirect offset addressing, at least one register must be r4-r7");
                        if ((reg_b < reg_a) && (reg_b < 4))
                        {
                            ushort tmp = reg_a;
                            reg_a = reg_b;
                            reg_b = tmp;
                        }
                        reg_b -= 4; // 0 = r4, 1 = r5, 2 = r6, 3 = r7
                        parsed.RegisterIndex = (ushort)(reg_a | (reg_b << 8));
                        parsed.AddressingMode = AddressingMode.IndirectIndexed;
                    }
                    else if (m_Registers.ContainsKey(param0) && CanDecodeLiteral(param1))
                    {
                        // Value base, Register index: [R0,$0000]
                        TryParseLiteralParameter(parsed, param1);
                        parsed.RegisterIndex = m_Registers[param0];
                        parsed.AddressingMode = AddressingMode.IndirectOffset;
                    }
                    else if (CanDecodeLiteral(param0) && m_Registers.ContainsKey(param1))
                    {
                        // Value base, Register index: [$0000,R0]
                        TryParseLiteralParameter(parsed, param0);
                        parsed.RegisterIndex = m_Registers[param1];
                        parsed.AddressingMode = AddressingMode.IndirectOffset;
                    }
                    else
                    {
                        // indexed register is invalid.
                        throw new Exception($"Invalid operand '{originalParam}'");
                    }
                }
                else if (CanDecodeLiteral(param))
                {
                    // Absolute (literal indirect): [$0000]
                    TryParseLiteralParameter(parsed, param);
                    parsed.AddressingMode = AddressingMode.Absolute;
                }
                else
                {
                    // invlid bracketed operand.
                    throw new Exception($"Invalid operand '{originalParam}'");
                }
            }
            else if (CanDecodeLiteral(param))
            {
                // Literal: $0000
                TryParseLiteralParameter(parsed, param);
            }
            else
            {
                // what is this?! not an acceptable operand, that's for certain!
                throw new Exception($"Invalid operand '{originalParam}'");
            }

            if (useExtraDataSegment)
            {
                switch (parsed.AddressingMode)
                {
                    case AddressingMode.Absolute:
                    case AddressingMode.Indirect:
                    case AddressingMode.IndirectIndexed:
                    case AddressingMode.IndirectOffset:
                        parsed.UsesExtraDataSegment = true;
                        break;
                    default:
                        throw new Exception(
                            $"Cannot parse {originalParam}: use of extended segment not allowed with this addressing mode");
                }
            }

            return parsed;
        }

        private bool TryParseLiteralParameter(Param parsedOpcode, string originalParam)
        {
            ushort? literalValue = null;
            ushort value;

            uint? literalBig = null;
            uint big;

            string param = originalParam;

            if (param.Contains("$")) // allow both $ and 0x as indicators of hex numbers. other formats as well?
                param = param.Replace("$", "0x");

            if (param.Contains("0x"))
            {
                // format: 0x12EF or -0x12EF or 0x12345678 or -0x12345678
                if (param.IndexOf('-') == 0)
                {
                    // negative number
                    try
                    {
                        literalValue = (ushort)(0 - Convert.ToInt16(param.Substring(1, param.Length - 1), 16));
                    }
                    catch
                    {
                        try
                        {
                            literalBig = (uint)(0 - Convert.ToInt32(param.Substring(1, param.Length - 1), 16));
                        }
                        catch
                        {
                            throw new Exception($"Could not parse this hexidecimal parameter: '{originalParam}'");
                        }
                    }
                }
                else
                {
                    // positive number
                    try
                    {
                        literalValue = Convert.ToUInt16(param, 16);
                    }
                    catch
                    {
                        try
                        {
                            literalBig = Convert.ToUInt32(param, 16);
                        }
                        catch
                        {
                            throw new Exception($"Could not parse this hexidecimal parameter: '{originalParam}'");
                        }
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
            else if (uint.TryParse(param, out big))
            {
                // format 12345678
                literalBig = value;
            }
            else if ((param.IndexOf('-') == 0) && (uint.TryParse(param.Substring(1), out big)))
            {
                // format -12345678
                literalBig = (ushort)(0 - value);
            }
            else
            {
                // format LABEL
                parsedOpcode.RegisterIndex = 0x0000;
                parsedOpcode.Label = param;
                parsedOpcode.AddressingMode = AddressingMode.Immediate;
                return true;
            }

            // unless the parameter is a LABEL, parameter parsing will end with this code.
            // if literal value has not been set, then we were unable to parse it. fail!
            if (!literalValue.HasValue && !literalBig.HasValue)
            {
                return false;
            }
            if (literalBig.HasValue)
            {
                parsedOpcode.AddressingMode = AddressingMode.ImmediateBig;
                parsedOpcode.ImmediateWordLong = literalBig.Value;
                return true;
            }
            parsedOpcode.AddressingMode = AddressingMode.Immediate;
            parsedOpcode.ImmediateWordShort = literalValue.Value;
            return true;
        }

        public bool CanDecodeLiteral(string param)
        {
            Param opcode = new Param();
            bool success = TryParseLiteralParameter(opcode, param);
            return success;
        }

        private bool isParamBracketed(string param)
        {
            if ((param.StartsWith("[") && param.EndsWith("]")) || (param.StartsWith("(") && param.EndsWith(")")))
                return true;
            return false;
        }

        private string removeBrackets(string param)
        {
            return param.Substring(1, param.Length - 2);
        }
    }
}
