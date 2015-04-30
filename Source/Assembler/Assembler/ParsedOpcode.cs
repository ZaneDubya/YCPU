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
    public class ParsedOpcode : DCPU16ASM.ParsedOpcode
    {
        public ParsedOpcode() : base()
        {
            this.AddressingMode = AddressingMode.None;
        }

        public AddressingMode AddressingMode { get; set; }
    }

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
