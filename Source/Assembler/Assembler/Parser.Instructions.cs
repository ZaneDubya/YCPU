/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;
using System.Collections.Generic;

namespace Ypsilon.Assembler
{
    public partial class Parser
    {
        public const int c_InstructionSize = 2;

        OpcodeFlag[] flag8or16 = new OpcodeFlag[] { OpcodeFlag.BitWidth8, OpcodeFlag.BitWidth16 };

        #region ALU
        ushort[] AssembleCMP(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0000, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleNEG(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0008, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleADD(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0010, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleSUB(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0018, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleADC(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0020, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleSBC(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0028, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleMUL(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0030, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleDIV(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0038, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleMLI(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0040, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleDVI(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0048, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleMOD(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0050, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleMDI(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0058, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleAND(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0060, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleORR(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0068, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleEOR(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0070, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleNOT(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, flag8or16);
            return AssembleALU((ushort)0x0078, param[0], param[1], opcodeFlag, state);
        }


        ushort[] AssembleLOD(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth8, OpcodeFlag.BitWidth16 });

            return AssembleALU((ushort)0x0080, param[0], param[1], opcodeFlag, state);
        }

        ushort[] AssembleSTO(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth8, OpcodeFlag.BitWidth16 });

            ushort[] code = AssembleALU((ushort)0x0088, param[0], param[1], opcodeFlag, state);
            int addressing = (code[0] & 0x0F00);
            if ((addressing == 0x0000) || (addressing == 0x0200)) // no sto reg or sto immediate.
                return null;
            return code;
        }
        #endregion

        #region Branch operations
        ushort[] AssembleBCC(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBRA((ushort)0x0090, param[0], state);
        }

        ushort[] AssembleBCS(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBRA((ushort)0x0091, param[0], state);
        }

        ushort[] AssembleBNE(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBRA((ushort)0x0092, param[0], state);
        }

        ushort[] AssembleBEQ(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBRA((ushort)0x0093, param[0], state);
        }

        ushort[] AssembleBPL(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBRA((ushort)0x0094, param[0], state);
        }

        ushort[] AssembleBMI(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBRA((ushort)0x0095, param[0], state);
        }

        ushort[] AssembleBVC(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBRA((ushort)0x0096, param[0], state);
        }

        ushort[] AssembleBVS(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBRA((ushort)0x0097, param[0], state);
        }

        ushort[] AssembleBUG(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBRA((ushort)0x0098, param[0], state);
        }

        ushort[] AssembleBSG(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBRA((ushort)0x0099, param[0], state);
        }

        ushort[] AssembleBAW(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBRA((ushort)0x009F, param[0], state);
        }
        #endregion

        #region Shift operations
        ushort[] AssembleASL(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleSHF((ushort)0x00A0, param[0], param[1]);
        }

        ushort[] AssembleLSL(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleSHF((ushort)0x00A1, param[0], param[1]);
        }

        ushort[] AssembleROL(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleSHF((ushort)0x00A2, param[0], param[1]);
        }

        ushort[] AssembleRNL(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleSHF((ushort)0x00A3, param[0], param[1]);
        }

        ushort[] AssembleASR(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleSHF((ushort)0x00A4, param[0], param[1]);
        }

        ushort[] AssembleLSR(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleSHF((ushort)0x00A5, param[0], param[1]);
        }

        ushort[] AssembleROR(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleSHF((ushort)0x00A6, param[0], param[1]);
        }

        ushort[] AssembleRNR(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleSHF((ushort)0x00A7, param[0], param[1]);
        }
        #endregion

        #region Bit testing operations
        ushort[] AssembleBTT(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBTT((ushort)0x00A8, param[0], param[1]);
        }

        ushort[] AssembleBTX(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBTT((ushort)0x00A9, param[0], param[1]);
        }

        ushort[] AssembleBTC(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBTT((ushort)0x00AA, param[0], param[1]);
        }

        ushort[] AssembleBTS(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleBTT((ushort)0x00AB, param[0], param[1]);
        }
        #endregion

        // FPU instructions would go here. Not currently implemented.

        #region Set flags
        ushort[] AssembleSEF(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountMinMax(param, 1, 4);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleFLG((ushort)0x00AE, param);
        }

        ushort[] AssembleCLF(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountMinMax(param, 1, 4);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleFLG((ushort)0x00AF, param);
        }
        #endregion

        #region Stack Push/Pop/Flush
        ushort[] AssemblePSH(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountMinMax(param, 1, 13);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleSTK(0x00B0, param, false);
        }

        ushort[] AssemblePOP(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountMinMax(param, 1, 13);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleSTK(0x00B2, param, true);
        }

        ushort[] AssembleSFL(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            // base opcode is 0x00B4, high byte is number of stack inst to flush, + 1
            return AssembleSFL(0x00B4, param[0]);
        }
        #endregion

        #region MMU
        ushort[] AssembleMMR(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleMMU((ushort)0x00B5, param[0], param[1]);
        }

        ushort[] AssembleMMW(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleMMU((ushort)0x01B5, param[0], param[1]);
        }

        ushort[] AssembleMML(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleJMP((ushort)0x02B5, param[0], state);
        }

        ushort[] AssembleMMS(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleJMP((ushort)0x03B5, param[0], state);
        }
        #endregion

        ushort[] AssembleSET(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            return AssembleSET((ushort)0x00B6, param[0], param[1]);
        }

        #region Inc/Dec
        ushort[] AssembleINC(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleIMM((ushort)0x00B8, param[0], 1.ToString());
        }

        ushort[] AssembleADI(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleIMM((ushort)0x00B8, param[0], param[1]);
        }

        ushort[] AssembleDEC(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleIMM((ushort)0x00B9, param[0], 1.ToString());
        }

        ushort[] AssembleSBI(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleIMM((ushort)0x00B9, param[0], param[1]);
        }
        #endregion

        #region Processor Functions
        ushort[] AssembleHWQ(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleHWQ((ushort)0x00BC, param[0]);
        }

        ushort[] AssembleSLP(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 0);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return new ushort[1] { (ushort)0x00BD };
        }

        ushort[] AssembleSWI(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 0);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return new ushort[1] { (ushort)0x00BE };
        }

        ushort[] AssembleRTI(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 0);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return new ushort[1] { (ushort)0x00BF };
        }
        #endregion

        #region Jump operations
        ushort[] AssembleJMP(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleJMP((ushort)0x00BA, param[0], state);
        }

        ushort[] AssembleJSR(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssembleJMP((ushort)0x00BB, param[0], state);
        }
        #endregion

        

        

        #region Macros: RTS, NOP
        ushort[] AssembleRTS(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return AssemblePOP(new string[1] { "PC" }, opcodeFlag, state);
        }

        ushort[] AssembleNOP(string[] param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 0);
            return new ushort[] { (ushort)0x0280 }; // LOD R0, R0
        }
        #endregion

        private List<ushort> m_Code = new List<ushort>();

        ushort[] AssembleALU(ushort opcode, string param1, string param2, OpcodeFlag opcodeFlag, ParserState state)
        {
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);

            if (p1.AddressingMode != AddressingMode.Register)
                return null;
            
            ushort addressingmode = 0x0000;
            switch (p2.AddressingMode)
            {
                case AddressingMode.None:
                    return null;
                case AddressingMode.Immediate:
                    // special case: alu.8 immediate with value greater than $FF should raise a warning...
                    if (opcodeFlag == OpcodeFlag.BitWidth8 && p2.ImmediateWord >= 256)
                    {
                        throw new Exception("8-bit load operation with an immediate value of greater than 8 bits.");
                    }
                    addressingmode = 0x0000;
                    break;
                case AddressingMode.Absolute:
                    addressingmode = 0x0100;
                    break;
                case AddressingMode.Register:
                    addressingmode = 0x0200;
                    break;
                case AddressingMode.Indirect:
                    addressingmode = 0x0300;
                    break;
                case AddressingMode.IndirectOffset:
                    addressingmode = 0x0400;
                    break;
                case AddressingMode.StackAccess:
                    addressingmode = 0x0500;
                    break;
                case AddressingMode.IndirectPostInc:
                    addressingmode = 0x0600;
                    break;
                case AddressingMode.IndirectPreDec:
                    addressingmode = 0x0700;
                    break;
                case AddressingMode.IndirectIndexed:
                    int r3 = (p2.OpcodeWord & 0x0700);
                    addressingmode = (ushort)(0x0800 | r3);
                    break;
                default:
                    throw new Exception("Unknown addressing mode.");
            }

            ushort bitwidth = 0x0000;
            if (opcodeFlag == OpcodeFlag.BitWidth8)
                bitwidth = 0x1000;

            // FEDC BA98 7654 3210                             
            // rrrE AAAA OOOO ORRR
            m_Code.Clear();
            m_Code.Add((ushort)(opcode | addressingmode | bitwidth | (p1.OpcodeWord & 0x0007) | ((p2.OpcodeWord & 0x0007) << 13)));
            if (p2.HasImmediateWord)
            {
                if (p2.LabelName.Length > 0)
                    state.Labels.Add((ushort)(state.Code.Count + m_Code.Count * c_InstructionSize), p2.LabelName);
                m_Code.Add(p2.ImmediateWord);
            }
            return m_Code.ToArray();
        }

        ushort[] AssembleBTT(ushort opcode, string param1, string param2)
        {
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);

            // p1 MUST be a register.
            if (p1.AddressingMode != AddressingMode.Register)
                return null;

            // p2 MUST be EITHER an immediate value, OR a register.
            // if p2 is immediate, it MUST be 0 - 15.
            // if p2 is a register, it MUST be 0 - 7.
            switch (p2.AddressingMode)
            {
                case AddressingMode.Immediate:
                    if (p2.OpcodeWord >= 16)
                        return null;
                    break;
                case AddressingMode.Register:
                    if (p2.OpcodeWord >= 8)
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

            ushort r_bits = (ushort)((p1.OpcodeWord & 0x0007) << 13);

            ushort s_bits = 0x0000;
            if (p2.AddressingMode == AddressingMode.Immediate)
                s_bits = (ushort)(p2.OpcodeWord << 8);
            if (p2.AddressingMode == AddressingMode.Register)
                s_bits = (ushort)((0x1000) | (p2.OpcodeWord << 8));

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | r_bits | s_bits));
            return m_Code.ToArray();
        }

        ushort[] AssembleBRA(ushort opcode, string param1, ParserState state)
        {
            ParsedOpcode p1 = ParseParam(param1);
            // must be branching to a label or an immediate value between $7f and -$80
            if (p1.LabelName == string.Empty)
                if (p1.ImmediateWord > 255)
                    return null;

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | (p1.ImmediateWord << 8)));
            if (p1.LabelName != string.Empty)
                state.Branches.Add((ushort)state.Code.Count, p1.LabelName.ToLower());
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
            if ((p1.OpcodeWord < 0) || (p1.OpcodeWord > 255))
                return null;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // iiii iiii OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | ((p1.ImmediateWord & 0x00FF) << 8)));
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
            if ((p2.ImmediateWord < 1) || (p2.ImmediateWord > 32))
                return null;

            p2.ImmediateWord -= 1;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // RRRv vvvv OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | ((p1.OpcodeWord & 0x0007) << 13) | ((p2.ImmediateWord & 0x001F) << 8)));
            return m_Code.ToArray();
        }

        ushort[] AssembleJMP(ushort opcode, string param1, ParserState state)
        {
            ParsedOpcode p1 = ParseParam(param1);

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
                    int r3 = (p1.OpcodeWord & 0x0700);
                    addressingmode = (ushort)(0xC000 + (r3 & 0x0300) + ((r3 & 0x0400) << 3));
                    break;
            }
            m_Code.Clear();
            m_Code.Add((ushort)(opcode | addressingmode | ((p1.OpcodeWord & 0x0007) << 13)));
            if (p1.HasImmediateWord)
            {
                if (p1.LabelName.Length > 0)
                    state.Labels.Add((ushort)(state.Code.Count + m_Code.Count * c_InstructionSize), p1.LabelName);
                m_Code.Add(p1.ImmediateWord);
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
            m_Code.Add((ushort)(opcode | ((p1.OpcodeWord & 0x0007) << 10) | ((p2.OpcodeWord & 0x0007) << 13)));
            return m_Code.ToArray();
        }

        ushort[] AssembleSET(ushort opcode, string param1, string param2)
        {
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);

            // p1 MUST be a register.
            if (p1.AddressingMode != AddressingMode.Register)
                return null;

            // p2 must be a value, and there is a limited range of allowed values.
            if (p2.AddressingMode != AddressingMode.Immediate)
                return null;

            ushort alternateValueBit, value;
            bool valueIsAllowed = TryGetSETValue(p2, out value, out alternateValueBit);

            if (!valueIsAllowed)
                throw new Exception(string.Format("SET instruction with invalid value parameter: {0}.", p2.ImmediateWord));

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | alternateValueBit | (value << 8) | ((p1.OpcodeWord & 0x0007) << 13)));
            return m_Code.ToArray();
        }

        bool IsAcceptableSETValue(ParsedOpcode input)
        {
            ushort alternateValueBit, value;
            return TryGetSETValue(input, out value, out alternateValueBit);
        }

        bool TryGetSETValue(ParsedOpcode input, out ushort value, out ushort alternateValueBit)
        {
            alternateValueBit = 0;
            value = 0;

            if (input.HasImmediateWord && input.LabelName.Length > 0)
            {
                // not allowed to use labels with SET instructions.
                return false;
            }

            if (input.ImmediateWord <= 0x1F)
            {
                value = input.ImmediateWord;
                return true;
            }
            else if (input.ImmediateWord >= 0xFFEB)
            {
                value = (ushort)((input.ImmediateWord - 0xFFEB) + 0x000B);
                alternateValueBit = 1;
                return true;
            }
            else
            {
                for (int i = 0x05; i < 0x10; i++)
                {
                    int allowedValue = (ushort)(Math.Pow(2, i));
                    if (input.ImmediateWord == allowedValue)
                    {
                        value = (ushort)(i - 5);
                        alternateValueBit = 1;
                        return true;
                    }
                }
            }
            return false;
        }

        ushort[] AssembleSHF(ushort opcode, string param1, string param2)
        {
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);


            // p1 MUST be a register.
            if (p1.AddressingMode != AddressingMode.Register)
                return null;

            // p2 MUST be EITHER an immediate value, OR a register.
            // if p2 is immediate, it MUST be 1 - 16.
            // if p2 is a register, it MUST be 0 - 7.
            switch (p2.AddressingMode)
            {
                case AddressingMode.Immediate:
                    if (p2.ImmediateWord > 16 || p2.ImmediateWord == 0)
                        return null;
                    break;
                case AddressingMode.Register:
                    if (p2.OpcodeWord >= 8)
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

            ushort r_bits = (ushort)((p1.OpcodeWord & 0x0007) << 13);

            ushort s_bits = 0x0000;
            if (p2.AddressingMode == AddressingMode.Immediate)
                s_bits = (ushort)((p2.ImmediateWord - 1) << 8);
            if (p2.AddressingMode == AddressingMode.Register)
                s_bits = (ushort)((0x1000) | (p2.OpcodeWord << 8));

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | r_bits | s_bits));
            return m_Code.ToArray();
        }

        ushort[] AssembleSFL(ushort opcode, string param1)
        {
            // param1 = index of operations, must be integer from 1-256
            ParsedOpcode p1 = ParseParam(param1);

            if (p1.AddressingMode != AddressingMode.Immediate)
                return null;
            if ((p1.ImmediateWord < 1) || (p1.ImmediateWord > 256))
                return null;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // iiii iiii OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | (((p1.ImmediateWord - 1) & 0x00FF) << 8)));
            return m_Code.ToArray();
        }

        ushort[] AssembleSTK(ushort opcode, string[] param, bool general_first)
        {
            ushort flags0 = 0x0000, flags1 = 0x0000;
            // there MUST be 1 - 10 params.
            // params MUST be one of:   R0, R1, R2, R3, R4, R5, R6, R7
            //                          A, B, C, I, J, X, Y, Z
            //                          FL, PC, PS, SP, USP, II, IA, P2
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
                        flags1 |= 0x1001;
                        break;
                    case "p2":
                        flags1 |= 0x2001;
                        break;
                    case "ii":
                        flags1 |= 0x4001;
                        break;
                    case "ia":
                        flags1 |= 0x8001;
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
            m_Code.Add((ushort)(opcode | ((p1.OpcodeWord & 0x0007) << 13) | ((p2.OpcodeWord & 0x0007) << 10) | (flag_type << 8)));
            return m_Code.ToArray();
        }
    }
}
