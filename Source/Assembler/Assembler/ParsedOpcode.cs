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

namespace Ypsilon.Assembler
{
    public class ParsedOpcode
    {
        public ParsedOpcode()
        {
            this.AddressingMode = AddressingMode.None;
            this.Word = 0x0000;
            this.UsesNextWord = false;
            this.NextWord = 0x0000;
            this.LabelName = string.Empty;
            this.Illegal = false;
        }

        public ushort Word { get; set; }

        public bool UsesNextWord { get; set; }

        public ushort NextWord { get; set; }

        public string LabelName { get; set; }

        public bool Illegal { get; set; }

        public AddressingMode AddressingMode { get; set; }
    }
}
