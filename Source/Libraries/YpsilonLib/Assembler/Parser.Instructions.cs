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
        List<ushort> AssembleCMP(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0000, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleNEG(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0008, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleADD(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0010, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleSUB(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0018, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleADC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0020, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleSBC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0028, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleMUL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0030, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleDIV(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0038, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleMLI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0040, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleDVI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0048, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleMOD(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0050, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleMDI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0058, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleAND(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0060, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleORR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0068, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleEOR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0070, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleNOT(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU((ushort)0x0078, param[0], param[1], opcodeFlag, state);
        }


        List<ushort> AssembleLOD(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth8, OpcodeFlag.BitWidth16 });

            return state.Parser.AssembleALU((ushort)0x0080, param[0], param[1], opcodeFlag, state);
        }

        List<ushort> AssembleSTO(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth8, OpcodeFlag.BitWidth16 });

            List<ushort> code = state.Parser.AssembleALU((ushort)0x0088, param[0], param[1], opcodeFlag, state);
            if ((code[0] & 0xFE00) == 0x0000) // no sto immediate
                throw new Exception("Store register instructions not supported.");
            else if ((code[0] & 0xF000) == 0x2000) // no sto immediate.
                throw new Exception("Store immediate instructions not supported.");
            return code;
        }
        #endregion

        #region Branch operations
        List<ushort> AssembleBCC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA((ushort)0x0090, param[0], state);
        }

        List<ushort> AssembleBCS(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA((ushort)0x0091, param[0], state);
        }

        List<ushort> AssembleBNE(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA((ushort)0x0092, param[0], state);
        }

        List<ushort> AssembleBEQ(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA((ushort)0x0093, param[0], state);
        }

        List<ushort> AssembleBPL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA((ushort)0x0094, param[0], state);
        }

        List<ushort> AssembleBMI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA((ushort)0x0095, param[0], state);
        }

        List<ushort> AssembleBVC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA((ushort)0x0096, param[0], state);
        }

        List<ushort> AssembleBVS(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA((ushort)0x0097, param[0], state);
        }

        List<ushort> AssembleBUG(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA((ushort)0x0098, param[0], state);
        }

        List<ushort> AssembleBSG(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA((ushort)0x0099, param[0], state);
        }

        List<ushort> AssembleBAW(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA((ushort)0x009F, param[0], state);
        }
        #endregion

        #region Shift operations
        List<ushort> AssembleASL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF((ushort)0x00A0, param[0], param[1]);
        }

        List<ushort> AssembleLSL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF((ushort)0x00A1, param[0], param[1]);
        }

        List<ushort> AssembleROL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF((ushort)0x00A2, param[0], param[1]);
        }

        List<ushort> AssembleRNL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF((ushort)0x00A3, param[0], param[1]);
        }

        List<ushort> AssembleASR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF((ushort)0x00A4, param[0], param[1]);
        }

        List<ushort> AssembleLSR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF((ushort)0x00A5, param[0], param[1]);
        }

        List<ushort> AssembleROR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF((ushort)0x00A6, param[0], param[1]);
        }

        List<ushort> AssembleRNR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF((ushort)0x00A7, param[0], param[1]);
        }
        #endregion

        #region Bit testing operations
        List<ushort> AssembleBTT(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBTI((ushort)0x00A8, param[0], param[1]);
        }

        List<ushort> AssembleBTX(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBTI((ushort)0x00A9, param[0], param[1]);
        }

        List<ushort> AssembleBTC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBTI((ushort)0x00AA, param[0], param[1]);
        }

        List<ushort> AssembleBTS(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBTI((ushort)0x00AB, param[0], param[1]);
        }
        #endregion

        #region Set flags
        List<ushort> AssembleSEF(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountMinMax(param, 1, 4);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleFLG((ushort)0x00AE, param);
        }

        List<ushort> AssembleCLF(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountMinMax(param, 1, 4);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleFLG((ushort)0x00AF, param);
        }
        #endregion

        #region Stack Push/Pop/Flush
        List<ushort> AssemblePSH(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountMinMax(param, 1, 13);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSTK(0x00B0, param, false);
        }

        List<ushort> AssemblePOP(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountMinMax(param, 1, 13);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSTK(0x00B2, param, true);
        }
        #endregion

        #region MMU
        List<ushort> AssembleMMR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleMMU((ushort)0x00B5, param[0], param[1], state);
        }

        List<ushort> AssembleMMW(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleMMU((ushort)0x01B5, param[0], param[1], state);
        }

        List<ushort> AssembleMML(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleMMU((ushort)0x02B5, param[0], null, state, true);
        }

        List<ushort> AssembleMMS(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleMMU((ushort)0x03B5, param[0], null, state, true);
        }
        #endregion

        List<ushort> AssembleSET(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            return state.Parser.AssembleSEI((ushort)0x00B6, param[0], param[1]);
        }

        #region Inc/Dec
        List<ushort> AssembleINC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleIMM((ushort)0x00B8, param[0], 1.ToString());
        }

        List<ushort> AssembleADI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleIMM((ushort)0x00B8, param[0], param[1]);
        }

        List<ushort> AssembleDEC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleIMM((ushort)0x00B9, param[0], 1.ToString());
        }

        List<ushort> AssembleSBI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 2);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleIMM((ushort)0x00B9, param[0], param[1]);
        }
        #endregion

        #region Processor Functions
        List<ushort> AssembleHWQ(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 1);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleHWI((ushort)0x00BC, param[0]);
        }

        List<ushort> AssembleSLP(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 0);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return new List<ushort>() { (ushort)0x00BD };
        }

        List<ushort> AssembleSWI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 0);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return new List<ushort>() { (ushort)0x00BE };
        }

        List<ushort> AssembleRTI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 0);
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return new List<ushort>() { (ushort)0x00BF };
        }
        #endregion

        #region Jump operations
        List<ushort> AssembleJMP(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            if (opcodeFlag.HasFlag(OpcodeFlag.FarJump))
            {
                Sanity.RequireParamCountMinMax(param, 1, 2);
                return state.Parser.AssembleJMI((ushort)0x01BA, param[0], param.Count == 2 ? param[1] : null, state);
            }
            else
            {
                Sanity.RequireParamCountExact(param, 1);
                return state.Parser.AssembleJMI((ushort)0x00BA, param[0], null, state);
            }
        }

        List<ushort> AssembleJSR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            if (opcodeFlag.HasFlag(OpcodeFlag.FarJump))
            {
                Sanity.RequireParamCountMinMax(param, 1, 2); // allow two params for immediate far jump.
                return state.Parser.AssembleJMI((ushort)0x01BB, param[0], param.Count == 2 ? param[1] : null, state);
            }
            else
            {
                Sanity.RequireParamCountExact(param, 1);
                return state.Parser.AssembleJMI((ushort)0x00BB, param[0], null, state);
            }
        }
        #endregion

        #region Macros: RTS, NOP
        List<ushort> AssembleRTS(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireOpcodeFlag(opcodeFlag, new OpcodeFlag[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssemblePOP(new List<string>() { "PC" }, opcodeFlag, state);
        }

        List<ushort> AssembleNOP(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Sanity.RequireParamCountExact(param, 0);
            return new List<ushort>() { (ushort)0x2080 }; // LOD R0, R0
        }
        #endregion

        private List<ushort> m_Code = new List<ushort>();

        List<ushort> AssembleALU(ushort opcode, string param1, string param2, OpcodeFlag opcodeFlag, ParserState state)
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
                    addressingmode = 0x0200;
                    break;
                case AddressingMode.StatusRegister:
                    // can't use eightbit mode with proc regs...
                    if (opcodeFlag.HasFlag(OpcodeFlag.BitWidth8))
                        throw new Exception("ALU instructions with status register operands do not support 8-bit mode.");
                    addressingmode = 0x1000;
                    break;
                case AddressingMode.Register:
                    addressingmode = 0x2000;
                    break;
                case AddressingMode.Indirect:
                    addressingmode = 0x3000;
                    break;
                case AddressingMode.IndirectOffset:
                    addressingmode = 0x4000;
                    break;
                case AddressingMode.StackAccess:
                    addressingmode = 0x5000;
                    break;
                case AddressingMode.IndirectPostInc:
                    addressingmode = 0x6000;
                    break;
                case AddressingMode.IndirectPreDec:
                    addressingmode = 0x7000;
                    break;
                case AddressingMode.IndirectIndexed:
                    int index_register = (p2.OpcodeWord & 0x0700) << 4;
                    addressingmode = (ushort)(0x8000 | index_register);
                    break;
                default:
                    throw new Exception("Unknown addressing mode.");
            }

            ushort bitwidth = 0x0000;
            if (opcodeFlag == OpcodeFlag.BitWidth8)
                bitwidth = 0x0100;

            // FEDC BA98 7654 3210                             
            // AAAA rrrE OOOO ORRR
            m_Code.Clear();
            m_Code.Add((ushort)(opcode | addressingmode | bitwidth | (p1.OpcodeWord & 0x0007) | ((p2.OpcodeWord & 0x0007) << 9)));
            if (p2.HasImmediateWord)
            {
                if (p2.LabelName.Length > 0)
                    state.Labels.Add((ushort)(state.Code.Count + m_Code.Count * c_InstructionSize), p2.LabelName);
                m_Code.Add(p2.ImmediateWord);
            }
            return m_Code;
        }

        List<ushort> AssembleBTI(ushort opcode, string param1, string param2)
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
                s_bits = (ushort)(p2.ImmediateWord << 8);
            else if (p2.AddressingMode == AddressingMode.Register)
                s_bits = (ushort)((0x1000) | (p2.OpcodeWord << 8));

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | r_bits | s_bits));
            return m_Code;
        }

        List<ushort> AssembleBRA(ushort opcode, string param1, ParserState state)
        {
            ParsedOpcode p1 = ParseParam(param1);
            // must be branching to a label or an immediate value between $7f and -$80
            if (p1.LabelName == string.Empty)
                if (p1.ImmediateWord > 511)
                    return null;

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | (p1.ImmediateWord << 8)));
            if (p1.LabelName != string.Empty)
                state.Branches.Add((ushort)state.Code.Count, p1.LabelName.ToLowerInvariant());
            return m_Code;
        }

        List<ushort> AssembleFLG(ushort opcode, List<string> param)
        {
            bool n = false, z = false, c = false, v = false;
            // there MUST be 1 - 4 params.
            // params MUST be one of: N, Z, C, V (case invariant)
            for (int i = 0; i < param.Count; i++)
            {
                string p = param[i];
                switch (p.ToLowerInvariant())
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
            return m_Code;
        }

        List<ushort> AssembleHWI(ushort opcode, string param1)
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
            return m_Code;
        }

        List<ushort> AssembleIMM(ushort opcode, string param1, string param2)
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
            return m_Code;
        }

        List<ushort> AssembleJMI(ushort opcode, string param1, string param2, ParserState state)
        {
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);
            // sanity check - immediate jump must have two words, all others must have one.
            if (p1.AddressingMode == AddressingMode.Immediate && ((opcode & 0x0100) == 0x0100))
            {
                if (p2 == null)
                    throw new Exception("Immediate far jumps require two immedate words as parameters.");
            }
            else
            {
                if (p2 != null)
                    throw new Exception("Bad number of parameters. Expected 1.");
            }


            ushort addressingmode = 0x0000;
            switch (p1.AddressingMode)
            {
                case AddressingMode.None:
                    return null;
                case AddressingMode.Immediate:
                    addressingmode = 0x0000;
                    break;
                case AddressingMode.Absolute:
                    addressingmode = 0x0200;
                    break;
                case AddressingMode.Register:
                    addressingmode = 0x2000;
                    break;
                case AddressingMode.Indirect:
                    addressingmode = 0x3000;
                    break;
                case AddressingMode.IndirectOffset:
                    addressingmode = 0x4000;
                    break;
                case AddressingMode.StackAccess:
                    addressingmode = 0x5000;
                    break;
                case AddressingMode.IndirectPostInc:
                    addressingmode = 0x8000;
                    break;
                case AddressingMode.IndirectPreDec:
                    addressingmode = 0xA000;
                    break;
                case AddressingMode.IndirectIndexed:
                    int index_register = (p1.OpcodeWord & 0x0700) << 4;
                    addressingmode = (ushort)(0x8000 | index_register);
                    break;
                default:
                    throw new Exception("Addressing mode not usable with this instruction.");
            }
            m_Code.Clear();
            m_Code.Add((ushort)(opcode | addressingmode | ((p1.OpcodeWord & 0x0007) << 9)));
            if (p1.HasImmediateWord)
            {
                if (p1.LabelName.Length > 0)
                    state.Labels.Add((ushort)(state.Code.Count + m_Code.Count * c_InstructionSize), p1.LabelName);
                m_Code.Add(p1.ImmediateWord);
            }
            if (p2 != null && p2.HasImmediateWord)
            {
                if (p2.LabelName.Length > 0)
                    state.Labels.Add((ushort)(state.Code.Count + m_Code.Count * c_InstructionSize), p2.LabelName);
                m_Code.Add(p2.ImmediateWord);
            }
            return m_Code;
        }

        List<ushort> AssembleMMU(ushort opcode, string param1, string param2, ParserState state, bool singleParam = false)
        {
            // param1 = source register, MUST be register
            // param2 = dest register, MUST be register OR null
            ParsedOpcode p1 = ParseParam(param1);
            ParsedOpcode p2 = ParseParam(param2);

            if (p1 == null || p1.AddressingMode != AddressingMode.Register)
                return null;
            if (!(singleParam &&  p2 == null) && p2.AddressingMode != AddressingMode.Register)
                return null;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // RRRr rroo OOOO OOOO

            m_Code.Clear();
            if (singleParam)
            {
                m_Code.Add((ushort)(opcode | ((p1.OpcodeWord & 0x0007) << 13)));
            }
            else
            {
                m_Code.Add((ushort)(opcode | ((p1.OpcodeWord & 0x0007) << 10) | ((p2.OpcodeWord & 0x0007) << 13)));
            }
            return m_Code;
        }

        List<ushort> AssembleSEI(ushort opcode, string param1, string param2)
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
            return m_Code;
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

        List<ushort> AssembleSHF(ushort opcode, string param1, string param2)
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
            return m_Code;
        }

        List<ushort> AssembleSFL(ushort opcode, string param1)
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
            return m_Code;
        }

        List<ushort> AssembleSTK(ushort opcode, List<string> param, bool general_first)
        {
            ushort flags0 = 0x0000, flags1 = 0x0000;
            // there MUST be 1 - 10 params.
            // params MUST be one of:   R0, R1, R2, R3, R4, R5, R6, R7
            //                          A, B, C, I, J, X, Y, Z
            //                          FL, PC, PS, SP, USP, II, IA, P2
            for (int i = 0; i < param.Count; i++)
            {
                string p = param[i];
                ParsedOpcode op = ParseParam(p);
                if (op == null || 
                    ((op.AddressingMode != AddressingMode.Register) && (op.AddressingMode != AddressingMode.StatusRegister)))
                    throw new Exception(string.Format("STK operation with unknown register '{0}'.", p));
                else
                {
                    if (op.AddressingMode == AddressingMode.Register)
                    {
                        flags0 |= (ushort)(1 << (op.OpcodeWord + 8));
                    }
                    else if (op.AddressingMode == AddressingMode.StatusRegister)
                    {
                        flags1 |= (ushort)(1 << (op.OpcodeWord + 8));
                    }
                }
            }

            m_Code.Clear();

            if (general_first && (flags0 != 0))
                m_Code.Add((ushort)(opcode | flags0));
            if (flags1 != 0)
                m_Code.Add((ushort)(opcode | flags1 | 0x0001));
            if (!general_first && (flags0 != 0))
                m_Code.Add((ushort)(opcode | flags0));

            return m_Code;
        }

        List<ushort> AssembleSWO(ushort opcode, string param1, string param2, string param3)
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
            return m_Code;
        }
    }
}
