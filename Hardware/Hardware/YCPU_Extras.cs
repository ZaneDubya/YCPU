using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Hardware
{
    partial class YCPU
    {
        public delegate void YCPUOpcode(ushort opcode, YCPUBitPattern bits);
        public delegate void YCPUBitPattern(ushort operand, out ushort value, out RegGPIndex destination);
        public delegate string YCPUDisassembler(string name, ushort opcode, ushort nextword, ushort address, out bool uses_next_word);
    }
}
