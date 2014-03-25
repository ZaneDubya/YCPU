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

        ushort[] AssembleSTO(string param1, string param2)
        {
            ushort[] code = AssembleALU((ushort)0x0008, param1, param2);
            int addressing = (code[0] & 0x0007);
            if ((addressing == 0) && (addressing == 1)) // no sto reg or sto immediate.
                return null;
            return AssembleALU((ushort)0x0000, param1, param2);
        }

        ushort[] AssembleADD(string param1, string param2)
        {
            return AssembleALU((ushort)0x0010, param1, param2);
        }

        ushort[] AssembleSUB(string param1, string param2)
        {
            return AssembleALU((ushort)0x0018, param1, param2);
        }

        ushort[] AssembleADC(string param1, string param2)
        {
            return AssembleALU((ushort)0x0020, param1, param2);
        }

        ushort[] AssembleSBC(string param1, string param2)
        {
            return AssembleALU((ushort)0x0028, param1, param2);
        }

        ushort[] AssembleMUL(string param1, string param2)
        {
            return AssembleALU((ushort)0x0030, param1, param2);
        }

        ushort[] AssembleDIV(string param1, string param2)
        {
            return AssembleALU((ushort)0x0038, param1, param2);
        }

        ushort[] AssembleMLI(string param1, string param2)
        {
            return AssembleALU((ushort)0x0040, param1, param2);
        }

        ushort[] AssembleDVI(string param1, string param2)
        {
            return AssembleALU((ushort)0x0048, param1, param2);
        }

        ushort[] AssembleMOD(string param1, string param2)
        {
            return AssembleALU((ushort)0x0050, param1, param2);
        }

        ushort[] AssembleMDI(string param1, string param2)
        {
            return AssembleALU((ushort)0x0058, param1, param2);
        }

        ushort[] AssembleAND(string param1, string param2)
        {
            return AssembleALU((ushort)0x0060, param1, param2);
        }

        ushort[] AssembleORR(string param1, string param2)
        {
            return AssembleALU((ushort)0x0068, param1, param2);
        }

        ushort[] AssembleEOR(string param1, string param2)
        {
            return AssembleALU((ushort)0x0070, param1, param2);
        }

        ushort[] AssembleNOT(string param1, string param2)
        {
            return AssembleALU((ushort)0x0078, param1, param2);
        }

        ushort[] AssembleCMP(string param1, string param2)
        {
            return AssembleALU((ushort)0x0080, param1, param2);
        }

        ushort[] AssembleNEG(string param1, string param2)
        {
            return AssembleALU((ushort)0x0088, param1, param2);
        }








        ushort[] AssembleMML(string param1, string param2)
        {
            return AssembleJMP((ushort)0x00BE, param1);
        }

        ushort[] AssembleMMS(string param1, string param2)
        {
            return AssembleJMP((ushort)0x00BF, param1);
        }

        ushort[] AssembleJMP(string param1, string param2)
        {
            return AssembleJMP((ushort)0x00C0, param1);
        }

        ushort[] AssembleJSR(string param1, string param2)
        {
            return AssembleJMP((ushort)0x00C1, param1);
        }

        ushort[] AssembleJUM(string param1, string param2)
        {
            return AssembleJMP((ushort)0x00C2, param1);
        }

        ushort[] AssembleJCX(string param1, string param2)
        {
            return new ushort[1] { (ushort)0x00C3 };
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
            {
                if (p2.LabelName.Length > 0)
                    m_LabelReferences.Add((ushort)(m_MachineCode.Count + code.Count), p2.LabelName);
                code.Add(p2.NextWord);
            }
            return code.ToArray();
        }

        ushort[] AssembleJMP(ushort opcode, string param1)
        {
            ParsedOpcode p1 = ParseParam(param1);

            if (p1.Illegal)
                return null;
            ushort addressingmode = 0x0000;
            switch (p1.AddressingMode)
            {
                case AddressingMode.None:
                    return null;
                case AddressingMode.Immediate:
                    addressingmode = 0x0000;
                    break;
                case AddressingMode.Register:
                    addressingmode = 0x2000;
                    break;
                case AddressingMode.Indirect:
                    addressingmode = 0x4000;
                    break;
                case AddressingMode.IndirectOffset:
                    addressingmode = 0x6000;
                    break;
                case AddressingMode.IndirectPostInc:
                    addressingmode = 0x8000;
                    break;
                case AddressingMode.IndirectPreDec:
                    addressingmode = 0xA000;
                    break;
                case AddressingMode.IndirectIndexed:
                    int r3 = (p1.Word & 0x0700);
                    addressingmode = (ushort)(0xC000 + (r3 & 0x0300) + ((r3 & 0x0400) << 3));
                    break;
            }
            List<ushort> code = new List<ushort>();
            code.Add((ushort)(opcode | addressingmode | ((p1.Word & 0x0007) << 13)));
            if (p1.UsesNextWord)
            {
                if (p1.LabelName.Length > 0)
                    m_LabelReferences.Add((ushort)(m_MachineCode.Count + code.Count), p1.LabelName);
                code.Add(p1.NextWord);
            }
            return code.ToArray();
        }
    }
}
