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

namespace YCPU.Assembler
{
    partial class Parser
    {
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
