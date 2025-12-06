using System;

namespace Ypsilon.Emulation.Processor {
    partial class YCPU {
        public delegate string YCPUDisassembler(string name, ushort opcode, ushort nextword, ushort address, bool showMemoryContents, out ushort instructionSize);

        public delegate void YCPUOpcode(ushort opcode);

        public enum BitWidth {
            Int8 = 0,
            Int16 = 1
        }

        private readonly YCPUInstruction[] m_Opcodes = new YCPUInstruction[0x100];

        public struct YCPUInstruction {
            public readonly string Name;
            public readonly YCPUOpcode Opcode;
            public readonly YCPUDisassembler Disassembler;
            public readonly int Cycles;
            public bool IsNOP;

            public YCPUInstruction(string name, YCPUOpcode opcode, YCPUDisassembler disassembler, int cycles, bool isNOP = false) {
                Name = name;
                Opcode = opcode;
                Disassembler = disassembler;
                Cycles = cycles;
                IsNOP = isNOP;
            }

            public override string ToString() {
                return Name;
            }
        }

        #region HWQ & SLP

        private void HWQ(ushort operand) {
            if (!PS_S) {
                Interrupt_UnPrivFault(operand);
                return;
            }
            ushort query_type;
            RegGeneral unused;
            BitPatternHWQ(operand, out query_type, out unused);
            switch (query_type) {
                case 0x00: // Query number of devices connected
                    R[(int)RegGeneral.R0] = BUS.DevicesConnected;
                    break;
                case 0x01: // Query device attached to bus in slot R0
                    ushort[] device_info = BUS.QueryDevice(R[(int)RegGeneral.R0]);
                    R[(int)RegGeneral.R0] = device_info[0];
                    R[(int)RegGeneral.R1] = device_info[1];
                    R[(int)RegGeneral.R2] = device_info[2];
                    R[(int)RegGeneral.R3] = device_info[3];
                    break;
                case 0x02: // Send message to hardware device
                    R[0] = BUS.SendDeviceMessage(R[(int)RegGeneral.R0],
                        R[(int)RegGeneral.R1],
                        R[(int)RegGeneral.R2]);
                    break;
                case 0x03: // Get RAM/ROM amounts in R0-R3.
                    R[0] = (ushort)BUS.RAMSize;
                    R[1] = (ushort)(BUS.RAMSize >> 16);
                    R[2] = (ushort)BUS.ROMSize;
                    R[3] = (ushort)(BUS.ROMSize >> 16);
                    break;
                case 0x80: // Get RTC time
                    ushort[] rtc_data = m_RTC.GetData();
                    R[(int)RegGeneral.R0] = rtc_data[0];
                    R[(int)RegGeneral.R1] = rtc_data[1];
                    R[(int)RegGeneral.R2] = rtc_data[2];
                    R[(int)RegGeneral.R3] = rtc_data[3];
                    break;
                case 0x82:
                    R[(int)RegGeneral.R0] = m_RTC.GetTickRate();
                    break;
                case 0x83:
                    R[(int)RegGeneral.R0] = m_RTC.SetTickRate(R[(int)RegGeneral.R0], Cycles);
                    break;
                default:
                    Interrupt_UndefFault(operand);
                    break;
            }
        }

        #endregion

        #region OpCode Initialization

        private void InitializeOpcodes() {
            // Specification at 3:
            // All instructions are comprised of single 16 - bit program words. Some instructions
            // may be suffixed by a single 16 - bit immediate value. One instruction -the far
            // jump immediate instruction - is suffixed by three 16 - bit immediate values.
            // All instructions are defined by the 8-bit low octet of the 16-bit program word.
            // Attempted execution of a program word that does not have a defined 8 - bit low
            // octet will raise the 'undefined' interrupt.
            m_Opcodes[0x00] = new YCPUInstruction("CMP", CMP, DisassembleALU, 0);
            m_Opcodes[0x01] = new YCPUInstruction("CMP", CMP, DisassembleALU, 0);
            m_Opcodes[0x02] = new YCPUInstruction("CMP", CMP, DisassembleALU, 0);
            m_Opcodes[0x03] = new YCPUInstruction("CMP", CMP, DisassembleALU, 0);
            m_Opcodes[0x04] = new YCPUInstruction("CMP", CMP, DisassembleALU, 0);
            m_Opcodes[0x05] = new YCPUInstruction("CMP", CMP, DisassembleALU, 0);
            m_Opcodes[0x06] = new YCPUInstruction("CMP", CMP, DisassembleALU, 0);
            m_Opcodes[0x07] = new YCPUInstruction("CMP", CMP, DisassembleALU, 0);
            m_Opcodes[0x08] = new YCPUInstruction("NEG", NEG, DisassembleALU, 0);
            m_Opcodes[0x09] = new YCPUInstruction("NEG", NEG, DisassembleALU, 0);
            m_Opcodes[0x0A] = new YCPUInstruction("NEG", NEG, DisassembleALU, 0);
            m_Opcodes[0x0B] = new YCPUInstruction("NEG", NEG, DisassembleALU, 0);
            m_Opcodes[0x0C] = new YCPUInstruction("NEG", NEG, DisassembleALU, 0);
            m_Opcodes[0x0D] = new YCPUInstruction("NEG", NEG, DisassembleALU, 0);
            m_Opcodes[0x0E] = new YCPUInstruction("NEG", NEG, DisassembleALU, 0);
            m_Opcodes[0x0F] = new YCPUInstruction("NEG", NEG, DisassembleALU, 0);
            m_Opcodes[0x10] = new YCPUInstruction("ADD", ADD, DisassembleALU, 0);
            m_Opcodes[0x11] = new YCPUInstruction("ADD", ADD, DisassembleALU, 0);
            m_Opcodes[0x12] = new YCPUInstruction("ADD", ADD, DisassembleALU, 0);
            m_Opcodes[0x13] = new YCPUInstruction("ADD", ADD, DisassembleALU, 0);
            m_Opcodes[0x14] = new YCPUInstruction("ADD", ADD, DisassembleALU, 0);
            m_Opcodes[0x15] = new YCPUInstruction("ADD", ADD, DisassembleALU, 0);
            m_Opcodes[0x16] = new YCPUInstruction("ADD", ADD, DisassembleALU, 0);
            m_Opcodes[0x17] = new YCPUInstruction("ADD", ADD, DisassembleALU, 0);
            m_Opcodes[0x18] = new YCPUInstruction("SUB", SUB, DisassembleALU, 0);
            m_Opcodes[0x19] = new YCPUInstruction("SUB", SUB, DisassembleALU, 0);
            m_Opcodes[0x1A] = new YCPUInstruction("SUB", SUB, DisassembleALU, 0);
            m_Opcodes[0x1B] = new YCPUInstruction("SUB", SUB, DisassembleALU, 0);
            m_Opcodes[0x1C] = new YCPUInstruction("SUB", SUB, DisassembleALU, 0);
            m_Opcodes[0x1D] = new YCPUInstruction("SUB", SUB, DisassembleALU, 0);
            m_Opcodes[0x1E] = new YCPUInstruction("SUB", SUB, DisassembleALU, 0);
            m_Opcodes[0x1F] = new YCPUInstruction("SUB", SUB, DisassembleALU, 0);
            m_Opcodes[0x20] = new YCPUInstruction("ADC", ADC, DisassembleALU, 0);
            m_Opcodes[0x21] = new YCPUInstruction("ADC", ADC, DisassembleALU, 0);
            m_Opcodes[0x22] = new YCPUInstruction("ADC", ADC, DisassembleALU, 0);
            m_Opcodes[0x23] = new YCPUInstruction("ADC", ADC, DisassembleALU, 0);
            m_Opcodes[0x24] = new YCPUInstruction("ADC", ADC, DisassembleALU, 0);
            m_Opcodes[0x25] = new YCPUInstruction("ADC", ADC, DisassembleALU, 0);
            m_Opcodes[0x26] = new YCPUInstruction("ADC", ADC, DisassembleALU, 0);
            m_Opcodes[0x27] = new YCPUInstruction("ADC", ADC, DisassembleALU, 0);
            m_Opcodes[0x28] = new YCPUInstruction("SBC", SBC, DisassembleALU, 0);
            m_Opcodes[0x29] = new YCPUInstruction("SBC", SBC, DisassembleALU, 0);
            m_Opcodes[0x2A] = new YCPUInstruction("SBC", SBC, DisassembleALU, 0);
            m_Opcodes[0x2B] = new YCPUInstruction("SBC", SBC, DisassembleALU, 0);
            m_Opcodes[0x2C] = new YCPUInstruction("SBC", SBC, DisassembleALU, 0);
            m_Opcodes[0x2D] = new YCPUInstruction("SBC", SBC, DisassembleALU, 0);
            m_Opcodes[0x2E] = new YCPUInstruction("SBC", SBC, DisassembleALU, 0);
            m_Opcodes[0x2F] = new YCPUInstruction("SBC", SBC, DisassembleALU, 0);
            m_Opcodes[0x30] = new YCPUInstruction("MUL", MUL, DisassembleALU, 7);
            m_Opcodes[0x31] = new YCPUInstruction("MUL", MUL, DisassembleALU, 7);
            m_Opcodes[0x32] = new YCPUInstruction("MUL", MUL, DisassembleALU, 7);
            m_Opcodes[0x33] = new YCPUInstruction("MUL", MUL, DisassembleALU, 7);
            m_Opcodes[0x34] = new YCPUInstruction("MUL", MUL, DisassembleALU, 7);
            m_Opcodes[0x35] = new YCPUInstruction("MUL", MUL, DisassembleALU, 7);
            m_Opcodes[0x36] = new YCPUInstruction("MUL", MUL, DisassembleALU, 7);
            m_Opcodes[0x37] = new YCPUInstruction("MUL", MUL, DisassembleALU, 7);
            m_Opcodes[0x38] = new YCPUInstruction("DIV", DIV, DisassembleALU, 47);
            m_Opcodes[0x39] = new YCPUInstruction("DIV", DIV, DisassembleALU, 47);
            m_Opcodes[0x3A] = new YCPUInstruction("DIV", DIV, DisassembleALU, 47);
            m_Opcodes[0x3B] = new YCPUInstruction("DIV", DIV, DisassembleALU, 47);
            m_Opcodes[0x3C] = new YCPUInstruction("DIV", DIV, DisassembleALU, 47);
            m_Opcodes[0x3D] = new YCPUInstruction("DIV", DIV, DisassembleALU, 47);
            m_Opcodes[0x3E] = new YCPUInstruction("DIV", DIV, DisassembleALU, 47);
            m_Opcodes[0x3F] = new YCPUInstruction("DIV", DIV, DisassembleALU, 47);
            m_Opcodes[0x40] = new YCPUInstruction("MLI", MLI, DisassembleALU, 7);
            m_Opcodes[0x41] = new YCPUInstruction("MLI", MLI, DisassembleALU, 7);
            m_Opcodes[0x42] = new YCPUInstruction("MLI", MLI, DisassembleALU, 7);
            m_Opcodes[0x43] = new YCPUInstruction("MLI", MLI, DisassembleALU, 7);
            m_Opcodes[0x44] = new YCPUInstruction("MLI", MLI, DisassembleALU, 7);
            m_Opcodes[0x45] = new YCPUInstruction("MLI", MLI, DisassembleALU, 7);
            m_Opcodes[0x46] = new YCPUInstruction("MLI", MLI, DisassembleALU, 7);
            m_Opcodes[0x47] = new YCPUInstruction("MLI", MLI, DisassembleALU, 7);
            m_Opcodes[0x48] = new YCPUInstruction("DVI", DVI, DisassembleALU, 47);
            m_Opcodes[0x49] = new YCPUInstruction("DVI", DVI, DisassembleALU, 47);
            m_Opcodes[0x4A] = new YCPUInstruction("DVI", DVI, DisassembleALU, 47);
            m_Opcodes[0x4B] = new YCPUInstruction("DVI", DVI, DisassembleALU, 47);
            m_Opcodes[0x4C] = new YCPUInstruction("DVI", DVI, DisassembleALU, 47);
            m_Opcodes[0x4D] = new YCPUInstruction("DVI", DVI, DisassembleALU, 47);
            m_Opcodes[0x4E] = new YCPUInstruction("DVI", DVI, DisassembleALU, 47);
            m_Opcodes[0x4F] = new YCPUInstruction("DVI", DVI, DisassembleALU, 47);
            m_Opcodes[0x50] = new YCPUInstruction("MOD", MOD, DisassembleALU, 47);
            m_Opcodes[0x51] = new YCPUInstruction("MOD", MOD, DisassembleALU, 47);
            m_Opcodes[0x52] = new YCPUInstruction("MOD", MOD, DisassembleALU, 47);
            m_Opcodes[0x53] = new YCPUInstruction("MOD", MOD, DisassembleALU, 47);
            m_Opcodes[0x54] = new YCPUInstruction("MOD", MOD, DisassembleALU, 47);
            m_Opcodes[0x55] = new YCPUInstruction("MOD", MOD, DisassembleALU, 47);
            m_Opcodes[0x56] = new YCPUInstruction("MOD", MOD, DisassembleALU, 47);
            m_Opcodes[0x57] = new YCPUInstruction("MOD", MOD, DisassembleALU, 47);
            m_Opcodes[0x58] = new YCPUInstruction("MDI", MDI, DisassembleALU, 47);
            m_Opcodes[0x59] = new YCPUInstruction("MDI", MDI, DisassembleALU, 47);
            m_Opcodes[0x5A] = new YCPUInstruction("MDI", MDI, DisassembleALU, 47);
            m_Opcodes[0x5B] = new YCPUInstruction("MDI", MDI, DisassembleALU, 47);
            m_Opcodes[0x5C] = new YCPUInstruction("MDI", MDI, DisassembleALU, 47);
            m_Opcodes[0x5D] = new YCPUInstruction("MDI", MDI, DisassembleALU, 47);
            m_Opcodes[0x5E] = new YCPUInstruction("MDI", MDI, DisassembleALU, 47);
            m_Opcodes[0x5F] = new YCPUInstruction("MDI", MDI, DisassembleALU, 47);
            m_Opcodes[0x60] = new YCPUInstruction("AND", AND, DisassembleALU, 0);
            m_Opcodes[0x61] = new YCPUInstruction("AND", AND, DisassembleALU, 0);
            m_Opcodes[0x62] = new YCPUInstruction("AND", AND, DisassembleALU, 0);
            m_Opcodes[0x63] = new YCPUInstruction("AND", AND, DisassembleALU, 0);
            m_Opcodes[0x64] = new YCPUInstruction("AND", AND, DisassembleALU, 0);
            m_Opcodes[0x65] = new YCPUInstruction("AND", AND, DisassembleALU, 0);
            m_Opcodes[0x66] = new YCPUInstruction("AND", AND, DisassembleALU, 0);
            m_Opcodes[0x67] = new YCPUInstruction("AND", AND, DisassembleALU, 0);
            m_Opcodes[0x68] = new YCPUInstruction("ORR", ORR, DisassembleALU, 0);
            m_Opcodes[0x69] = new YCPUInstruction("ORR", ORR, DisassembleALU, 0);
            m_Opcodes[0x6A] = new YCPUInstruction("ORR", ORR, DisassembleALU, 0);
            m_Opcodes[0x6B] = new YCPUInstruction("ORR", ORR, DisassembleALU, 0);
            m_Opcodes[0x6C] = new YCPUInstruction("ORR", ORR, DisassembleALU, 0);
            m_Opcodes[0x6D] = new YCPUInstruction("ORR", ORR, DisassembleALU, 0);
            m_Opcodes[0x6E] = new YCPUInstruction("ORR", ORR, DisassembleALU, 0);
            m_Opcodes[0x6F] = new YCPUInstruction("ORR", ORR, DisassembleALU, 0);
            m_Opcodes[0x70] = new YCPUInstruction("EOR", EOR, DisassembleALU, 0);
            m_Opcodes[0x71] = new YCPUInstruction("EOR", EOR, DisassembleALU, 0);
            m_Opcodes[0x72] = new YCPUInstruction("EOR", EOR, DisassembleALU, 0);
            m_Opcodes[0x73] = new YCPUInstruction("EOR", EOR, DisassembleALU, 0);
            m_Opcodes[0x74] = new YCPUInstruction("EOR", EOR, DisassembleALU, 0);
            m_Opcodes[0x75] = new YCPUInstruction("EOR", EOR, DisassembleALU, 0);
            m_Opcodes[0x76] = new YCPUInstruction("EOR", EOR, DisassembleALU, 0);
            m_Opcodes[0x77] = new YCPUInstruction("EOR", EOR, DisassembleALU, 0);
            m_Opcodes[0x78] = new YCPUInstruction("NOT", NOT, DisassembleALU, 0);
            m_Opcodes[0x79] = new YCPUInstruction("NOT", NOT, DisassembleALU, 0);
            m_Opcodes[0x7A] = new YCPUInstruction("NOT", NOT, DisassembleALU, 0);
            m_Opcodes[0x7B] = new YCPUInstruction("NOT", NOT, DisassembleALU, 0);
            m_Opcodes[0x7C] = new YCPUInstruction("NOT", NOT, DisassembleALU, 0);
            m_Opcodes[0x7D] = new YCPUInstruction("NOT", NOT, DisassembleALU, 0);
            m_Opcodes[0x7E] = new YCPUInstruction("NOT", NOT, DisassembleALU, 0);
            m_Opcodes[0x7F] = new YCPUInstruction("NOT", NOT, DisassembleALU, 0);
            m_Opcodes[0x80] = new YCPUInstruction("LOD", LOD, DisassembleALU, 0);
            m_Opcodes[0x81] = new YCPUInstruction("LOD", LOD, DisassembleALU, 0);
            m_Opcodes[0x82] = new YCPUInstruction("LOD", LOD, DisassembleALU, 0);
            m_Opcodes[0x83] = new YCPUInstruction("LOD", LOD, DisassembleALU, 0);
            m_Opcodes[0x84] = new YCPUInstruction("LOD", LOD, DisassembleALU, 0);
            m_Opcodes[0x85] = new YCPUInstruction("LOD", LOD, DisassembleALU, 0);
            m_Opcodes[0x86] = new YCPUInstruction("LOD", LOD, DisassembleALU, 0);
            m_Opcodes[0x87] = new YCPUInstruction("LOD", LOD, DisassembleALU, 0);
            m_Opcodes[0x88] = new YCPUInstruction("STO", STO, DisassembleALU, 0);
            m_Opcodes[0x89] = new YCPUInstruction("STO", STO, DisassembleALU, 0);
            m_Opcodes[0x8A] = new YCPUInstruction("STO", STO, DisassembleALU, 0);
            m_Opcodes[0x8B] = new YCPUInstruction("STO", STO, DisassembleALU, 0);
            m_Opcodes[0x8C] = new YCPUInstruction("STO", STO, DisassembleALU, 0);
            m_Opcodes[0x8D] = new YCPUInstruction("STO", STO, DisassembleALU, 0);
            m_Opcodes[0x8E] = new YCPUInstruction("STO", STO, DisassembleALU, 0);
            m_Opcodes[0x8F] = new YCPUInstruction("STO", STO, DisassembleALU, 0);
            m_Opcodes[0x90] = new YCPUInstruction("BCC", BCC, DisassembleBRA, 0);
            m_Opcodes[0x91] = new YCPUInstruction("BCS", BCS, DisassembleBRA, 0);
            m_Opcodes[0x92] = new YCPUInstruction("BNE", BNE, DisassembleBRA, 0);
            m_Opcodes[0x93] = new YCPUInstruction("BEQ", BEQ, DisassembleBRA, 0);
            m_Opcodes[0x94] = new YCPUInstruction("BPL", BPL, DisassembleBRA, 0);
            m_Opcodes[0x95] = new YCPUInstruction("BMI", BMI, DisassembleBRA, 0);
            m_Opcodes[0x96] = new YCPUInstruction("BVC", BVC, DisassembleBRA, 0);
            m_Opcodes[0x97] = new YCPUInstruction("BVS", BVS, DisassembleBRA, 0);
            m_Opcodes[0x98] = new YCPUInstruction("BUG", BUG, DisassembleBRA, 0);
            m_Opcodes[0x99] = new YCPUInstruction("BSG", BSG, DisassembleBRA, 0);
            // 0x9A - 0x9E are Undefined operations in the branch opcode space.
            m_Opcodes[0x9F] = new YCPUInstruction("BAW", BAW, DisassembleBRA, 0);
            m_Opcodes[0xA0] = new YCPUInstruction("ASL", ASL, DisassembleSHF, 3);
            m_Opcodes[0xA1] = new YCPUInstruction("LSL", ASL, DisassembleSHF, 3); // ASL == LSL, per specification.
            m_Opcodes[0xA2] = new YCPUInstruction("ROL", ROL, DisassembleSHF, 1);
            m_Opcodes[0xA3] = new YCPUInstruction("RNL", RNL, DisassembleSHF, 1);
            m_Opcodes[0xA4] = new YCPUInstruction("ASR", ASR, DisassembleSHF, 3);
            m_Opcodes[0xA5] = new YCPUInstruction("LSR", LSR, DisassembleSHF, 3);
            m_Opcodes[0xA6] = new YCPUInstruction("ROR", ROR, DisassembleSHF, 1);
            m_Opcodes[0xA7] = new YCPUInstruction("RNR", RNR, DisassembleSHF, 1);
            m_Opcodes[0xA8] = new YCPUInstruction("BTT", BIT, DisassembleBTT, 1);
            m_Opcodes[0xA9] = new YCPUInstruction("BTX", BTX, DisassembleBTT, 1);
            m_Opcodes[0xAA] = new YCPUInstruction("BTC", BTC, DisassembleBTT, 1);
            m_Opcodes[0xAB] = new YCPUInstruction("BTS", BTS, DisassembleBTT, 1);
            m_Opcodes[0xAC] = new YCPUInstruction("SET", SET, DisassembleSET, 0);
            m_Opcodes[0xAD] = new YCPUInstruction("SET", SET, DisassembleSET, 0);
            m_Opcodes[0xAE] = new YCPUInstruction("SEF", SEF, DisassembleFLG, 0);
            m_Opcodes[0xAF] = new YCPUInstruction("CLF", CLF, DisassembleFLG, 0);
            m_Opcodes[0xB0] = new YCPUInstruction("PSH", PSH, DisassembleSTK, 0);
            m_Opcodes[0xB1] = new YCPUInstruction("PSH", PSH, DisassembleSTK, 0);
            m_Opcodes[0xB2] = new YCPUInstruction("POP", POP, DisassembleSTK, 0);
            m_Opcodes[0xB3] = new YCPUInstruction("POP", POP, DisassembleSTK, 0);
            m_Opcodes[0xB4] = new YCPUInstruction("PRX", PRX, DisassemblePRX, 1);
            m_Opcodes[0xB5] = new YCPUInstruction("XSG", XSG, DisassembleXSG, 3);
            m_Opcodes[0xB6] = new YCPUInstruction("ADI", ADI, DisassembleINC, 0);
            m_Opcodes[0xB7] = new YCPUInstruction("SBI", SBI, DisassembleINC, 0);
            m_Opcodes[0xB8] = new YCPUInstruction("JMP", JMP, DisassembleJMP, 0);
            m_Opcodes[0xB9] = new YCPUInstruction("JSR", JSR, DisassembleJMP, 1);
            m_Opcodes[0xBA] = new YCPUInstruction("HWQ", HWQ, DisassembleHWQ, 0);
            m_Opcodes[0xBB] = new YCPUInstruction("STX", STX, DisassembleSTX, 0);

            // 0xBE = 0xFF are undefined (66 opcodes).
            for (int i = 0; i < 0x100; i += 1)
                if (m_Opcodes[i].Opcode == null)
                    m_Opcodes[i] = new YCPUInstruction("NOP", NOP, DisassembleNoBits, 1, true);
        }

        #endregion

        #region NOP

        private void NOP(ushort operand) {
            // do nohting for one cycle.
        }

        #endregion

        #region SET Instructions

        private void SET(ushort operand) {
            RegGeneral regDestination;
            ushort value;
            BitPatternSET(operand, out value, out regDestination);
            R[(int)regDestination] = value;
        }

        #endregion

        #region STX Instructions

        private void STX(ushort operand) {
            int sp_delta = (sbyte)((operand & 0xff00) >> 8);
            if (PS_S) {
                SSP = (ushort)(SSP + sp_delta * 2);
            }
            else {
                USP = (ushort)(USP + sp_delta * 2);
            }
        }

        #endregion

        #region Segment Register Instructions

        private void XSG(ushort operand) {
            if (!PS_S) {
                Interrupt_UnPrivFault(operand);
                return;
            }
            bool push = (operand & 0x0100) != 0;
            int register = (operand & 0x0E00) >> 9;
            bool user = (operand & 0x8000) != 0;
            Segment segment;
            switch ((SegmentIndex)register) {
                case SegmentIndex.CS:
                    segment = user ? m_CSU : m_CSS;
                    break;
                case SegmentIndex.DS:
                    segment = user ? m_DSU : m_DSS;
                    break;
                case SegmentIndex.ES:
                    segment = user ? m_ESU : m_ESS;
                    break;
                case SegmentIndex.SS:
                    segment = user ? m_SSU : m_SSS;
                    break;
                case SegmentIndex.IS:
                    if (user) {
                        Interrupt_UndefFault(operand);
                        return;
                    }
                    segment = m_IS;
                    break;
                default:
                    // operand does not include a valid segment index.
                    Interrupt_UndefFault(operand);
                    return;
            }
            if (push) {
                ushort register_lo = (ushort)segment.Register;
                ushort register_hi = (ushort)(segment.Register >> 16);
                StackPush(operand, register_hi);
                StackPush(operand, register_lo);
            }
            else {
                ushort register_lo = StackPop(operand);
                ushort register_hi = StackPop(operand);
                segment.Register = (uint)(register_hi << 16) | register_lo;
            }
        }

        #endregion

        #region ALU Instructions

        private void ADC(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                int u_result = R[(int)destination] + value + Carry;
                int s_result = (short)R[(int)destination] + (short)value + Carry;
                R[(int)destination] = (ushort)(u_result & 0x0000FFFF);
                FL_N = (u_result & 0x8000) != 0;
                FL_Z = u_result == 0x0000;
                FL_C = (u_result & 0xFFFF0000) != 0;
                FL_V = (s_result < -0x8000) | (s_result > 0x7FFF);
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void ADD(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                int u_result = R[(int)destination] + value;
                int s_result = (short)R[(int)destination] + (short)value;
                R[(int)destination] = (ushort)(u_result & 0x0000FFFF);
                FL_N = (u_result & 0x8000) != 0;
                FL_Z = u_result == 0x0000;
                FL_C = (u_result & 0xFFFF0000) != 0;
                FL_V = (s_result < -0x8000) | (s_result > 0x7FFF);
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void AND(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                int result = R[(int)destination] & value;
                R[(int)destination] = (ushort)(result & 0x0000FFFF);
                FL_N = (result & 0x8000) != 0;
                FL_Z = result == 0x0000;
                // C [Carry] Not effected.
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void CMP(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                int register = R[(int)destination];
                FL_N = (short)register >= (short)value;
                FL_Z = register == value;
                FL_C = register >= value;
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void DIV(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                if (value == 0) {
                    Interrupt_DivZeroFault(operand);
                    return;
                }
                int result = R[(int)destination] / value;
                R[(int)destination] = (ushort)(result & 0x0000FFFF);
                FL_N = false;
                FL_Z = value == 0x0000;
                // C [Carry] Not effected.
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void DVI(ushort operand) {
            try {
                ushort value;
                RegGeneral destination;
                BitPatternALU(operand, out value, out destination);
                if (value == 0) {
                    Interrupt_DivZeroFault(operand);
                    return;
                }
                if ((R[(int)destination] == 0x8000) && (value == 0xFFFF)) {
                    // R is unchanged.
                    FL_N = true;
                    FL_Z = false;
                    // C [Carry] Not effected.
                    FL_V = true;
                }
                else {
                    int result = (short)R[(int)destination] / (short)value;
                    R[(int)RegGeneral.R0] = (ushort)(result >> 16);
                    R[(int)destination] = (ushort)(result & 0x0000FFFF);
                    FL_N = (result & 0x8000) != 0;
                    FL_Z = result == 0x0000;
                    // C [Carry] Not effected.
                    FL_V = false;
                }
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void EOR(ushort operand) {
            try {
                ushort value;
                RegGeneral destination;
                BitPatternALU(operand, out value, out destination);
                int result = R[(int)destination] ^ value;
                R[(int)destination] = (ushort)(result & 0x0000FFFF);
                FL_N = (value & 0x8000) != 0;
                FL_Z = value == 0x0000;
                // C [Carry] Not effected.
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void LOD(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                R[(int)destination] = value;
                FL_N = (value & 0x8000) != 0;
                FL_Z = value == 0x0000;
                // C [Carry] Not effected.
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void MDI(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                if (value == 0) {
                    Interrupt_DivZeroFault(operand);
                    return;
                }
                int s_result = (short)R[(int)destination] % (short)value;
                ushort r = (ushort)(s_result & 0x0000FFFF);
                R[(int)destination] = r;
                FL_N = (r & 0x8000) != 0;
                FL_Z = r == 0x0000;
                // C [Carry] Not effected.
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void MLI(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                R[(int)destination] = 0xFFFE;
                value = 0x8000;
                int result = (short)R[(int)destination] * (short)value;
                R[(int)RegGeneral.R0] = (ushort)(result >> 16);
                FL_C = R[(int)RegGeneral.R0] != 0;
                R[(int)destination] = (ushort)(result & 0x0000FFFF);
                FL_N = (result & 0x80000000) != 0;
                FL_Z = result == 0;
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void MOD(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                if (value == 0) {
                    Interrupt_DivZeroFault(operand);
                    return;
                }
                int result = R[(int)destination] % value;
                R[(int)destination] = (ushort)(result & 0x0000FFFF);
                FL_N = (result & 0x8000) != 0;
                FL_Z = result == 0x0000;
                // C [Carry] Not effected.
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void MUL(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                int result = R[(int)destination] * value;
                R[(int)RegGeneral.R0] = (ushort)(result >> 16);
                FL_C = R[(int)RegGeneral.R0] != 0;
                R[(int)destination] = (ushort)(result & 0x0000FFFF);
                FL_N = false; // Always cleared.
                FL_Z = result == 0;
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void NEG(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                if (value == 0x8000) {
                    // negated value of -32768 doesn't fit in 16-bits.
                    R[(int)destination] = 0x8000;
                    FL_N = true;
                    FL_Z = false;
                    // C [Carry] Not effected.
                    FL_V = true;
                }
                else {
                    int result = 0 - value;
                    R[(int)destination] = (ushort)(result & 0x0000FFFF);
                    FL_N = (result & 0x8000) != 0;
                    FL_Z = result == 0x0000;
                    // C [Carry] Not effected.
                    FL_V = false;
                }
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void NOT(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                int result = ~value;
                R[(int)destination] = (ushort)(result & 0x0000FFFF);
                FL_N = (result & 0x8000) != 0;
                FL_Z = result == 0x0000;
                // C [Carry] Not effected.
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void ORR(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                int result = R[(int)destination] | value;
                R[(int)destination] = (ushort)(result & 0x0000FFFF);
                FL_N = (result & 0x8000) != 0;
                FL_Z = result == 0x0000;
                // C [Carry] Not effected.
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void SBC(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                int u_result = R[(int)destination] - value - (1 - Carry);
                int s_result = (short)R[(int)destination] - (short)value - (1 - Carry);
                R[(int)destination] = (ushort)(u_result & 0x0000FFFF);
                FL_N = (u_result & 0x8000) != 0;
                FL_Z = u_result == 0x0000;
                FL_C = (u_result & 0xFFFF0000) != 0;
                FL_V = (s_result < -0x8000) | (s_result > 0x7FFF);
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void STO(ushort operand) {
            RegGeneral source;
            ushort dest_address;
            try {
                BitPatternSTO(operand, out dest_address, out source);
                if (source == RegGeneral.None) {}
                else {
                    SegmentIndex dataSeg = (operand & 0x8000) != 0 ? SegmentIndex.ES : SegmentIndex.DS; // S = extra segment select.
                    bool eightBitMode = (operand & 0x0100) != 0; // E = 8-bit mode
                    if (eightBitMode)
                        WriteMemInt8(dest_address, (byte)R[(int)source], dataSeg);
                    else
                        WriteMemInt16(dest_address, R[(int)source], dataSeg);
                }
                // N [Negative] Not effected.
                // Z [Zero] Not effected.
                // C [Carry] Not effected.
                // V [Overflow] Not effected.
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void SUB(ushort operand) {
            ushort value;
            RegGeneral destination;
            try {
                BitPatternALU(operand, out value, out destination);
                int u_result = R[(int)destination] - value;
                int s_result = (short)R[(int)destination] - (short)value;
                R[(int)destination] = (ushort)(u_result & 0x0000FFFF);
                FL_N = (u_result & 0x8000) != 0;
                FL_Z = u_result == 0x0000;
                FL_C = (u_result & 0xFFFF0000) != 0;
                FL_V = (s_result < -0x8000) | (s_result > 0x7FFF);
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        #endregion

        #region INC Instructions

        private void ADI(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternIMM(operand, out value, out destination);
            int u_result = R[(int)destination] + value;
            int s_result = (short)R[(int)destination] + value;
            R[(int)destination] = (ushort)(u_result & 0x0000FFFF);
            FL_N = (u_result & 0x8000) != 0;
            FL_Z = u_result == 0x0000;
            FL_C = (u_result & 0xFFFF0000) != 0;
            FL_V = (s_result < -0x8000) | (s_result > 0x7FFF);
        }

        private void SBI(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternIMM(operand, out value, out destination);
            int u_result = R[(int)destination] - value;
            int s_result = (short)R[(int)destination] - value;
            R[(int)destination] = (ushort)(u_result & 0x0000FFFF);
            FL_N = (u_result & 0x8000) != 0;
            FL_Z = u_result == 0x0000;
            FL_C = (u_result & 0xFFFF0000) != 0;
            FL_V = (s_result < -0x8000) | (s_result > 0x7FFF);
        }

        #endregion

        #region Bit Testing Instructions

        private void BIT(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternBTI(operand, out value, out destination);
            ushort bit = (ushort)Math.Pow(2, value);
            if ((R[(int)destination] & bit) != 0) {
                FL_Z = false;
            }
            else {
                FL_Z = true;
            }
        }

        private void BTX(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternBTI(operand, out value, out destination);
            ushort bit = (ushort)Math.Pow(2, value);
            if ((R[(int)destination] & bit) != 0) {
                FL_Z = false;
                FL_C = true;
            }
            else {
                FL_Z = true;
                FL_C = false;
            }
            R[(int)destination] ^= bit;
        }

        private void BTC(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternBTI(operand, out value, out destination);
            ushort bit = (ushort)Math.Pow(2, value);
            if ((R[(int)destination] & bit) != 0) {
                FL_Z = false;
                FL_C = true;
            }
            else {
                FL_Z = true;
                FL_C = false;
            }
            R[(int)destination] &= (ushort)~bit;
        }

        private void BTS(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternBTI(operand, out value, out destination);
            ushort bit = (ushort)Math.Pow(2, value);
            if ((R[(int)destination] & bit) != 0) {
                FL_Z = false;
                FL_C = false;
            }
            else {
                FL_Z = true;
                FL_C = true;
            }
            R[(int)destination] |= bit;
        }

        #endregion

        #region Branch Instructions

        private void BCC(ushort operand) {
            if (!FL_C) {
                ushort value;
                RegGeneral destination;
                BitPatternBRA(operand, out value, out destination);
                PC = (ushort)(PC + (sbyte)value - 2);
            }
        }

        private void BCS(ushort operand) {
            if (FL_C) {
                ushort value;
                RegGeneral destination;
                BitPatternBRA(operand, out value, out destination);
                PC = (ushort)(PC + (sbyte)value - 2);
            }
        }

        private void BNE(ushort operand) {
            if (!FL_Z) {
                ushort value;
                RegGeneral destination;
                BitPatternBRA(operand, out value, out destination);
                PC = (ushort)(PC + (sbyte)value - 2);
            }
        }

        private void BEQ(ushort operand) {
            if (FL_Z) {
                ushort value;
                RegGeneral destination;
                BitPatternBRA(operand, out value, out destination);
                PC = (ushort)(PC + (sbyte)value - 2);
            }
        }

        private void BPL(ushort operand) {
            if (!FL_N) {
                ushort value;
                RegGeneral destination;
                BitPatternBRA(operand, out value, out destination);
                PC = (ushort)(PC + (sbyte)value - 2);
            }
        }

        private void BMI(ushort operand) {
            if (FL_N) {
                ushort value;
                RegGeneral destination;
                BitPatternBRA(operand, out value, out destination);
                PC = (ushort)(PC + (sbyte)value - 2);
            }
        }

        private void BVC(ushort operand) {
            if (!FL_V) {
                ushort value;
                RegGeneral destination;
                BitPatternBRA(operand, out value, out destination);
                PC = (ushort)(PC + (sbyte)value - 2);
            }
        }

        private void BVS(ushort operand) {
            if (FL_V) {
                ushort value;
                RegGeneral destination;
                BitPatternBRA(operand, out value, out destination);
                PC = (ushort)(PC + (sbyte)value - 2);
            }
        }

        private void BUG(ushort operand) {
            if (!FL_Z && FL_C) {
                ushort value;
                RegGeneral destination;
                BitPatternBRA(operand, out value, out destination);
                PC = (ushort)(PC + (sbyte)value - 2);
            }
        }

        private void BSG(ushort operand) {
            if (!FL_Z && FL_N) {
                ushort value;
                RegGeneral destination;
                BitPatternBRA(operand, out value, out destination);
                PC = (ushort)(PC + (sbyte)value - 2);
            }
        }

        private void BAW(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternBRA(operand, out value, out destination);
            PC = (ushort)(PC + (sbyte)value - 2);
        }

        #endregion

        #region FLG Instructions

        private void SEF(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternFLG(operand, out value, out destination);
            if ((operand & 0x8000) != 0)
                FL_N = true;
            if ((operand & 0x4000) != 0)
                FL_Z = true;
            if ((operand & 0x2000) != 0)
                FL_C = true;
            if ((operand & 0x1000) != 0)
                FL_V = true;
        }

        private void CLF(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternFLG(operand, out value, out destination);
            if ((operand & 0x8000) != 0)
                FL_N = false;
            if ((operand & 0x4000) != 0)
                FL_Z = false;
            if ((operand & 0x2000) != 0)
                FL_C = false;
            if ((operand & 0x1000) != 0)
                FL_V = false;
        }

        #endregion

        #region JMP Instructions

        private void JMP(ushort operand) {
            ushort value;
            uint farValue;
            bool isFar;
            try {
                BitPatternJMI(operand, out value, out farValue, out isFar);
                if (isFar && !PS_S) {
                    Interrupt_UnPrivFault(operand);
                }
                else {
                    PC = value;
                }
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        private void JSR(ushort operand) {
            ushort value;
            uint farValue;
            bool isFar;
            try {
                BitPatternJMI(operand, out value, out farValue, out isFar);
                if (isFar && !PS_S) {
                    Interrupt_UnPrivFault(operand);
                }
                else if (isFar) {
                    StackPush(operand, (ushort)(farValue >> 16));
                    StackPush(operand, (ushort)farValue);
                }
                StackPush(operand, PC);
                PC = value;
            }
            catch (SegFaultException e) {
                Interrupt_SegFault(e.SegmentType, operand, e.Address);
            }
        }

        #endregion

        #region SHF Instructions

        private void ASL(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternSHF(operand, out value, out destination);
            int register = (short)R[(int)destination] << value;
            R[(int)destination] = (ushort)(register & 0x0000FFFF);
            FL_N = (R[(int)destination] & 0x8000) != 0;
            FL_Z = register == 0x0000;
            FL_C = (register & 0xFFFF0000) != 0;
            // V [Overflow]    Not effected.
        }

        private void ASR(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternSHF(operand, out value, out destination);
            int register = (short)R[(int)destination] >> value;
            int mask = (int)Math.Pow(2, value) - 1;
            FL_C = (mask & register) != 0;
            R[(int)destination] = (ushort)(register & 0x0000FFFF);
            FL_N = (R[(int)destination] & 0x8000) != 0;
            FL_Z = register == 0x0000;
            // V [Overflow]    Not effected.
        }

        private void LSR(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternSHF(operand, out value, out destination);
            uint register = (uint)R[(int)destination] >> value;
            uint mask = (uint)Math.Pow(2, value) - 1;
            FL_C = (mask & register) != 0;
            R[(int)destination] = (ushort)(register & 0x0000FFFF);
            FL_N = (R[(int)destination] & 0x8000) != 0;
            FL_Z = register == 0x0000;
            // V [Overflow]    Not effected.
        }

        private void RNL(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternSHF(operand, out value, out destination);
            if (value != 0) {
                uint register = (uint)R[(int)destination] << value;
                uint lo_bits = (register & 0xFFFF0000) >> 16;
                R[(int)destination] = (ushort)((register & 0x0000FFFF) | lo_bits);
                // C [Carry]       Not effected.
                // V [Overflow]    Not effected.
            }
            FL_N = (R[(int)destination] & 0x8000) != 0;
            FL_Z = R[(int)destination] == 0x0000;
        }

        private void RNR(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternSHF(operand, out value, out destination);
            if (value != 0) {
                uint register = (uint)(R[(int)destination] >> value);
                uint lo_mask = (uint)0x0000FFFF >> (16 - value);
                uint lo_bits = (R[(int)destination] & lo_mask) << (16 - value);
                R[(int)destination] = (ushort)((register & 0x0000FFFF) | lo_bits);
                // C [Carry]       Not effected.
                // V [Overflow]    Not effected.
            }
            FL_N = (R[(int)destination] & 0x8000) != 0;
            FL_Z = R[(int)destination] == 0x0000;
        }

        private void ROL(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternSHF(operand, out value, out destination);
            if (value != 0) {
                uint out_carry = (uint)R[(int)destination] & (ushort)Math.Pow(2, 16 - value);
                uint in_carry = FL_C ? (uint)Math.Pow(2, value - 1) : 0;
                uint register = (uint)(R[(int)destination] << value);
                uint hi_mask = 0xFFFF0000 ^ (uint)Math.Pow(2, 15 + value);
                R[(int)destination] = (ushort)(register & 0x0000FFFF | (register & hi_mask) >> 16 | in_carry);
                FL_C = out_carry != 0;

                // V [Overflow]    Not effected.
            }
            FL_N = (R[(int)destination] & 0x8000) != 0;
            FL_Z = R[(int)destination] == 0x0000;
        }

        private void ROR(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternSHF(operand, out value, out destination);
            if (value != 0) {
                R[(int)destination] = 0x000F;
                value = 1;
                FL_C = false;
                uint out_carry = (uint)R[(int)destination] & (ushort)Math.Pow(2, value - 1);
                uint in_carry = FL_C ? (uint)Math.Pow(2, 16 - value) : 0;
                uint register = (uint)(R[(int)destination] >> value);
                uint lo_mask = (uint)0x0000FFFF >> (17 - value);
                uint lo_bits = (R[(int)destination] & lo_mask) << (17 - value);
                R[(int)destination] = (ushort)(register & 0x0000FFFF | lo_bits | in_carry);
                FL_C = out_carry != 0;
                // V [Overflow]    Not effected.
            }
            FL_N = (R[(int)destination] & 0x8000) != 0;
            FL_Z = R[(int)destination] == 0x0000;
        }

        #endregion

        #region PRX Instructions: RTS / RTI / SWI / SLP

        private void PRX(ushort operand) {
            int operation_index = (operand & 0xff00) >> 8;
            switch (operation_index) {
                case 0: // RTS
                    RTS(operand, false);
                    break;
                case 1: // RTS.F
                    RTS(operand, true);
                    break;
                case 2: // RTI
                    RTI(operand);
                    break;
                case 3: // SWI
                    SWI();
                    break;
                case 4: // SLP
                    SLP(operand);
                    break;
                default:
                    Interrupt_UndefFault(operand);
                    break;
            }
        }

        private void RTS(ushort operand, bool far) {
            if (!far) {
                PC = StackPop(operand);
            }
            else {
                if (!PS_S) {
                    Interrupt_UnPrivFault(operand);
                    return;
                }
                PC = StackPop(operand);
                ushort cs_lo = StackPop(operand);
                ushort cs_hi = StackPop(operand);
                m_CSS.Register = (uint)(cs_lo + (cs_hi << 16));
            }
        }

        private void RTI(ushort operand) {
            if (!PS_S) {
                Interrupt_UnPrivFault(operand);
                return;
            }
            ReturnFromInterrupt();
        }

        private void SLP(ushort operand) {
            if (!PS_S) {
                Interrupt_UnPrivFault(operand);
            }

            // pause processor
        }

        private void SWI() {
            Interrupt_SWI();
        }

        #endregion

        #region STK Instructions

        private void PSH(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternSTK(operand, out value, out destination);
            if ((value & 0x0001) == 0) {
                if ((value & 0x0100) != 0)
                    StackPush(operand, R[(int)RegGeneral.R0]);
                if ((value & 0x0200) != 0)
                    StackPush(operand, R[(int)RegGeneral.R1]);
                if ((value & 0x0400) != 0)
                    StackPush(operand, R[(int)RegGeneral.R2]);
                if ((value & 0x0800) != 0)
                    StackPush(operand, R[(int)RegGeneral.R3]);
                if ((value & 0x1000) != 0)
                    StackPush(operand, R[(int)RegGeneral.R4]);
                if ((value & 0x2000) != 0)
                    StackPush(operand, R[(int)RegGeneral.R5]);
                if ((value & 0x4000) != 0)
                    StackPush(operand, R[(int)RegGeneral.R6]);
                if ((value & 0x8000) != 0)
                    StackPush(operand, R[(int)RegGeneral.R7]);
            }
            else {
                if ((value & 0x2000) != 0 ||
                    (value & 0x1000) != 0 ||
                    (value & 0x0800) != 0) {
                    Interrupt_UndefFault(operand);
                }
                else {
                    if ((value & 0x8000) != 0)
                        StackPush(operand, SP);
                    if ((value & 0x4000) != 0)
                        StackPush(operand, USP);
                    if ((value & 0x0400) != 0)
                        StackPush(operand, PS);
                    if ((value & 0x0200) != 0)
                        StackPush(operand, PC);
                    if ((value & 0x0100) != 0)
                        StackPush(operand, FL);
                }
            }
        }

        private void POP(ushort operand) {
            ushort value;
            RegGeneral destination;
            BitPatternSTK(operand, out value, out destination);
            if ((value & 0x0001) == 0) {
                if ((value & 0x8000) != 0)
                    R[(int)RegGeneral.R7] = StackPop(operand);
                if ((value & 0x4000) != 0)
                    R[(int)RegGeneral.R6] = StackPop(operand);
                if ((value & 0x2000) != 0)
                    R[(int)RegGeneral.R5] = StackPop(operand);
                if ((value & 0x1000) != 0)
                    R[(int)RegGeneral.R4] = StackPop(operand);
                if ((value & 0x0800) != 0)
                    R[(int)RegGeneral.R3] = StackPop(operand);
                if ((value & 0x0400) != 0)
                    R[(int)RegGeneral.R2] = StackPop(operand);
                if ((value & 0x0200) != 0)
                    R[(int)RegGeneral.R1] = StackPop(operand);
                if ((value & 0x0100) != 0)
                    R[(int)RegGeneral.R0] = StackPop(operand);
            }
            else {
                if ((value & 0x2000) != 0 ||
                    (value & 0x1000) != 0 ||
                    (value & 0x0800) != 0) {
                    Interrupt_UndefFault(operand);
                }
                else {
                    if ((value & 0x0100) != 0)
                        FL = StackPop(operand);
                    if ((value & 0x0200) != 0)
                        PC = StackPop(operand);
                    if ((value & 0x0400) != 0)
                        PS = StackPop(operand);
                    if ((value & 0x4000) != 0)
                        USP = StackPop(operand);
                    if ((value & 0x8000) != 0)
                        SP = StackPop(operand);
                }
            }
        }

        #endregion
    }
}