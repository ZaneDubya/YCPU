using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Assembler
{
    public enum AddressingMode
    {
        None,
        Immediate,
        Absolute,
        Register,
        Indirect,
        IndirectOffset,
        IndirectPostInc,
        IndirectPreDec,
        IndirectIndexed
    }
}
