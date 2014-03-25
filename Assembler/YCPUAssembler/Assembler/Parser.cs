using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Assembler
{
    public partial class Parser : DCPU16ASM.Parser
    {
        private Platform.YCPU m_YCPU = new Platform.YCPU();

        public Parser() : base()
        {

        }

        protected override void InitOpcodeDictionary()
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
            m_OpcodeAssemblers.Add("bcc", null);
            m_OpcodeAssemblers.Add("buf", null);
            m_OpcodeAssemblers.Add("bcs", null);
            m_OpcodeAssemblers.Add("buh", null);
            m_OpcodeAssemblers.Add("bne", null);
            m_OpcodeAssemblers.Add("beq", null);
            m_OpcodeAssemblers.Add("bpl", null);
            m_OpcodeAssemblers.Add("bsf", null);
            m_OpcodeAssemblers.Add("bmi", null);
            m_OpcodeAssemblers.Add("bsh", null);
            m_OpcodeAssemblers.Add("bvc", null);
            m_OpcodeAssemblers.Add("bvs", null);
            m_OpcodeAssemblers.Add("bug", null);
            m_OpcodeAssemblers.Add("bsg", null);
            m_OpcodeAssemblers.Add("baw", null);
            // shift instructions
            m_OpcodeAssemblers.Add("asl", null);
            m_OpcodeAssemblers.Add("lsl", null);
            m_OpcodeAssemblers.Add("rol", null);
            m_OpcodeAssemblers.Add("rnl", null);
            m_OpcodeAssemblers.Add("asr", null);
            m_OpcodeAssemblers.Add("lsr", null);
            m_OpcodeAssemblers.Add("ror", null);
            m_OpcodeAssemblers.Add("rnr", null);
            // bit testing operations
            m_OpcodeAssemblers.Add("bit", null);
            m_OpcodeAssemblers.Add("btx", null);
            m_OpcodeAssemblers.Add("btc", null);
            m_OpcodeAssemblers.Add("bts", null);
            // switch octet
            m_OpcodeAssemblers.Add("swo", null);
            // fpu testing operations
            m_OpcodeAssemblers.Add("fpa", null);
            m_OpcodeAssemblers.Add("fps", null);
            m_OpcodeAssemblers.Add("fpm", null);
            m_OpcodeAssemblers.Add("fpd", null);
            // flag operations
            m_OpcodeAssemblers.Add("sef", null);
            m_OpcodeAssemblers.Add("clf", null);
            // stack operations
            m_OpcodeAssemblers.Add("psh", null);
            m_OpcodeAssemblers.Add("pop", null);
            // increment / decrement
            m_OpcodeAssemblers.Add("inc", null);
            m_OpcodeAssemblers.Add("adi", null);
            m_OpcodeAssemblers.Add("dec", null);
            m_OpcodeAssemblers.Add("sbi", null);
            // transfer special
            m_OpcodeAssemblers.Add("tsr", null);
            m_OpcodeAssemblers.Add("trs", null);
            // MMU operations
            m_OpcodeAssemblers.Add("mmr", null);
            m_OpcodeAssemblers.Add("mmw", null);
            m_OpcodeAssemblers.Add("mml", null);
            m_OpcodeAssemblers.Add("mms", null);
            // jump operations
            m_OpcodeAssemblers.Add("jmp", null);
            m_OpcodeAssemblers.Add("jsr", null);
            m_OpcodeAssemblers.Add("jum", null);
            m_OpcodeAssemblers.Add("jcx", null);
            // other instructions
            m_OpcodeAssemblers.Add("slp", null);
            m_OpcodeAssemblers.Add("swi", null);
            m_OpcodeAssemblers.Add("rti", null);
        }

        protected override void InitRegisterDictionary()
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
    }
}
