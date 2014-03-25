using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Assembler
{
    public partial class Parser : DCPU16ASM.Parser
    {
        private const ushort c_NOP = 0x0001;

        #region ALU
        ushort[] AssembleLOD(string[] param)
        {
            return AssembleALU((ushort)0x0000, param[0], param[1]);
        }

        ushort[] AssembleSTO(string[] param)
        {
            ushort[] code = AssembleALU((ushort)0x0008, param[0], param[1]);
            int addressing = (code[0] & 0x0007);
            if ((addressing == 0) && (addressing == 1)) // no sto reg or sto immediate.
                return null;
            return AssembleALU((ushort)0x0000, param[0], param[1]);
        }

        ushort[] AssembleADD(string[] param)
        {
            return AssembleALU((ushort)0x0010, param[0], param[1]);
        }

        ushort[] AssembleSUB(string[] param)
        {
            return AssembleALU((ushort)0x0018, param[0], param[1]);
        }

        ushort[] AssembleADC(string[] param)
        {
            return AssembleALU((ushort)0x0020, param[0], param[1]);
        }

        ushort[] AssembleSBC(string[] param)
        {
            return AssembleALU((ushort)0x0028, param[0], param[1]);
        }

        ushort[] AssembleMUL(string[] param)
        {
            return AssembleALU((ushort)0x0030, param[0], param[1]);
        }

        ushort[] AssembleDIV(string[] param)
        {
            return AssembleALU((ushort)0x0038, param[0], param[1]);
        }

        ushort[] AssembleMLI(string[] param)
        {
            return AssembleALU((ushort)0x0040, param[0], param[1]);
        }

        ushort[] AssembleDVI(string[] param)
        {
            return AssembleALU((ushort)0x0048, param[0], param[1]);
        }

        ushort[] AssembleMOD(string[] param)
        {
            return AssembleALU((ushort)0x0050, param[0], param[1]);
        }

        ushort[] AssembleMDI(string[] param)
        {
            return AssembleALU((ushort)0x0058, param[0], param[1]);
        }

        ushort[] AssembleAND(string[] param)
        {
            return AssembleALU((ushort)0x0060, param[0], param[1]);
        }

        ushort[] AssembleORR(string[] param)
        {
            return AssembleALU((ushort)0x0068, param[0], param[1]);
        }

        ushort[] AssembleEOR(string[] param)
        {
            return AssembleALU((ushort)0x0070, param[0], param[1]);
        }

        ushort[] AssembleNOT(string[] param)
        {
            return AssembleALU((ushort)0x0078, param[0], param[1]);
        }

        ushort[] AssembleCMP(string[] param)
        {
            return AssembleALU((ushort)0x0080, param[0], param[1]);
        }

        ushort[] AssembleNEG(string[] param)
        {
            return AssembleALU((ushort)0x0088, param[0], param[1]);
        }
        #endregion

        #region Branch operations
        ushort[] AssembleBCC(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBCS(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBNE(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBEQ(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBPL(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBMI(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBVC(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBVS(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBUG(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBSG(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBAW(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }
        #endregion

        #region shift operations
        ushort[] AssembleASL(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleLSL(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleROL(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleRNL(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleASR(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleLSR(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleROR(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleRNR(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }
        #endregion

        #region Bit testing operations
        ushort[] AssembleBIT(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBTX(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBTC(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleBTS(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }
        #endregion

        #region Switch Octet
        ushort[] AssembleSWO(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }
        #endregion

        #region Set flags
        ushort[] AssembleSEF(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleCLF(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }
        #endregion

        #region Stack Push/Pop
        ushort[] AssemblePSH(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssemblePOP(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }
        #endregion

        #region Inc/Dec
        ushort[] AssembleINC(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleADI(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleDEC(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleSBI(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }
        #endregion

        #region Transfer registers
        ushort[] AssembleTSR(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleTRS(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }
        #endregion

        #region MMU
        ushort[] AssembleMMR(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleMMW(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleMML(string[] param)
        {
            if (param.Length != 1)
                throw new Exception("Bad param length, expected 1");
            return AssembleJMP((ushort)0x00BE, param[0]);
        }

        ushort[] AssembleMMS(string[] param)
        {
            if (param.Length != 1)
                throw new Exception("Bad param length, expected 1");
            return AssembleJMP((ushort)0x00BF, param[0]);
        }
        #endregion

        #region Jump
        ushort[] AssembleJMP(string[] param)
        {
            if (param.Length != 1)
                throw new Exception("Bad param length, expected 1");
            return AssembleJMP((ushort)0x00C0, param[0]);
        }

        ushort[] AssembleJSR(string[] param)
        {
            if (param.Length != 1)
                throw new Exception("Bad param length, expected 1");
            return AssembleJMP((ushort)0x00C1, param[0]);
        }

        ushort[] AssembleJUM(string[] param)
        {
            if (param.Length != 1)
                throw new Exception("Bad param length, expected 1");
            return AssembleJMP((ushort)0x00C2, param[0]);
        }

        ushort[] AssembleJCX(string[] param)
        {
            return new ushort[1] { (ushort)0x00C3 };
        }
        #endregion

        #region Processor Functions
        ushort[] AssembleHWQ(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleSLP(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleSWI(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }

        ushort[] AssembleRTI(string[] param)
        {
            return new ushort[1] { (ushort)c_NOP };
        }
        #endregion

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
