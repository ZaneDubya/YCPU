/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;
using System.Collections.Generic;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        private Dictionary<string, Func<List<string>, OpcodeFlag, ParserState, List<ushort>>> m_Opcodes;
        private Dictionary<string, ushort> m_Registers;
        private Dictionary<string, ushort> m_ControlRegisters;
        private Dictionary<string, ushort> m_SegmentRegisters;

        private void Initialize()
        {
            InitOpcodeDictionary();
            InitRegisterDictionary();
        }

        private void InitOpcodeDictionary()
        {
            m_Opcodes = new Dictionary<string, Func<List<string>, OpcodeFlag, ParserState, List<ushort>>>();

            // alu instructions
            m_Opcodes.Add("cmp", AssembleCMP);
            m_Opcodes.Add("neg", AssembleNEG);
            m_Opcodes.Add("add", AssembleADD);
            m_Opcodes.Add("sub", AssembleSUB);
            m_Opcodes.Add("adc", AssembleADC);
            m_Opcodes.Add("sbc", AssembleSBC);
            m_Opcodes.Add("mul", AssembleMUL);
            m_Opcodes.Add("div", AssembleDIV);
            m_Opcodes.Add("mli", AssembleMLI);
            m_Opcodes.Add("dvi", AssembleDVI);
            m_Opcodes.Add("mod", AssembleMOD);
            m_Opcodes.Add("mdi", AssembleMDI);
            m_Opcodes.Add("and", AssembleAND);
            m_Opcodes.Add("orr", AssembleORR);
            m_Opcodes.Add("eor", AssembleEOR);
            m_Opcodes.Add("not", AssembleNOT);
            m_Opcodes.Add("lod", AssembleLOD);
            m_Opcodes.Add("sto", AssembleSTO);
            // branch instructions
            m_Opcodes.Add("bcc", AssembleBCC);
            m_Opcodes.Add("buf", AssembleBCC);
            m_Opcodes.Add("bcs", AssembleBCS);
            m_Opcodes.Add("buh", AssembleBCS);
            m_Opcodes.Add("bne", AssembleBNE);
            m_Opcodes.Add("beq", AssembleBEQ);
            m_Opcodes.Add("bpl", AssembleBPL);
            m_Opcodes.Add("bsf", AssembleBPL);
            m_Opcodes.Add("bmi", AssembleBMI);
            m_Opcodes.Add("bsh", AssembleBMI);
            m_Opcodes.Add("bvc", AssembleBVC);
            m_Opcodes.Add("bvs", AssembleBVS);
            m_Opcodes.Add("bug", AssembleBUG);
            m_Opcodes.Add("bsg", AssembleBSG);
            m_Opcodes.Add("baw", AssembleBAW);
            // shift instructions
            m_Opcodes.Add("asl", AssembleASL);
            m_Opcodes.Add("lsl", AssembleLSL);
            m_Opcodes.Add("rol", AssembleROL);
            m_Opcodes.Add("rnl", AssembleRNL);
            m_Opcodes.Add("asr", AssembleASR);
            m_Opcodes.Add("lsr", AssembleLSR);
            m_Opcodes.Add("ror", AssembleROR);
            m_Opcodes.Add("rnr", AssembleRNR);
            // bit testing operations
            m_Opcodes.Add("btt", AssembleBTT);
            m_Opcodes.Add("btx", AssembleBTX);
            m_Opcodes.Add("btc", AssembleBTC);
            m_Opcodes.Add("bts", AssembleBTS);
            // set register to value
            m_Opcodes.Add("set", AssembleSET);
            // flag operations
            m_Opcodes.Add("sef", AssembleSEF);
            m_Opcodes.Add("clf", AssembleCLF);
            // stack operations
            m_Opcodes.Add("psh", AssemblePSH);
            m_Opcodes.Add("pop", AssemblePOP);
            // MMU operations
            m_Opcodes.Add("lsg", AssembleLSG);
            m_Opcodes.Add("ssg", AssembleSSG);
            // increment / decrement / add small immediate
            m_Opcodes.Add("inc", AssembleINC);
            m_Opcodes.Add("adi", AssembleADI);
            m_Opcodes.Add("dec", AssembleDEC);
            m_Opcodes.Add("sbi", AssembleSBI);
            // jump operations
            m_Opcodes.Add("jmp", AssembleJMP);
            m_Opcodes.Add("jsr", AssembleJSR);
            // other instructions
            m_Opcodes.Add("hwq", AssembleHWQ);
            m_Opcodes.Add("slp", AssembleSLP);
            m_Opcodes.Add("swi", AssembleSWI);
            m_Opcodes.Add("rti", AssembleRTI);
            m_Opcodes.Add("rts", AssembleRTS);
            m_Opcodes.Add("nop", AssembleNOP);
            m_Opcodes.Add("stx", AssembleSTX);
        }

        private void InitRegisterDictionary()
        {
            m_Registers = new Dictionary<string, ushort>
            {
                {"r0", (ushort) YCPUReg.R0},
                {"r1", (ushort) YCPUReg.R1},
                {"r2", (ushort) YCPUReg.R2},
                {"r3", (ushort) YCPUReg.R3},
                {"r4", (ushort) YCPUReg.R4},
                {"r5", (ushort) YCPUReg.R5},
                {"r6", (ushort) YCPUReg.R6},
                {"r7", (ushort) YCPUReg.R7},
                {"a", (ushort) YCPUReg.R0},
                {"b", (ushort) YCPUReg.R1},
                {"c", (ushort) YCPUReg.R2},
                {"d", (ushort) YCPUReg.R3},
                {"w", (ushort) YCPUReg.R4},
                {"x", (ushort) YCPUReg.R5},
                {"y", (ushort) YCPUReg.R6},
                {"z", (ushort) YCPUReg.R7}
            };
            // alternate naming scheme

            m_ControlRegisters = new Dictionary<string, ushort>
            {
                {"fl", (ushort) YCPUReg.FL},
                {"pc", (ushort) YCPUReg.PC},
                {"ps", (ushort) YCPUReg.PS},
                {"usp", (ushort) YCPUReg.USP},
                {"sp", (ushort) YCPUReg.SP}
            };

            m_SegmentRegisters = new Dictionary<string, ushort>
            {
                {"cs", 0x0000},
                {"ds", 0x0001},
                {"es", 0x0002},
                {"ss", 0x0003},
                {"css", 0x0000},
                {"dss", 0x0001},
                {"ess", 0x0002},
                {"sss", 0x0003},
                {"csu", 0x0040},
                {"dsu", 0x0041},
                {"esu", 0x0042},
                {"ssu", 0x0043},
                {"is", 0x0004}
            };
        }

        private enum YCPUReg : ushort
        {
            R0 = 0x0000,
            R1 = 0x0001,
            R2 = 0x0002,
            R3 = 0x0003,
            R4 = 0x0004,
            R5 = 0x0005,
            R6 = 0x0006,
            R7 = 0x0007,

            FL = 0x0000,
            PC = 0x0001,
            PS = 0x0002,
            USP = 0x0006,
            SP = 0x0007
        }
    }
}
