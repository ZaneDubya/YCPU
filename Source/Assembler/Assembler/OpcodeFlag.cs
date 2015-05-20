using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Assembler
{
    [Flags]
    enum OpcodeFlag
    {
        BitWidth8 = 1,
        BitWidth16 = 2,
        FarJump = 4
    }
}
