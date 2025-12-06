/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;

namespace Ypsilon.Assembler
{
    [Flags]
    internal enum OpcodeFlag
    {
        BitWidth8 = 0x01,
        BitWidth16 = 0x02,
        BitWidthsAll = 0x0F,
        FarJump = 0x10,
        ExtraDataSegment = 0x20
    }
}
