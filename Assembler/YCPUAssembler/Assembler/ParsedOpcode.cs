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
        Register,
        Indirect,
        IndirectOffset,
        IndirectPostInc,
        IndirectPreDec,
        IndirectIndexed
    }
}
