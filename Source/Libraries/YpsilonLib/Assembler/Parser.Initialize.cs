﻿/* =================================================================
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
        Dictionary<string, Func<List<string>, OpcodeFlag, ParserState, List<ushort>>> m_Opcodes;
        Dictionary<string, ushort> m_Registers;
        Dictionary<string, ushort> m_StatusRegisters;

        void Initialize()
        {
            InitOpcodeDictionary();
            InitRegisterDictionary();
        }

        void InitOpcodeDictionary()
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
            // fpu operations. not implemented.
            m_Opcodes.Add("fpa", null);
            m_Opcodes.Add("fps", null);
            m_Opcodes.Add("fpm", null);
            m_Opcodes.Add("fpd", null);
            // flag operations
            m_Opcodes.Add("sef", AssembleSEF);
            m_Opcodes.Add("clf", AssembleCLF);
            // stack operations
            m_Opcodes.Add("psh", AssemblePSH);
            m_Opcodes.Add("pop", AssemblePOP);
            // sfl used to be here :(
            
            // MMU operations
            m_Opcodes.Add("mmr", AssembleMMR);
            m_Opcodes.Add("mmw", AssembleMMW);
            m_Opcodes.Add("mml", AssembleMML);
            m_Opcodes.Add("mms", AssembleMMS);
            // set register to value
            m_Opcodes.Add("set", AssembleSET);
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
            // macros
            m_Opcodes.Add("rts", AssembleRTS);
            m_Opcodes.Add("nop", AssembleNOP);
        }

        void InitRegisterDictionary()
        {
            m_Registers = new Dictionary<string, ushort>();

            m_Registers.Add("r0", (ushort)YCPUReg.R0);
            m_Registers.Add("r1", (ushort)YCPUReg.R1);
            m_Registers.Add("r2", (ushort)YCPUReg.R2);
            m_Registers.Add("r3", (ushort)YCPUReg.R3);
            m_Registers.Add("r4", (ushort)YCPUReg.R4);
            m_Registers.Add("r5", (ushort)YCPUReg.R5);
            m_Registers.Add("r6", (ushort)YCPUReg.R6);
            m_Registers.Add("r7", (ushort)YCPUReg.R7);

            m_Registers.Add("a", (ushort)YCPUReg.R0);
            m_Registers.Add("b", (ushort)YCPUReg.R1);
            m_Registers.Add("c", (ushort)YCPUReg.R2);
            m_Registers.Add("i", (ushort)YCPUReg.R3);
            m_Registers.Add("j", (ushort)YCPUReg.R4);
            m_Registers.Add("x", (ushort)YCPUReg.R5);
            m_Registers.Add("y", (ushort)YCPUReg.R6);
            m_Registers.Add("z", (ushort)YCPUReg.R7);

            m_StatusRegisters = new Dictionary<string, ushort>();

            m_StatusRegisters.Add("fl", (ushort)YCPUReg.FL);
            m_StatusRegisters.Add("pc", (ushort)YCPUReg.PC);
            m_StatusRegisters.Add("ps", (ushort)YCPUReg.PS);
            m_StatusRegisters.Add("p2", (ushort)YCPUReg.P2);
            m_StatusRegisters.Add("ia", (ushort)YCPUReg.IA);
            m_StatusRegisters.Add("ii", (ushort)YCPUReg.II);
            m_StatusRegisters.Add("usp", (ushort)YCPUReg.USP);
            m_StatusRegisters.Add("sp", (ushort)YCPUReg.SP);
        }

        enum YCPUReg : ushort
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
            P2 = 0x0003,
            II = 0x0004,
            IA = 0x0005,
            USP = 0x0006,
            SP = 0x0007,
        }
    }
}
