/*
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        protected void InitOpcodeDictionary()
        {
            // alu instructions
            m_OpcodeAssemblers.Add("lod", AssembleLOD);
            m_OpcodeAssemblers.Add("sto", AssembleSTO);
            m_OpcodeAssemblers.Add("add", AssembleADD);
            m_OpcodeAssemblers.Add("sub", AssembleSUB);
            m_OpcodeAssemblers.Add("adc", AssembleADC);
            m_OpcodeAssemblers.Add("sbc", AssembleSBC);
            m_OpcodeAssemblers.Add("mul", AssembleMUL);
            m_OpcodeAssemblers.Add("div", AssembleDIV);
            m_OpcodeAssemblers.Add("mli", AssembleMLI);
            m_OpcodeAssemblers.Add("dvi", AssembleDVI);
            m_OpcodeAssemblers.Add("mod", AssembleMOD);
            m_OpcodeAssemblers.Add("mdi", AssembleMDI);
            m_OpcodeAssemblers.Add("and", AssembleAND);
            m_OpcodeAssemblers.Add("orr", AssembleORR);
            m_OpcodeAssemblers.Add("eor", AssembleEOR);
            m_OpcodeAssemblers.Add("not", AssembleNOT);
            m_OpcodeAssemblers.Add("cmp", AssembleCMP);
            m_OpcodeAssemblers.Add("neg", AssembleNEG);
            // branch instructions
            m_OpcodeAssemblers.Add("bcc", AssembleBCC);
            m_OpcodeAssemblers.Add("buf", AssembleBCC);
            m_OpcodeAssemblers.Add("bcs", AssembleBCS);
            m_OpcodeAssemblers.Add("buh", AssembleBCS);
            m_OpcodeAssemblers.Add("bne", AssembleBNE);
            m_OpcodeAssemblers.Add("beq", AssembleBEQ);
            m_OpcodeAssemblers.Add("bpl", AssembleBPL);
            m_OpcodeAssemblers.Add("bsf", AssembleBPL);
            m_OpcodeAssemblers.Add("bmi", AssembleBMI);
            m_OpcodeAssemblers.Add("bsh", AssembleBMI);
            m_OpcodeAssemblers.Add("bvc", AssembleBVC);
            m_OpcodeAssemblers.Add("bvs", AssembleBVS);
            m_OpcodeAssemblers.Add("bug", AssembleBUG);
            m_OpcodeAssemblers.Add("bsg", AssembleBSG);
            m_OpcodeAssemblers.Add("baw", AssembleBAW);
            // shift instructions
            m_OpcodeAssemblers.Add("asl", AssembleASL);
            m_OpcodeAssemblers.Add("lsl", AssembleLSL);
            m_OpcodeAssemblers.Add("rol", AssembleROL);
            m_OpcodeAssemblers.Add("rnl", AssembleRNL);
            m_OpcodeAssemblers.Add("asr", AssembleASR);
            m_OpcodeAssemblers.Add("lsr", AssembleLSR);
            m_OpcodeAssemblers.Add("ror", AssembleROR);
            m_OpcodeAssemblers.Add("rnr", AssembleRNR);
            // bit testing operations
            m_OpcodeAssemblers.Add("bit", AssembleBIT);
            m_OpcodeAssemblers.Add("btx", AssembleBTX);
            m_OpcodeAssemblers.Add("btc", AssembleBTC);
            m_OpcodeAssemblers.Add("bts", AssembleBTS);
            // switch octet
            m_OpcodeAssemblers.Add("swo", AssembleSWO);
            // fpu testing operations
            m_OpcodeAssemblers.Add("fpa", null);
            m_OpcodeAssemblers.Add("fps", null);
            m_OpcodeAssemblers.Add("fpm", null);
            m_OpcodeAssemblers.Add("fpd", null);
            // flag operations
            m_OpcodeAssemblers.Add("sef", AssembleSEF);
            m_OpcodeAssemblers.Add("clf", AssembleCLF);
            // stack operations
            m_OpcodeAssemblers.Add("psh", AssemblePSH);
            m_OpcodeAssemblers.Add("pop", AssemblePOP);
            // increment / decrement
            m_OpcodeAssemblers.Add("inc", AssembleINC);
            m_OpcodeAssemblers.Add("adi", AssembleADI);
            m_OpcodeAssemblers.Add("dec", AssembleDEC);
            m_OpcodeAssemblers.Add("sbi", AssembleSBI);
            // transfer special
            m_OpcodeAssemblers.Add("tsr", AssembleTSR);
            m_OpcodeAssemblers.Add("trs", AssembleTRS);
            // MMU operations
            m_OpcodeAssemblers.Add("mmr", AssembleMMR);
            m_OpcodeAssemblers.Add("mmw", AssembleMMW);
            m_OpcodeAssemblers.Add("mml", AssembleMML);
            m_OpcodeAssemblers.Add("mms", AssembleMMS);
            // jump operations
            m_OpcodeAssemblers.Add("jmp", AssembleJMP);
            m_OpcodeAssemblers.Add("jsr", AssembleJSR);
            m_OpcodeAssemblers.Add("jum", AssembleJUM);
            m_OpcodeAssemblers.Add("jcx", AssembleJCX);
            // other instructions
            m_OpcodeAssemblers.Add("hwq", AssembleHWQ);
            m_OpcodeAssemblers.Add("slp", AssembleSLP);
            m_OpcodeAssemblers.Add("swi", AssembleSWI);
            m_OpcodeAssemblers.Add("rti", AssembleRTI);
            // macros
            m_OpcodeAssemblers.Add("rts", AssembleRTS);
        }

        protected void InitRegisterDictionary()
        {
            m_RegisterDictionary.Add("r0", (ushort)YCPUReg.R0);
            m_RegisterDictionary.Add("r1", (ushort)YCPUReg.R1);
            m_RegisterDictionary.Add("r2", (ushort)YCPUReg.R2);
            m_RegisterDictionary.Add("r3", (ushort)YCPUReg.R3);
            m_RegisterDictionary.Add("r4", (ushort)YCPUReg.R4);
            m_RegisterDictionary.Add("r5", (ushort)YCPUReg.R5);
            m_RegisterDictionary.Add("r6", (ushort)YCPUReg.R6);
            m_RegisterDictionary.Add("r7", (ushort)YCPUReg.R7);

            m_RegisterDictionary.Add("a", (ushort)YCPUReg.R0);
            m_RegisterDictionary.Add("b", (ushort)YCPUReg.R1);
            m_RegisterDictionary.Add("c", (ushort)YCPUReg.R2);
            m_RegisterDictionary.Add("i", (ushort)YCPUReg.R3);
            m_RegisterDictionary.Add("j", (ushort)YCPUReg.R4);
            m_RegisterDictionary.Add("x", (ushort)YCPUReg.R5);
            m_RegisterDictionary.Add("y", (ushort)YCPUReg.R6);
            m_RegisterDictionary.Add("z", (ushort)YCPUReg.R7);

            m_RegisterDictionary.Add("sp", (ushort)YCPUReg.R7);
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
            R7 = 0x0007
        }
    }
}
