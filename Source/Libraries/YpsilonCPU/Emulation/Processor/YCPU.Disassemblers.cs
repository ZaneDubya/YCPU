namespace Ypsilon.Emulation.Processor {
    partial class YCPU {
        public string[] Disassemble(ushort address, int count, bool extendedFormat = true) {
            string[] s = new string[count];
            ushort word;
            for (int i = 0; i < count; i += 1) {
                word = DebugReadMemory(address, SegmentIndex.CS);
                ushort nextword = DebugReadMemory((ushort)(address + 2), SegmentIndex.CS);
                ushort instructionSize = 2;
                YCPUInstruction opcode = m_Opcodes[word & 0x00FF];
                if (extendedFormat) {
                    s[i] =
                        $"{address:X4}:{word:X4} {(opcode.Disassembler != null ? opcode.Disassembler(opcode.Name, word, nextword, address, true, out instructionSize) : opcode.Name)}";
                }
                else {
                    s[i] = opcode.Disassembler != null ?
                        opcode.Disassembler(opcode.Name, word, nextword, address, false, out instructionSize).ToLowerInvariant() :
                        opcode.Name;
                }
                address += instructionSize;
            }
            return s;
        }

        private string AppendMemoryContents(string disasm, ushort mem) {
            int len = disasm.Length;
            disasm = $"{disasm}{new string(' ', 24 - len)}(${mem:X4})";
            return disasm;
        }

        private string DisassembleALU(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            int addressingmode = (operand & 0x7000) >> 12;
            RegGeneral regDest = (RegGeneral)(operand & 0x0007);
            RegGeneral regSrc = (RegGeneral)((operand & 0x0E00) >> 9);
            bool isEightBit = (operand & 0x0100) != 0;
            SegmentIndex segData = (operand & 0x8000) != 0 ? SegmentIndex.ES : SegmentIndex.DS;
            switch (addressingmode) {
                case 0:
                    if (regSrc == 0) // immediate
                    {
                        if (name == "STO") {
                            instructionSize = 2;
                            return "???";
                        }
                        instructionSize = 4;
                        string disasm =
                            $"{name + (isEightBit ? ".8" : string.Empty),-8}{NameOfRegGP(regDest)}, ${nextword:X4}";
                        if (showMemoryContents)
                            disasm = AppendMemoryContents(disasm, nextword);
                        return disasm;
                    }
                    if ((int)regSrc == 1) // absolute
                    {
                        instructionSize = 4;
                        string disasm =
                            $"{name + (isEightBit ? ".8" : string.Empty),-8}{NameOfRegGP(regDest)}, [${nextword:X4}]";
                        if (showMemoryContents)
                            disasm = AppendMemoryContents(disasm, DebugReadMemory(nextword, segData));
                        return disasm;
                    }
                    else // control register
                    {
                        instructionSize = 2;
                        RegControl cr = (RegControl)((operand & 0x0700) >> 8);
                        string disasm = $"{name,-8}{NameOfRegGP(regDest)}, {NameOfRegSP(cr)}";
                        if (showMemoryContents)
                            disasm = AppendMemoryContents(disasm, nextword);
                        return disasm;
                    }
                case 1: // Register
                    instructionSize = 2;
                    return
                        $"{name + (isEightBit ? ".8" : string.Empty),-8}{NameOfRegGP(regDest)}, {NameOfRegGP(regSrc),-12}(${R[(int)regSrc]:X4})";
                case 2: // Indirect
                    instructionSize = 2;
                    return
                        $"{name + (isEightBit ? ".8" : string.Empty),-8}{NameOfRegGP(regDest)}, [{NameOfRegGP(regSrc)}]        (${DebugReadMemory(R[(int)regSrc], segData):X4})";
                case 3: // Indirect Offset (also Absolute Offset)
                    instructionSize = 4;
                    return
                        $"{name + (isEightBit ? ".8" : string.Empty),-8}{NameOfRegGP(regDest)}, [{NameOfRegGP(regSrc)},${nextword:X4}]  (${DebugReadMemory((ushort)(R[(int)regSrc] + nextword), segData):X4})";
                default: // $4 - $7 are Indirect Indexed
                    instructionSize = 2;
                    RegGeneral regIndex = (RegGeneral)((operand & 0x7000) >> 12);
                    return
                        $"{name + (isEightBit ? ".8" : string.Empty),-8}{NameOfRegGP(regDest)}, [{NameOfRegGP(regSrc)},{NameOfRegGP(regIndex)}]     (${DebugReadMemory((ushort)(R[(int)regSrc] + R[(int)regIndex]), segData):X4})";
            }
        }

        private string DisassembleBRA(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            instructionSize = 2;
            sbyte value = (sbyte)((operand & 0xFF00) >> 8);
            return string.Format("{0,-8}{3}{1:000}            (${2:X4})", name, value, (ushort)(address + value), (value & 0x80) == 0 ? "+" : string.Empty);
        }

        private string DisassembleBTT(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            instructionSize = 2;
            RegGeneral destination = (RegGeneral)((operand & 0xE000) >> 13);
            bool as_register = (operand & 0x1000) != 0;
            ushort value = as_register ?
                (ushort)((operand & 0x0700) >> 8) :
                (ushort)((operand & 0x0F00) >> 8);
            return
                $"{name,-8}{NameOfRegGP(destination)}, {(as_register ? $"{NameOfRegGP((RegGeneral)value),-8}(${R[value]:X1})" : $"${value:X1}")}";
        }

        private string DisassembleFLG(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            instructionSize = 2;
            string flags =
                $"{((operand & 0x8000) != 0 ? "N " : string.Empty)}{((operand & 0x4000) != 0 ? "Z " : string.Empty)}{((operand & 0x2000) != 0 ? "C " : string.Empty)}{((operand & 0x1000) != 0 ? "V" : string.Empty)}";
            if (flags == string.Empty)
                flags = "<NONE>";
            return $"{name,-8}{flags}";
        }

        private string DisassembleHWQ(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            instructionSize = 2;
            RegGeneral unused;
            ushort value;
            BitPatternHWQ(operand, out value, out unused);
            return $"{name,-8}${value:X2}";
        }

        private string DisassembleINC(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            instructionSize = 2;
            RegGeneral destination;
            ushort value;
            BitPatternIMM(operand, out value, out destination);
            return $"{name,-8}{NameOfRegGP(destination)}, ${value:X2}";
        }

        private string DisassembleJMP(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            int addressingmode = (operand & 0x7000) >> 12;
            RegGeneral r_src = (RegGeneral)((operand & 0x1C00) >> 10);
            int index_bits = (operand & 0x0300) >> 8;
            SegmentIndex segData = (operand & 0x8000) != 0 ? SegmentIndex.ES : SegmentIndex.DS;
            bool isFar = (operand & 0x0100) != 0;
            if (isFar) {
                name += ".F";
            }
            switch (addressingmode) {
                case 0: // Immediate or Absolute
                    bool absolute = (operand & 0x0200) != 0;
                    instructionSize = (ushort)(isFar && !absolute ? 8 : 4);
                    if (absolute) {
                        return
                            $"{name,-8}[${nextword:X4}]{string.Format($"         (${DebugReadMemory(nextword, SegmentIndex.CS):X4})", DebugReadMemory(nextword, SegmentIndex.CS))}";
                    }
                    return $"{name,-8}${nextword:X4}{(isFar ? ", $<SEGREG>" : string.Empty)}";
                case 1: // Register
                    instructionSize = 2;
                    return $"{name,-8}{NameOfRegGP(r_src)}              (${R[(int)r_src]:X4})";
                case 2: // Indirect
                    instructionSize = 2;
                    return
                        $"{name,-8}[{NameOfRegGP(r_src)}]            (${DebugReadMemory(R[(int)r_src], segData):X4})";
                case 3: // Indirect Offset (also Absolute Offset)
                    instructionSize = 4;
                    return
                        $"{name,-8}[{NameOfRegGP(r_src)},${nextword:X4}]      (${DebugReadMemory((ushort)(R[(int)r_src] + nextword), segData):X4})";
                default: // $8 - $f = Indirect Indexed
                    instructionSize = 2;
                    index_bits += (operand & 0x4000) >> 12;
                    return
                        $"{name,-8}[{NameOfRegGP(r_src)},{NameOfRegGP((RegGeneral)index_bits)}]         (${DebugReadMemory((ushort)(R[(int)r_src] + R[index_bits]), segData):X4})";
            }
        }

        private string DisassembleNoBits(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            instructionSize = 2;
            return string.Format(name);
        }

        private string DisassemblePRX(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            int operation_index = (operand & 0xff00) >> 8;
            instructionSize = 2;
            switch (operation_index) {
                case 0: // RTS
                    return "RTS";
                case 1: // RTS.F
                    return "RTS.F";
                case 2: // RTI
                    return "RTI";
                case 3: // SWI
                    return "SWI";
                case 4: // SLP
                    return "SLP";
                default:
                    return "???";
            }
        }

        private string DisassembleSET(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            instructionSize = 2;
            RegGeneral destination = (RegGeneral)((operand & 0xE000) >> 13);
            int value = (operand & 0x1F00) >> 8;
            if ((operand & 0x0001) == 1) {
                if (value <= 0x0A)
                    value = (ushort)(0x0001 << (value + 0x05));
                else
                    value = (ushort)(0xFFE0 + value);
            }
            return $"{name,-8}{NameOfRegGP(destination)}, {string.Format($"${value:X2}", value)}";
        }

        private string DisassembleSHF(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            instructionSize = 2;
            RegGeneral destination = (RegGeneral)((operand & 0xE000) >> 13);
            string value;
            if ((operand & 0x1000) == 0) {
                int shiftby = ((operand & 0x0F00) >> 8) + 1;
                value = $"${(ushort)shiftby:X2}";
            }
            else
                value = NameOfRegGP((RegGeneral)((operand & 0x0700) >> 8));
            return $"{name,-8}{NameOfRegGP(destination)}, {value}";
        }

        private string DisassembleSTK(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            instructionSize = 2;
            string flags = "{0}{1}{2}{3}{4}{5}{6}{7}";
            if ((operand & 0x0001) == 0x0000) {
                flags = string.Format(flags,
                    (operand & 0x8000) != 0 ? "R7 " : string.Empty,
                    (operand & 0x4000) != 0 ? "R6 " : string.Empty,
                    (operand & 0x2000) != 0 ? "R5 " : string.Empty,
                    (operand & 0x1000) != 0 ? "R4 " : string.Empty,
                    (operand & 0x0800) != 0 ? "R3 " : string.Empty,
                    (operand & 0x0400) != 0 ? "R2 " : string.Empty,
                    (operand & 0x0200) != 0 ? "R1 " : string.Empty,
                    (operand & 0x0100) != 0 ? "R0 " : string.Empty);
            }
            else {
                flags = string.Format(flags,
                    (operand & 0x8000) != 0 ? "SP " : string.Empty,
                    (operand & 0x4000) != 0 ? "USP " : string.Empty,
                    (operand & 0x2000) != 0 ? "?? " : string.Empty,
                    (operand & 0x1000) != 0 ? "?? " : string.Empty,
                    (operand & 0x0800) != 0 ? "?? " : string.Empty,
                    (operand & 0x0400) != 0 ? "PS " : string.Empty,
                    (operand & 0x0200) != 0 ? "PC " : string.Empty,
                    (operand & 0x0100) != 0 ? "FL " : string.Empty);
            }
            if (flags == string.Empty)
                flags = "<NONE>";
            return $"{name,-8}{flags}";
        }

        private string DisassembleSTX(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            instructionSize = 2;
            int sp_delta = (sbyte)((operand & 0xff00) >> 8);
            return $"{name,-8}{(sp_delta >= 0 ? "+" : string.Empty)}{sp_delta}";
        }

        private string DisassembleXSG(string name, ushort operand, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize) {
            instructionSize = 2;
            bool push = (operand & 0x0100) != 0;
            int register = (operand & 0x0E00) >> 9;
            bool user = (operand & 0x8000) != 0;
            name = push ? "SSG" : "LSG";
            string reg;
            switch ((SegmentIndex)register) {
                case SegmentIndex.CS:
                    reg = user ? "CSU" : "CSS";
                    break;
                case SegmentIndex.DS:
                    reg = user ? "DSU" : "DSS";
                    break;
                case SegmentIndex.ES:
                    reg = user ? "ESU" : "ESS";
                    break;
                case SegmentIndex.SS:
                    reg = user ? "SSU" : "SSS";
                    break;
                case SegmentIndex.IS:
                    reg = user ? "???" : "IS";
                    break;
                default:
                    // operand does not include a valid segment index.
                    reg = "???";
                    break;
            }
            return $"{name,-8}{reg}";
        }

        private string NameOfRegGP(RegGeneral register) {
            switch (register) {
                case RegGeneral.R0:
                    return "R0";
                case RegGeneral.R1:
                    return "R1";
                case RegGeneral.R2:
                    return "R2";
                case RegGeneral.R3:
                    return "R3";
                case RegGeneral.R4:
                    return "R4";
                case RegGeneral.R5:
                    return "R5";
                case RegGeneral.R6:
                    return "R6";
                case RegGeneral.R7:
                    return "R7";
                default:
                    return "??";
            }
        }

        private string NameOfRegSP(RegControl register) {
            switch (register) {
                case RegControl.FL:
                    return "FL";
                case RegControl.PC:
                    return "PC";
                case RegControl.PS:
                    return "PS";
                case RegControl.USP:
                    return "USP";
                case RegControl.SSP:
                    return "SP";
                default:
                    return "??";
            }
        }
    }
}