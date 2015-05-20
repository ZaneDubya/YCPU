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
    public class ParsedOpcode
    {
        public ParsedOpcode()
        {
            this.AddressingMode = AddressingMode.None;
            this.OpcodeWord = 0x0000;
            this.HasImmediateWord = false;
            this.ImmediateWord = 0x0000;
            this.LabelName = string.Empty;
            this.IsIllegal = false;
        }

        public AddressingMode AddressingMode { get; set; }

        public ushort OpcodeWord { get; set; }

        public bool HasImmediateWord { get; set; }

        public ushort ImmediateWord { get; set; }

        public string LabelName { get; set; }

        public bool IsIllegal { get; set; }
    }
}
