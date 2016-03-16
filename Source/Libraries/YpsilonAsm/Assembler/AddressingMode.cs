/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

namespace Ypsilon.Assembler
{
    public enum AddressingMode
    {
        None,
        Immediate,
        ImmediateBig,
        Absolute,
        Register,
        Indirect,
        IndirectOffset,
        IndirectIndexed,
        ControlRegister,
        SegmentRegister
    }
}
