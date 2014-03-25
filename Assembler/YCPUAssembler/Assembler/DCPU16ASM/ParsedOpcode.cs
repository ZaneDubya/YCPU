/*
 * DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

namespace YCPU.Assembler.DCPU16ASM
{
    public class ParsedOpcode
    {
        public ParsedOpcode()
        {
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
    }
}