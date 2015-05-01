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
    public partial class Parser
    {
        private const ushort c_NOP = 0x0001; // LOD R0, R0
        public const int c_InstructionSize = 2;

        #region ALU
        ushort[] AssembleLOD(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[2] { 8, 16 });

            switch (bit_width)
            {
                case 8:
                    return AssembleAL8((ushort)0x00D0, param[0], param[1]);
                case 16:
                    return AssembleALU((ushort)0x0000, param[0], param[1]);
            }
            return null;
        }

        ushort[] AssembleSTO(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[2] { 8, 16 });

            ushort[] code = AssembleALU((ushort)0x0008, param[0], param[1]);
            int addressing = (code[0] & 0x0107);
            if ((addressing == 0) || (addressing == 1)) // no sto reg or sto immediate.
                return null;
            switch (bit_width)
            {
                case 8:
                    return AssembleAL8((ushort)0x00D8, param[0], param[1]);
                case 16:
                    return AssembleALU((ushort)0x0008, param[0], param[1]);
            }
            return null;
        }

        ushort[] AssembleADD(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0010, param[0], param[1]);
        }

        ushort[] AssembleSUB(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0018, param[0], param[1]);
        }

        ushort[] AssembleADC(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0020, param[0], param[1]);
        }

        ushort[] AssembleSBC(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0028, param[0], param[1]);
        }

        ushort[] AssembleMUL(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0030, param[0], param[1]);
        }

        ushort[] AssembleDIV(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0038, param[0], param[1]);
        }

        ushort[] AssembleMLI(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0040, param[0], param[1]);
        }

        ushort[] AssembleDVI(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0048, param[0], param[1]);
        }

        ushort[] AssembleMOD(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0050, param[0], param[1]);
        }

        ushort[] AssembleMDI(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0058, param[0], param[1]);
        }

        ushort[] AssembleAND(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0060, param[0], param[1]);
        }

        ushort[] AssembleORR(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0068, param[0], param[1]);
        }

        ushort[] AssembleEOR(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0070, param[0], param[1]);
        }

        ushort[] AssembleNOT(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0078, param[0], param[1]);
        }

        ushort[] AssembleCMP(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0080, param[0], param[1]);
        }

        ushort[] AssembleNEG(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleALU((ushort)0x0088, param[0], param[1]);
        }
        #endregion

        #region Branch operations
        ushort[] AssembleBCC(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBRA((ushort)0x0090, param[0]);
        }

        ushort[] AssembleBCS(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBRA((ushort)0x0091, param[0]);
        }

        ushort[] AssembleBNE(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBRA((ushort)0x0092, param[0]);
        }

        ushort[] AssembleBEQ(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBRA((ushort)0x0093, param[0]);
        }

        ushort[] AssembleBPL(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBRA((ushort)0x0094, param[0]);
        }

        ushort[] AssembleBMI(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBRA((ushort)0x0095, param[0]);
        }

        ushort[] AssembleBVC(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBRA((ushort)0x0096, param[0]);
        }

        ushort[] AssembleBVS(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBRA((ushort)0x0097, param[0]);
        }

        ushort[] AssembleBUG(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBRA((ushort)0x0098, param[0]);
        }

        ushort[] AssembleBSG(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBRA((ushort)0x0099, param[0]);
        }

        ushort[] AssembleBAW(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBRA((ushort)0x009F, param[0]);
        }
        #endregion

        #region Shift operations
        ushort[] AssembleASL(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleSHF((ushort)0x00A0, param[0], param[1]);
        }

        ushort[] AssembleLSL(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleSHF((ushort)0x00A1, param[0], param[1]);
        }

        ushort[] AssembleROL(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleSHF((ushort)0x00A2, param[0], param[1]);
        }

        ushort[] AssembleRNL(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleSHF((ushort)0x00A3, param[0], param[1]);
        }

        ushort[] AssembleASR(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleSHF((ushort)0x00A4, param[0], param[1]);
        }

        ushort[] AssembleLSR(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleSHF((ushort)0x00A5, param[0], param[1]);
        }

        ushort[] AssembleROR(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleSHF((ushort)0x00A6, param[0], param[1]);
        }

        ushort[] AssembleRNR(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleSHF((ushort)0x00A7, param[0], param[1]);
        }
        #endregion

        #region Bit testing operations
        ushort[] AssembleBIT(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBTT((ushort)0x00A8, param[0], param[1]);
        }

        ushort[] AssembleBTX(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBTT((ushort)0x00A9, param[0], param[1]);
        }

        ushort[] AssembleBTC(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBTT((ushort)0x00AA, param[0], param[1]);
        }

        ushort[] AssembleBTS(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleBTT((ushort)0x00AB, param[0], param[1]);
        }
        #endregion

        #region Switch Octet
        ushort[] AssembleSWO(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 3);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleSWO((ushort)0x00AC, param[0], param[1], param[2]);
        }
        #endregion

        #region Set flags
        ushort[] AssembleSEF(string[] param, int bit_width)
        {
            Sanity_RequireParamCountMinMax(param, 1, 4);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleFLG((ushort)0x00AE, param);
        }

        ushort[] AssembleCLF(string[] param, int bit_width)
        {
            Sanity_RequireParamCountMinMax(param, 1, 4);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleFLG((ushort)0x00AF, param);
        }
        #endregion

        #region Stack Push/Pop
        ushort[] AssemblePSH(string[] param, int bit_width)
        {
            Sanity_RequireParamCountMinMax(param, 1, 13);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleSTK(0xB0, param, false);
        }

        ushort[] AssemblePOP(string[] param, int bit_width)
        {
            Sanity_RequireParamCountMinMax(param, 1, 13);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleSTK(0xB2, param, true);
        }
        #endregion

        #region Inc/Dec
        ushort[] AssembleINC(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleIMM((ushort)0x00B8, param[0], 1.ToString());
        }

        ushort[] AssembleADI(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleIMM((ushort)0x00B8, param[0], param[1]);
        }

        ushort[] AssembleDEC(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleIMM((ushort)0x00B9, param[0], 1.ToString());
        }

        ushort[] AssembleSBI(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleIMM((ushort)0x00B9, param[0], param[1]);
        }
        #endregion

        #region Transfer registers
        ushort[] AssembleTSR(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            return AssembleXSR((ushort)0x00BA, param[0], param[1]);
        }

        ushort[] AssembleTRS(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            return AssembleXSR((ushort)0x00BB, param[0], param[1]);
        }
        #endregion

        #region MMU
        ushort[] AssembleMMR(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleMMU((ushort)0x00BC, param[0], param[1]);
        }

        ushort[] AssembleMMW(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 2);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleMMU((ushort)0x00BD, param[0], param[1]);
        }

        ushort[] AssembleMML(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleJMP((ushort)0x00BE, param[0]);
        }

        ushort[] AssembleMMS(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleJMP((ushort)0x00BF, param[0]);
        }
        #endregion

        #region Jump operations
        ushort[] AssembleJMP(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleJMP((ushort)0x00C0, param[0]);
        }

        ushort[] AssembleJSR(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleJMP((ushort)0x00C1, param[0]);
        }

        ushort[] AssembleJUM(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleJMP((ushort)0x00C2, param[0]);
        }

        ushort[] AssembleJCX(string[] param, int bit_width)
        {
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return new ushort[1] { (ushort)0x00C3 };
        }
        #endregion

        #region Processor Functions
        ushort[] AssembleHWQ(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 1);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssembleHWQ((ushort)0x00C4, param[0]);
        }

        ushort[] AssembleSLP(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 0);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return new ushort[1] { (ushort)0x00C5 };
        }

        ushort[] AssembleSWI(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 0);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return new ushort[1] { (ushort)0x00C6 };
        }

        ushort[] AssembleRTI(string[] param, int bit_width)
        {
            Sanity_RequireParamCountExact(param, 0);
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return new ushort[1] { (ushort)0x00C7 };
        }
        #endregion

        #region Macros
        ushort[] AssembleRTS(string[] param, int bit_width)
        {
            Sanity_RequireBitWidth(bit_width, new int[1] { 16 });
            return AssemblePOP(new string[1] { "PC" }, bit_width);
        }
        #endregion

        private List<ushort> m_Code = new List<ushort>();

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
                    // special case: we can optimize an immediate add or subtract to an adi or sbi instruction. Saves us the immediate word!
                    if (opcode == 0x0010 && p2.NextWord >= 1 && p2.NextWord <= 32) // add rx, ##.
                    {
                        return AssembleIMM((ushort)0x00B8, param1, param2);
                    }
                    else if (opcode == 0x0018 && p2.NextWord >= 1 && p2.NextWord <= 32) // sub rx, ##.
                    {
                        return AssembleIMM((ushort)0x00B9, param1, param2);
                    }
                    // special case: lod immediate should raise a warning...
                    else if (opcode == 0x00D0 && p2.NextWord >= 256)
                    {
                        throw new Exception("Error: 8-bit load operation with an immediate value of greater than 8 bits.");
                    }
                    else
                    {
                        addressingmode = 0x0000;
                    }
                    break;
                case AddressingMode.Absolute:
                    addressingmode = 0x0100;
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

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | addressingmode | ((p1.Word & 0x0007) << 13) | ((p2.Word & 0x0007) << 10)));
            if (p2.UsesNextWord)
            {
                if (p2.LabelName.Length > 0)
                    m_LabelReferences.Add((ushort)(m_MachineCodeOutput.Count + m_Code.Count * c_InstructionSize), p2.LabelName);
                m_Code.Add(p2.NextWord);
            }
            return m_Code.ToArray();
        }

        ushort[] AssembleAL8(ushort opcode, string param1, string param2)
        {
            return AssembleALU(opcode, param1, param2);
        }

        ushort[] AssembleBTT(ushort opcode, string param1, string param2)
        {
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);

            if (p1.Illegal || p2.Illegal)
                return null;

            // p1 MUST be a register.
            if (p1.AddressingMode != AddressingMode.Register)
                return null;

            // p2 MUST be EITHER an immediate value, OR a register.
            // if p2 is immediate, it MUST be 0 - 15.
            // if p2 is a register, it MUST be 0 - 7.
            switch (p2.AddressingMode)
            {
                case AddressingMode.Immediate:
                    if (p2.Word >= 16)
                        return null;
                    break;
                case AddressingMode.Register:
                    if (p2.Word >= 8)
                        return null;
                    break;
                default:
                    return null;
            }
            //  Bit pattern is:
            //  FEDC BA98 7654 3210 
            //  rrrR ssss OOOO OOOO
            //      r = p1
            //      R = select use of s.
            //      s = p2

            ushort r_bits = (ushort)((p1.Word & 0x0007) << 13);

            ushort s_bits = 0x0000;
            if (p2.AddressingMode == AddressingMode.Immediate)
                s_bits = (ushort)(p2.Word << 8);
            if (p2.AddressingMode == AddressingMode.Register)
                s_bits = (ushort)((0x1000) | (p2.Word << 8));

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | r_bits | s_bits));
            return m_Code.ToArray();
        }

        ushort[] AssembleBRA(ushort opcode, string param1)
        {
            ParsedOpcode p1 = ParseParam(param1);
            if (p1.Illegal)
                return null;
            // must be branching to a label
            if (p1.LabelName == string.Empty)
                return null;

            m_Code.Clear();
            m_Code.Add((ushort)opcode);
            m_BranchReferences.Add((ushort)m_MachineCodeOutput.Count, p1.LabelName);
            return m_Code.ToArray();
        }

        ushort[] AssembleFLG(ushort opcode, string[] param)
        {
            bool n = false, z = false, c = false, v = false;
            // there MUST be 1 - 4 params.
            // params MUST be one of: N, Z, C, V (case invariant)
            foreach (string p in param)
            {
                switch (p.ToLower())
                {
                    case "n":
                        n = true;
                        break;
                    case "z":
                        z = true;
                        break;
                    case "c":
                        c = true;
                        break;
                    case "v":
                        v = true;
                        break;
                    default:
                        throw new Exception("FLG instruction with non-existing flag.");
                }
            }

            ushort flags = (ushort)((n ? 0x8000 : 0x0000) | (z ? 0x4000 : 0x0000) | (c ? 0x2000 : 0x0000) | (v ? 0x1000 : 0x0000));

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | flags));
            return m_Code.ToArray();
        }

        ushort[] AssembleHWQ(ushort opcode, string param1)
        {
            // param1 = index of operations, must be integer from 0-255
            ParsedOpcode p1 = ParseParam(param1);

            if (p1.AddressingMode != AddressingMode.Immediate)
                return null;
            if ((p1.Word < 0) || (p1.Word > 255))
                return null;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // iiii iiii OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | ((p1.Word & 0x00FF) << 8)));
            return m_Code.ToArray();
        }

        ushort[] AssembleIMM(ushort opcode, string param1, string param2)
        {
            // param1 = source/dest register, MUST be register
            // param2 = immediate value, MUST be number, MUST be 1 - 32
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);

            if (p1.AddressingMode != AddressingMode.Register)
                return null;
            if (p2.AddressingMode != AddressingMode.Immediate)
                return null;
            if ((p2.NextWord < 1) || (p2.NextWord > 32))
                return null;

            p2.NextWord -= 1;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // RRRv vvvv OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | ((p1.Word & 0x0007) << 13) | ((p2.NextWord & 0x001F) << 8)));
            return m_Code.ToArray();
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
                case AddressingMode.Absolute:
                    addressingmode = 0x0100;
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
            m_Code.Clear();
            m_Code.Add((ushort)(opcode | addressingmode | ((p1.Word & 0x0007) << 13)));
            if (p1.UsesNextWord)
            {
                if (p1.LabelName.Length > 0)
                    m_LabelReferences.Add((ushort)(m_MachineCodeOutput.Count + m_Code.Count * c_InstructionSize), p1.LabelName);
                m_Code.Add(p1.NextWord);
            }
            return m_Code.ToArray();
        }

        ushort[] AssembleMMU(ushort opcode, string param1, string param2)
        {
            // param1 = source register, MUST be register
            // param2 = dest register, MUST be register
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);

            if (p1.AddressingMode != AddressingMode.Register)
                return null;
            if (p2.AddressingMode != AddressingMode.Register)
                return null;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // RRRr rr.. OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | ((p1.Word & 0x0007) << 13) | ((p2.Word & 0x0007) << 10)));
            return m_Code.ToArray();
        }

        ushort[] AssembleSHF(ushort opcode, string param1, string param2)
        {
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);

            if (p1.Illegal || p2.Illegal)
                return null;

            // p1 MUST be a register.
            if (p1.AddressingMode != AddressingMode.Register)
                return null;

            // p2 MUST be EITHER an immediate value, OR a register.
            // if p2 is immediate, it MUST be 0 - 15.
            // if p2 is a register, it MUST be 0 - 7.
            switch (p2.AddressingMode)
            {
                case AddressingMode.Immediate:
                    if (p2.Word >= 16)
                        return null;
                    break;
                case AddressingMode.Register:
                    if (p2.Word >= 8)
                        return null;
                    break;
                default:
                    return null;
            }
            //  Bit pattern is:
            //  FEDC BA98 7654 3210 
            //  rrrR ssss OOOO ODoo
            //      r = p1
            //      R = select use of s.
            //      s = p2

            ushort r_bits = (ushort)((p1.Word & 0x0007) << 13);

            ushort s_bits = 0x0000;
            if (p2.AddressingMode == AddressingMode.Immediate)
                s_bits = (ushort)(p2.Word << 8);
            if (p2.AddressingMode == AddressingMode.Register)
                s_bits = (ushort)((0x1000) | (p2.Word << 8));

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | r_bits | s_bits));
            return m_Code.ToArray();
        }

        ushort[] AssembleSTK(ushort opcode, string[] param, bool general_first)
        {
            ushort flags0 = 0x0000, flags1 = 0x0000;
            // there MUST be 1 - 10 params.
            // params MUST be one of:   R0, R1, R2, R3, R4, R5, R6, R7
            //                          A, B, C, I, J, X, Y, Z
            //                          FL, PC, PS, SP, and USP
            foreach (string p in param)
            {
                switch (p.ToLower())
                {
                    case "sp":
                        flags1 |= 0x0101;
                        break;
                    case "usp":
                        flags1 |= 0x0101;
                        break;
                    case "ps":
                        flags1 |= 0x0401;
                        break;
                    case "pc":
                        flags1 |= 0x0801;
                        break;
                    case "fl":
                        flags1 |= 0x1002;
                        break;
                    case "r0":
                    case "a":
                        flags0 |= 0x0100;
                        break;
                    case "r1":
                    case "b":
                        flags0 |= 0x0200;
                        break;
                    case "r2":
                    case "c":
                        flags0 |= 0x0400;
                        break;
                    case "r3":
                    case "i":
                        flags0 |= 0x0800;
                        break;
                    case "r4":
                    case "j":
                        flags0 |= 0x1000;
                        break;
                    case "r5":
                    case "x":
                        flags0 |= 0x2000;
                        break;
                    case "r6":
                    case "y":
                        flags0 |= 0x4000;
                        break;
                    case "r7":
                    case "z":
                        flags0 |= 0x8000;
                        break;
                    default:
                        throw new Exception("STK instruction with non-existing register.");
                }
            }

            m_Code.Clear();

            if (general_first && (flags0 != 0))
                m_Code.Add((ushort)(opcode | flags0));
            if (flags1 != 0)
                m_Code.Add((ushort)(opcode | flags1));
            if (!general_first && (flags0 != 0))
                m_Code.Add((ushort)(opcode | flags0));

            return m_Code.ToArray();
        }

        ushort[] AssembleSWO(ushort opcode, string param1, string param2, string param3)
        {
            // param1 = source register, MUST be register
            // param2 = dest register, MUST be register
            // param3 = flags, MUST be LR, HR, LW, HW
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);
            ushort flag_type = 0x0000;
            switch (param3)
            {
                case "LR":
                    flag_type = 0x0000;
                    break;
                case "HR":
                    flag_type = 0x0001;
                    break;
                case "LW":
                    flag_type = 0x0002;
                    break;
                case "HW":
                    flag_type = 0x0003;
                    break;
                default:
                    throw new Exception("Bad SWO flag.");
            }

            if (p1.AddressingMode != AddressingMode.Register)
                return null;
            if (p2.AddressingMode != AddressingMode.Register)
                return null;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // RRRr rroo OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | ((p1.Word & 0x0007) << 13) | ((p2.Word & 0x0007) << 10) | (flag_type << 8)));
            return m_Code.ToArray();
        }

        ushort[] AssembleXSR(ushort opcode, string param1, string param2)
        {
            // param1 = source/dest register, MUST be register
            // param2 = special register, MUST be one of:
            //          PC, SP, IA, II, PS, P2, USP, SSP
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);
            ushort special_index = 0x0000;
            switch (param2)
            {
                case "PC":
                    special_index = 0x0000;
                    break;
                case "SP":
                    special_index = 0x0001;
                    break;
                case "IA":
                    special_index = 0x0002;
                    break;
                case "II":
                    special_index = 0x0003;
                    break;
                case "PS":
                    special_index = 0x0004;
                    break;
                case "P2":
                    special_index = 0x0005;
                    break;
                case "USP":
                    special_index = 0x0006;
                    break;
                case "SSP":
                    special_index = 0x0007;
                    break;
                default:
                    throw new Exception("Unknown XSR register.");
            }

            if (p1.AddressingMode != AddressingMode.Register)
                return null;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // RRRv vvvv OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | ((p1.Word & 0x0007) << 13) | (special_index << 8)));
            return m_Code.ToArray();
        }

        #region Sanity
        private void Sanity_RequireParamCountExact(string[] param, int count)
        {
            if (param.Length != count)
                throw new Exception(string.Format("Bad param count, expected {0}.", count));
        }

        private void Sanity_RequireParamCountMinMax(string[] param, int min, int max)
        {
            if ((param.Length < min) || (param.Length > max))
                throw new Exception(string.Format("Bad param count, expected {0}-{1}.", min, max));
        }

        private void Sanity_RequireBitWidth(int bit_width, int[] param)
        {
            for (int i = 0; i < param.Length; i++)
                if (param[i] == bit_width)
                    return;
            throw new Exception(string.Format("Bit width of '{0}' is unsupported for this opcode.", bit_width));
        }
        #endregion
    }
}
