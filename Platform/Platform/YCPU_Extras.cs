using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Platform
{
    partial class YCPU
    {
        #region Delegates, etc.
        delegate void YCPUOpcode(ushort opcode, ushort nextword, YCPUBitPattern bits);
        delegate void YCPUBitPattern(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination);
        delegate string YCPUDisassembler(string name, ushort opcode, ushort nextword, ushort address, out bool uses_next_word);
        #endregion

        struct YCPUInstruction
        {
            public string Name;
            public YCPUOpcode Opcode;
            public YCPUBitPattern BitPattern;
            public YCPUDisassembler Disassembler;
            public int Cycles;

            public YCPUInstruction(string name, YCPUOpcode opcode, YCPUBitPattern bitpattern, YCPUDisassembler disassembler, int cycles)
            {
                Name = name;
                Opcode = opcode;
                BitPattern = bitpattern;
                Disassembler = disassembler;
                Cycles = cycles;
            }

            public bool UsesNextWord(ushort opcode)
            {
                bool value;
                string s = Disassembler(string.Empty, opcode, 0x0000, 0x0000, out value);
                return value;
            }
        }
    }
}
