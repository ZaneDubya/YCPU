﻿using System;

namespace YCPU.Hardware
{
    partial class YCPU
    {
        public string[] Disassemble(ushort memory, int begin, int count)
        {
            string[] s = new string[count];
            ushort m = memory;
            ushort word = ReadMemInt16((ushort)((m - 2)));
            bool badOpcodeIgnored = false;
            if (Opcodes[word & 0x00FF].UsesNextWord(word))
            {
                // bad opcode immediately prior to this one. ignore it.
                m -= 4;
                badOpcodeIgnored = true;
            }
            else
            {
                // opcode immediately prior is good. use it.
                m -= 2;
                word = ReadMemInt16((ushort)(m - 2));
                /*if (Opcodes[word & 0x00FF].UsesNextWord(word))
                {
                    m -= 2;
                    badOpcodeIgnored = true;
                }*/
            }

            for (int i = -1; i > begin; i -= 1)
            {
                // check the memory word before this one. if it is an opcode that
                // uses an extra word, then the current word is that extra word.
                // Otherwise, the current memory position is it's own opcode.
                word = ReadMemInt16((ushort)(m - 4));
                if (Opcodes[word & 0x00FF].UsesNextWord(word))
                {
                    m -= 4;
                }
                else
                {
                    m -= 2;
                }
            }


            for (int i = 0; i < count; i += 1)
            {
                if (badOpcodeIgnored && (m == (ushort)(memory - 2)))
                    m += 2;
                word = ReadMemInt16(m);
                ushort nextword = ReadMemInt16((ushort)(m + 2));
                bool uses_next_word = false;

                YCPUInstruction opcode = Opcodes[word & 0x00FF];
                s[i] = string.Format(
                    "{0:X4}:{1:X4} {2}",
                    m, word, (opcode.Disassembler != null) ?
                    opcode.Disassembler(opcode.Name, word, nextword, m, out uses_next_word) :
                    opcode.Name);
                m += (ushort)(uses_next_word ? 4 : 2);
            }
            return s;
        }

        private string DisassembleALU(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            int addressingmode = (operand & 0x0F00) >> 8;
            RegGPIndex regDest = (RegGPIndex)((operand & 0x0007));
            RegGPIndex regSrc = (RegGPIndex)((operand & 0xE000) >> 13);
            int index_bits = ((operand & 0x0700) >> 8);

            switch (addressingmode)
            {
                case 0: // Immediate
                case 1: // Absolute
                    bool absolute = (operand & 0x0100) != 0;
                    if (name == "STO" && absolute == false)
                    {
                        uses_next_word = false;
                        return "NOP";
                    }
                    else
                    {
                        uses_next_word = true;
                        return string.Format("{0,-8}{1}, {3}${2:X4}{4}", name, NameOfRegGP(regDest),
                            nextword, absolute ? "[" : string.Empty, absolute ? "]" : string.Empty);
                    }
                case 2: // Register
                    uses_next_word = false;
                    return string.Format("{0,-8}{1}, {2}          (${3:X4})", name, NameOfRegGP(regDest),
                        NameOfRegGP((RegGPIndex)((int)regSrc)),
                        R[(int)regSrc]);
                case 3: // Indirect
                    uses_next_word = false;
                    return string.Format("{0,-8}{1}, [{2}]        (${3:X4})", name, NameOfRegGP(regDest),
                        NameOfRegGP((RegGPIndex)((int)regSrc)),
                        ReadMemInt16(R[(int)regSrc]));
                case 4: // Indirect Offset (also Absolute Offset)
                    uses_next_word = true;
                    return string.Format("{0,-8}{1}, [{2},${3:X4}]  (${4:X4})", name, NameOfRegGP(regDest),
                        NameOfRegGP((RegGPIndex)((int)regSrc)), nextword,
                        ReadMemInt16((ushort)(R[(int)regSrc] + nextword)));
                case 5:
                    uses_next_word = true;
                    return string.Format("{0,-8}{1}, S[${2:X2}]  (${3:X4})", name, NameOfRegGP(regDest),  index_bits,
                        ReadMemInt16((ushort)(R[(int)regSrc] + nextword)));
                case 6: // Indirect PostInc
                    uses_next_word = false;
                    return string.Format("{0,-8}{1}, [{2}+]       (${3:X4})", name, NameOfRegGP(regDest),
                        NameOfRegGP((RegGPIndex)((int)regSrc)),
                        ReadMemInt16(R[(int)regSrc]));
                case 7: // Indirect PreDec
                    uses_next_word = false;
                    return string.Format("{0,-8}{1}, [-{2}]       (${3:X4})", name, NameOfRegGP(regDest),
                        NameOfRegGP((RegGPIndex)((int)regSrc)),
                        ReadMemInt16(R[(int)regSrc]));
                default: // $8 - $F are Indirect Indexed
                    uses_next_word = false;
                    return string.Format("{0,-8}{1}, [{2},{3}]     (${4:X4})", name, NameOfRegGP(regDest),
                        NameOfRegGP((RegGPIndex)((int)regSrc)),
                        NameOfRegGP((RegGPIndex)index_bits),
                        ReadMemInt16((ushort)
                            (R[(int)regSrc] + R[index_bits])));
            }
        }

        private string DisassembleBIT(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            RegGPIndex destination = (RegGPIndex)((operand & 0xE000) >> 13);
            bool as_register = ((operand & 0x0100) != 0);
            ushort value = (as_register) ?
                (ushort)(R[((operand & 0x1C00) >> 10)] & 0x000F) :
                (ushort)((operand & 0x1E00) >> 9);
            return string.Format("{0,-8}{1}, {2}", name, NameOfRegGP(destination), as_register ?
                string.Format("{0,-8}(${1:X1})", NameOfRegGP(destination), value) :
                string.Format("${0:X1}", value));
        }

        private string DisassembleBRA(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            sbyte value = (sbyte)((operand & 0xFF00) >> 8);
            return string.Format("{0,-8}{3}{1:000}            (${2:X4})", name, value, (ushort)(address + value), (value & 0x80) == 0 ? "+" : string.Empty);
        }

        private string DisassembleFLG(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            string flags = string.Format("{0}{1}{2}{3}",
                ((operand & 0x8000) != 0) ? "N " : string.Empty,
                ((operand & 0x4000) != 0) ? "Z " : string.Empty,
                ((operand & 0x2000) != 0) ? "C " : string.Empty,
                ((operand & 0x1000) != 0) ? "V" : string.Empty);
            if (flags == string.Empty)
                flags = "<NONE>";
            return string.Format("{0,-8}{1}", name, flags);
        }

        private string DisassembleFPU(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            RegGPIndex destination;
            ushort source;
            BitPatternFPU(operand, out source, out destination);
            int operation = (operand & 0x0300) >> 8;
            string op_name = string.Empty;

            switch (operation)
            {
                case 0x00:
                    op_name = "FPA";
                    break;
                case 0x01:
                    op_name = "FPS";
                    break;
                case 0x02:
                    op_name = "FPM";
                    break;
                case 0x03:
                    op_name = "FPD";
                    break;
                default:
                    return "ERR";
            }

            float[] operands = FPU_GetOperands(destination, (RegGPIndex)source);

            return string.Format("{0,-8}[{1}], [{2}]  {3:E3},{4:E3}", op_name, NameOfRegGP(destination), NameOfRegGP((RegGPIndex)source), operands[0], operands[1]);
        }

        private string DisassembleHWQ(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            RegGPIndex unused;
            ushort value;
            BitPatternHWQ(operand, out value, out unused);

            return string.Format("{0,-8}${1:X2}", name, value);
        }

        private string DisassembleINC(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            RegGPIndex destination;
            ushort value;
            BitPatternINC(operand, out value, out destination);

            return string.Format("{0,-8}{1}, ${2:X2}", name, NameOfRegGP(destination), value);
        }

        private string DisassembleJMP(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            int addressingmode = ((operand & 0xE000) >> 13);
            RegGPIndex r_src = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);

            switch (addressingmode)
            {
                case 0: // Immediate
                    uses_next_word = true;
                    bool absolute = (operand & 0x0100) != 0;
                    return string.Format("{0,-8}{2}${1:X4}{3}{4}", name, nextword, 
                        absolute ? "[" : string.Empty, absolute ? "]" : string.Empty,
                        absolute ? string.Format("         (${0:X4})", ReadMemInt16(nextword)) : string.Empty);
                case 1: // Register
                    uses_next_word = false;
                    return string.Format("{0,-8}{1}              (${2:X4})", name, 
                        NameOfRegGP((RegGPIndex)((int)r_src)),
                        R[(int)r_src]);
                case 2: // Indirect
                    uses_next_word = false;
                    return string.Format("{0,-8}[{1}]            (${2:X4})", name, 
                        NameOfRegGP((RegGPIndex)((int)r_src)),
                        ReadMemInt16(R[(int)r_src]));
                case 3: // Indirect Offset (also Absolute Offset)
                    uses_next_word = true;
                    return string.Format("{0,-8}[{1},${2:X4}]      (${3:X4})", name, 
                        NameOfRegGP((RegGPIndex)((int)r_src)), nextword,
                        ReadMemInt16((ushort)(R[(int)r_src] + nextword)));
                case 4: // Indirect PostInc
                    uses_next_word = false;
                    return string.Format("{0,-8}[{1}+]           (${2:X4})", name, 
                        NameOfRegGP((RegGPIndex)((int)r_src)),
                        ReadMemInt16(R[(int)r_src]));
                case 5: // Indirect PreDec
                    uses_next_word = false;
                    return string.Format("{0,-8}[-{1}]           (${2:X4})", name, 
                        NameOfRegGP((RegGPIndex)((int)r_src)),
                        ReadMemInt16(R[(int)r_src]));
                case 6: // Indirect Indexed
                    uses_next_word = false;
                    return string.Format("{0,-8}[{1},{2}]         (${3:X4})", name, 
                        NameOfRegGP((RegGPIndex)((int)r_src)),
                        NameOfRegGP((RegGPIndex)index_bits),
                        ReadMemInt16((ushort)
                            (R[(int)r_src] + R[index_bits])));
                case 7:
                    uses_next_word = false;
                    return string.Format("{0,-8}[{1},{2}]         (${3:X4})", name, 
                       NameOfRegGP((RegGPIndex)((int)r_src)),
                       NameOfRegGP((RegGPIndex)(index_bits + 4)),
                       ReadMemInt16((ushort)
                           (R[(int)r_src] + R[index_bits + 4])));
                default:
                    uses_next_word = false;
                    return "ERROR JMP Unsigned Format";
            }
        }

        private string DisassembleMMU(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            RegGPIndex RegMmuIndex;
            ushort RegMmuValue;
            BitPatternMMU(operand, out RegMmuValue, out RegMmuIndex);
            return string.Format("{0,-8}{1}, {2}", name, NameOfRegGP(RegMmuIndex), NameOfRegGP((RegGPIndex)RegMmuValue));
        }

        private string DisassembleNoBits(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            return string.Format(name);
        }

        private string DisassemblePSH(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            string flags = "{0}{1}{2}{3}{4}{5}{6}{7}";
            if ((operand & 0x0001) == 0x0000)
            {
                flags = string.Format(flags,
                    ((operand & 0x8000) != 0) ? "R7 " : string.Empty,
                    ((operand & 0x4000) != 0) ? "R6 " : string.Empty,
                    ((operand & 0x2000) != 0) ? "R5 " : string.Empty,
                    ((operand & 0x1000) != 0) ? "R4 " : string.Empty,
                    ((operand & 0x0800) != 0) ? "R3 " : string.Empty,
                    ((operand & 0x0400) != 0) ? "R2 " : string.Empty,
                    ((operand & 0x0200) != 0) ? "R1 " : string.Empty,
                    ((operand & 0x0100) != 0) ? "R0 " : string.Empty);
            }
            else
            {
                flags = string.Format(flags,
                    ((operand & 0x1000) != 0) ? "FL " : string.Empty,
                    ((operand & 0x0800) != 0) ? "PC " : string.Empty,
                    ((operand & 0x0400) != 0) ? "PS " : string.Empty,
                    ((operand & 0x0200) != 0) ? "USP " : string.Empty,
                    ((operand & 0x0100) != 0) ? "SP " : string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty);
            }
            if (flags == string.Empty)
                flags = "<NONE>";
            if (name.ToLower() == "pop" && (flags.Trim() == "PC"))
                return "RTS";
            return string.Format("{0,-8}{1}", name, flags);
        }

        private string DisassembleSHF(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            RegGPIndex destination = (RegGPIndex)((operand & 0xE000) >> 13);
            string value = string.Empty;
            if ((operand & 0x1000) == 0)
            {
                int shiftby = ((operand & 0x0F00) >> 8) + 1;
                value = string.Format("${0:X2}", (ushort)shiftby);
            }
            else
                value = NameOfRegGP((RegGPIndex)((operand & 0x0700) >> 8));
            return string.Format("{0,-8}{1}, {2}", name, NameOfRegGP(destination), value);
        }

        private string DisassembleSWO(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            RegGPIndex destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C) >> 10);
            int operation = (operand & 0x0300) >> 8;
            string regD = string.Empty, regS = string.Empty;

            switch (operation)
            {
                case 0x00: // src low octet -> dest low octet, clear dest upper 8 bits.
                    regD = NameOfRegGP(destination) + ".L";
                    regS = NameOfRegGP(source) + ".L";
                    break;
                case 0x01: // src high octect -> dest low octet, clear dest upper 8 bits.
                    regD = NameOfRegGP(destination) + ".L";
                    regS = NameOfRegGP(source) + ".H";
                    break;
                case 0x02: // src low octect -> dest low octet, preserves dest upper 8 bits.
                    regD = NameOfRegGP(destination) + ".LP";
                    regS = NameOfRegGP(source) + ".L";
                    break;
                case 0x03: // src low octect -> dest high octect, preserves dest lower 8 bits.
                    regD = NameOfRegGP(destination) + ".HP";
                    regS = NameOfRegGP(source) + ".L";
                    break;
                default:
                    return "ERR";
            }

            return string.Format("{0,-8}{1}, {2}", name, regD, regS);
        }

        private string DisassembleTSR(string name, ushort operand, ushort nextword, ushort address, out bool uses_next_word)
        {
            uses_next_word = false;
            RegGPIndex destination;
            ushort value;
            BitPatternTSR(operand, out value, out destination);
            return string.Format("{0,-8}{1}, ${2:X2}", name, NameOfRegGP(destination), value);
        }

        private string NameOfRegGP(RegGPIndex register)
        {
            switch (register)
            {
                case RegGPIndex.R0:
                    return "R0";
                case RegGPIndex.R1:
                    return "R1";
                case RegGPIndex.R2:
                    return "R2";
                case RegGPIndex.R3:
                    return "R3";
                case RegGPIndex.R4:
                    return "R4";
                case RegGPIndex.R5:
                    return "R5";
                case RegGPIndex.R6:
                    return "R6";
                case RegGPIndex.R7:
                    return "R7";
                default:
                    return "??";
            }
        }
    }
}
