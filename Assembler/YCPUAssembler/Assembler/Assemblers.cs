using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Assembler
{
    public partial class Parser : DCPU16ASM.Parser
    {
        ushort[] AssembleLOD(string param1, string param2)
        {
            return AssembleALU((ushort)0x0000, param1, param2);
        }

        ushort[] AssembleALU(ushort opcode, string param1, string param2)
        {
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);

            if (p1.Illegal || p2.Illegal)
                return null;
            if (p1.AddressingMode != AddressingMode.Register)
                return null;
            
            ushort addressingmode = 0x0000;
            switch (p2.AddressingMode)
            {
                case AddressingMode.None:
                    return null;
                case AddressingMode.Immediate:
                    addressingmode = 0x0000;
                    break;
                case AddressingMode.Register:
                    addressingmode = 0x0001;
                    break;
                case AddressingMode.Indirect:
                    addressingmode = 0x0002;
                    break;
                case AddressingMode.IndirectOffset:
                    addressingmode = 0x0003;
                    break;
                case AddressingMode.IndirectPostInc:
                    addressingmode = 0x0004;
                    break;
                case AddressingMode.IndirectPreDec:
                    addressingmode = 0x0005;
                    break;
                case AddressingMode.IndirectIndexed:
                    int r3 = (p2.Word & 0x0700);
                    addressingmode = (ushort)(0x0006 + (r3 & 0x0300) + ((r3 & 0x0400) >> 10));
                    break;
            }
            List<ushort> code = new List<ushort>();
            code.Add((ushort)(opcode | addressingmode | ((p1.Word & 0x0007) << 13) | ((p2.Word & 0x0007) << 10)));
            if (p2.UsesNextWord)
                code.Add(p2.NextWord);

            return code.ToArray();
        }
    }
}
