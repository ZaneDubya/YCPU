using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Assembler
{
    [Flags]
    enum OpcodeFlag
    {
        BitWidth8 = 0x01,
        BitWidth16 = 0x02,
        BitWidthsAll = 0x0F,
        FarJump = 0x10
    }
}
