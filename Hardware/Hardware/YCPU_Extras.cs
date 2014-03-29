using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Platform
{
    partial class YCPU
    {
        public delegate void YCPUOpcode(ushort opcode, ushort nextword, YCPUBitPattern bits);
        public delegate void YCPUBitPattern(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination);
        public delegate string YCPUDisassembler(string name, ushort opcode, ushort nextword, ushort address, out bool uses_next_word);

        public struct YCPUInstruction
        {
            public string Name;
            public YCPUOpcode Opcode;
            public YCPUBitPattern BitPattern;
            public YCPUDisassembler Disassembler;
            public int Cycles;
            public bool IsNOP;

            public YCPUInstruction(string name, YCPUOpcode opcode, YCPUBitPattern bitpattern, YCPUDisassembler disassembler, int cycles, bool isNOP = false)
            {
                Name = name;
                Opcode = opcode;
                BitPattern = bitpattern;
                Disassembler = disassembler;
                Cycles = cycles;
                IsNOP = isNOP;
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
