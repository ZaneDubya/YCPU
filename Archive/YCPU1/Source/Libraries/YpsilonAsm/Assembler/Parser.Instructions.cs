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

        private OpcodeFlag[] flag8or16 = { OpcodeFlag.BitWidth8, OpcodeFlag.BitWidth16 };

        #region ALU

        private List<ushort> AssembleCMP(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0000, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleNEG(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0008, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleADD(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0010, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleSUB(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0018, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleADC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0020, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleSBC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0028, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleMUL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0030, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleDIV(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0038, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleMLI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0040, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleDVI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0048, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleMOD(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0050, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleMDI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0058, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleAND(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0060, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleORR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0068, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleEOR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0070, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleNOT(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, state.Parser.flag8or16);
            return state.Parser.AssembleALU(0x0078, param[0], param[1], opcodeFlag, state);
        }


        private List<ushort> AssembleLOD(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth8, OpcodeFlag.BitWidth16 });

            return state.Parser.AssembleALU(0x0080, param[0], param[1], opcodeFlag, state);
        }

        private List<ushort> AssembleSTO(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth8, OpcodeFlag.BitWidth16 });

            List<ushort> code = state.Parser.AssembleALU(0x0088, param[0], param[1], opcodeFlag, state);
            if ((code[0] & 0xF000) == 0x1000) // no sto register - should lod r0, r1, not sto r0, r1
                throw new Exception("Store register instructions not supported");
            if ((code[0] & 0xFE00) == 0x0000) // no sto immediate.
                throw new Exception("Store immediate instructions not supported");
            return code;
        }
        #endregion

        #region Branch operations

        private List<ushort> AssembleBCC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA(0x0090, param[0], state);
        }

        private List<ushort> AssembleBCS(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA(0x0091, param[0], state);
        }

        private List<ushort> AssembleBNE(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA(0x0092, param[0], state);
        }

        private List<ushort> AssembleBEQ(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA(0x0093, param[0], state);
        }

        private List<ushort> AssembleBPL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA(0x0094, param[0], state);
        }

        private List<ushort> AssembleBMI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA(0x0095, param[0], state);
        }

        private List<ushort> AssembleBVC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA(0x0096, param[0], state);
        }

        private List<ushort> AssembleBVS(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA(0x0097, param[0], state);
        }

        private List<ushort> AssembleBUG(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA(0x0098, param[0], state);
        }

        private List<ushort> AssembleBSG(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA(0x0099, param[0], state);
        }

        private List<ushort> AssembleBAW(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBRA(0x009F, param[0], state);
        }
        #endregion

        #region Shift operations

        private List<ushort> AssembleASL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF(0x00A0, param[0], param[1]);
        }

        private List<ushort> AssembleLSL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF(0x00A1, param[0], param[1]);
        }

        private List<ushort> AssembleROL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF(0x00A2, param[0], param[1]);
        }

        private List<ushort> AssembleRNL(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF(0x00A3, param[0], param[1]);
        }

        private List<ushort> AssembleASR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF(0x00A4, param[0], param[1]);
        }

        private List<ushort> AssembleLSR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF(0x00A5, param[0], param[1]);
        }

        private List<ushort> AssembleROR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF(0x00A6, param[0], param[1]);
        }

        private List<ushort> AssembleRNR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSHF(0x00A7, param[0], param[1]);
        }
        #endregion

        #region Bit testing operations

        private List<ushort> AssembleBTT(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBTI(0x00A8, param[0], param[1]);
        }

        private List<ushort> AssembleBTX(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBTI(0x00A9, param[0], param[1]);
        }

        private List<ushort> AssembleBTC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBTI(0x00AA, param[0], param[1]);
        }

        private List<ushort> AssembleBTS(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleBTI(0x00AB, param[0], param[1]);
        }
        #endregion

        #region Set flags

        private List<ushort> AssembleSEF(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountMinMax(param, 1, 4);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleFLG(0x00AE, param);
        }

        private List<ushort> AssembleCLF(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountMinMax(param, 1, 4);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleFLG(0x00AF, param);
        }
        #endregion

        #region Stack Push/Pop/Flush

        private List<ushort> AssemblePSH(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountMinMax(param, 1, 13);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSTK(0x00B0, param, false);
        }

        private List<ushort> AssemblePOP(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountMinMax(param, 1, 13);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleSTK(0x00B2, param, true);
        }
        #endregion

        #region MMU

        private List<ushort> AssembleLSG(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            return state.Parser.AssembleMMU(0x00B5, param[0], state);
        }

        private List<ushort> AssembleSSG(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            return state.Parser.AssembleMMU(0x01B5, param[0], state);
        }
        #endregion

        private List<ushort> AssembleSET(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            return state.Parser.AssembleSEI(0x00AC, param[0], param[1]);
        }

        #region Inc/Dec

        private List<ushort> AssembleINC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleIMM(0x00B6, param[0], 1.ToString());
        }

        private List<ushort> AssembleADI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleIMM(0x00B6, param[0], param[1]);
        }

        private List<ushort> AssembleDEC(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleIMM(0x00B7, param[0], 1.ToString());
        }

        private List<ushort> AssembleSBI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 2);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleIMM(0x00B7, param[0], param[1]);
        }
        #endregion

        #region Processor Functions

        private List<ushort> AssembleHWQ(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return state.Parser.AssembleHWI(0x00BA, param[0]);
        }
        #endregion

        #region Jump operations

        private List<ushort> AssembleJMP(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            if (opcodeFlag.HasFlag(OpcodeFlag.FarJump))
            {
                Guard.RequireParamCountMinMax(param, 1, 2);
                return state.Parser.AssembleJMI(0x01B8, param[0], param.Count == 2 ? param[1] : null, state);
            }
            Guard.RequireParamCountExact(param, 1);
            return state.Parser.AssembleJMI(0x00B8, param[0], null, state);
        }

        private List<ushort> AssembleJSR(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            if (opcodeFlag.HasFlag(OpcodeFlag.FarJump))
            {
                Guard.RequireParamCountMinMax(param, 1, 2); // allow two params for immediate far jump.
                return state.Parser.AssembleJMI(0x01B9, param[0], param.Count == 2 ? param[1] : null, state);
            }
            Guard.RequireParamCountExact(param, 1);
            return state.Parser.AssembleJMI(0x00B9, param[0], null, state);
        }
        #endregion

        private List<ushort> AssembleRTS(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            if (opcodeFlag.HasFlag(OpcodeFlag.FarJump))
            {
                return new List<ushort> { 0x01B4 }; // RTS.F
            }
            return new List<ushort> { 0x00B4 }; // RTS
        }

        private List<ushort> AssembleRTI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 0);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return new List<ushort> { 0x02B4 };
        }

        private List<ushort> AssembleSTX(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 1);

            Param p1 = ParseParam(param[0]);
            if (p1.AddressingMode != AddressingMode.Immediate)
                throw new Exception("stx instructions expect a single immediate parameter");

            int p1i = (short)p1.ImmediateWordShort;
            if (p1i < sbyte.MinValue || p1i > sbyte.MaxValue)
                throw new Exception("stx instructions accept a single immediate parameter with values between -128 and +127");

            return new List<ushort> { (ushort)(0x00BB | (((sbyte)p1i) << 8)) };
        }

        private List<ushort> AssembleSWI(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 0);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return new List<ushort> { 0x03B4 };
        }

        private List<ushort> AssembleSLP(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 0);
            Guard.RequireOpcodeFlag(opcodeFlag, new[] { OpcodeFlag.BitWidth16 });
            return new List<ushort> { 0x04B4 };
        }

        #region Macro NOP

        private List<ushort> AssembleNOP(List<string> param, OpcodeFlag opcodeFlag, ParserState state)
        {
            Guard.RequireParamCountExact(param, 0);
            return new List<ushort> { 0x2080 }; // LOD R0, R0
        }
        #endregion

        private List<ushort> m_Code = new List<ushort>();

        private List<ushort> AssembleALU(ushort opcode, string param1, string param2, OpcodeFlag opcodeFlag, ParserState state)
        {
            Param p1 = ParseParam(param1);
            Param p2 = ParseParam(param2);

            if (p1.AddressingMode != AddressingMode.Register)
            {
                throw new Exception("ALU instruction first operand must be a general purpose register");
            }
            
            ushort addressingmode = 0x0000;
            switch (p2.AddressingMode)
            {
                case AddressingMode.None:
                    return null;
                case AddressingMode.Immediate:
                    // .000 000e               Immediate           LOD R0, $1234       +1m
                    // special case: alu.8 immediate with value greater than $FF should raise a warning...
                    if (opcodeFlag == OpcodeFlag.BitWidth8 && p2.ImmediateWordShort >= 256)
                    {
                        throw new Exception("8-bit load operation with an immediate value of greater than 8 bits");
                    }
                    addressingmode = 0x0000;
                    if (p2.UsesExtraDataSegment)
                        throw new Exception("Immediate addressing mode cannot use extra data segment");
                    break;
                case AddressingMode.Absolute:
                    // s000 001e               Absolute            LOD R0, [$1234]     +2m
                    addressingmode = 0x0200;
                    if (p2.UsesExtraDataSegment)
                        addressingmode |= 0x8000;
                    break;
                case AddressingMode.ControlRegister:
                    // .000 1ppp               Control register    LOD R0, PS             
                    // can't use eightbit mode with proc regs...
                    if (opcodeFlag.HasFlag(OpcodeFlag.BitWidth8))
                        throw new Exception("ALU instructions with status register operands do not support 8-bit mode");
                    if (p2.UsesExtraDataSegment)
                        throw new Exception("Control register addressing mode cannot use extra data segment");
                    addressingmode = (ushort)(0x0800 | ((p2.RegisterIndex & 0x0007) << 8));
                    p2.RegisterIndex = 0; // must wipe out control register index, as it is used later in this subroutine.
                    break;
                case AddressingMode.Register:
                    // .001 rrre               Register            LOD R0, r1            
                    addressingmode = 0x1000;
                    if (p2.UsesExtraDataSegment)
                        throw new Exception("Register addressing mode cannot use extra data segment");
                    break;
                case AddressingMode.Indirect:
                    // s010 rrre               Indirect            LOD R0, [r1]        +1m
                    addressingmode = 0x2000;
                    if (p2.UsesExtraDataSegment)
                        addressingmode |= 0x8000;
                    break;
                case AddressingMode.IndirectOffset:
                    // s011 rrre               Indirect Offset     LOD R0, [r1,$1234]  +2m
                    addressingmode = 0x3000;
                    if (p2.UsesExtraDataSegment)
                        addressingmode |= 0x8000;
                    break;
                case AddressingMode.IndirectIndexed:
                    // s1ii rrre               Indirect Indexed    LOD R0, [r1,i2]     +1m
                    int index_register = (p2.RegisterIndex & 0x0300) << 4;
                    addressingmode = (ushort)(0x4000 | index_register);
                    if (p2.UsesExtraDataSegment)
                        addressingmode |= 0x8000;
                    break;
                default:
                    throw new Exception("Unknown addressing mode");
            }

            ushort bitwidth = 0x0000;
            if (opcodeFlag == OpcodeFlag.BitWidth8)
                bitwidth = 0x0100;

            // FEDC BA98 7654 3210                             
            // AAAA rrrE OOOO ORRR
            m_Code.Clear();
            m_Code.Add((ushort)(opcode | addressingmode | bitwidth | (p1.RegisterIndex & 0x0007) | ((p2.RegisterIndex & 0x0007) << 9)));

            if (p2.HasLabel)
            {
                state.Labels.Add((ushort)(state.Code.Count + m_Code.Count * c_InstructionSize), p2.Label);
                m_Code.Add(0xDEAD);
            }
            else if (p2.HasImmediateWord)
            {
                m_Code.Add(p2.ImmediateWordShort);
            }
            return m_Code;
        }

        private List<ushort> AssembleBTI(ushort opcode, string param1, string param2)
        {
            Param p1 = ParseParam(param1);
            Param p2 = ParseParam(param2);

            // p1 MUST be a register.
            if (p1.AddressingMode != AddressingMode.Register)
                return null;

            // p2 MUST be EITHER an immediate value, OR a register.
            // if p2 is immediate, it MUST be 0 - 15.
            // if p2 is a register, it MUST be 0 - 7.
            switch (p2.AddressingMode)
            {
                case AddressingMode.Immediate:
                    if (p2.RegisterIndex >= 16)
                        return null;
                    break;
                case AddressingMode.Register:
                    if (p2.RegisterIndex >= 8)
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

            ushort r_bits = (ushort)((p1.RegisterIndex & 0x0007) << 13);

            ushort s_bits = 0x0000;
            if (p2.AddressingMode == AddressingMode.Immediate)
                s_bits = (ushort)(p2.ImmediateWordShort << 8);
            else if (p2.AddressingMode == AddressingMode.Register)
                s_bits = (ushort)((0x1000) | (p2.RegisterIndex << 8));

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | r_bits | s_bits));
            return m_Code;
        }

        private List<ushort> AssembleBRA(ushort opcode, string param1, ParserState state)
        {
            Param p1 = ParseParam(param1);
            // must be branching to a label or an immediate value between $7f and -$80
            if (!p1.HasLabel && ((short)p1.ImmediateWordShort > 0x7f) || ((short)p1.ImmediateWordShort < -0x80))
                throw new Exception("Branch operation must have offset between -128 and +127");

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | (p1.ImmediateWordShort << 8)));
            if (p1.HasLabel)
                state.Branches.Add((ushort)state.Code.Count, p1.Label.ToLowerInvariant());
            return m_Code;
        }

        private List<ushort> AssembleFLG(ushort opcode, List<string> param)
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
                        throw new Exception("FLG instruction with non-existing flag");
                }
            }

            ushort flags = (ushort)((n ? 0x8000 : 0x0000) | (z ? 0x4000 : 0x0000) | (c ? 0x2000 : 0x0000) | (v ? 0x1000 : 0x0000));

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | flags));
            return m_Code;
        }

        private List<ushort> AssembleHWI(ushort opcode, string param1)
        {
            // param1 = index of operations, must be integer from 0-255
            Param p1 = ParseParam(param1);

            if (p1.AddressingMode != AddressingMode.Immediate)
                return null;
            if ((p1.RegisterIndex < 0) || (p1.RegisterIndex > 255))
                return null;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // iiii iiii OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | ((p1.ImmediateWordShort & 0x00FF) << 8)));
            return m_Code;
        }

        private List<ushort> AssembleIMM(ushort opcode, string param1, string param2)
        {
            // param1 = source/dest register, MUST be register
            // param2 = immediate value, MUST be number, MUST be 1 - 32
            Param p1 = ParseParam(param1);
            Param p2 = ParseParam(param2);

            if (p1.AddressingMode != AddressingMode.Register)
                return null;
            if (p2.AddressingMode != AddressingMode.Immediate)
                return null;
            if ((p2.ImmediateWordShort < 1) || (p2.ImmediateWordShort > 32))
                return null;

            p2.ImmediateWordShort -= 1;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // RRRv vvvv OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | ((p1.RegisterIndex & 0x0007) << 13) | ((p2.ImmediateWordShort & 0x001F) << 8)));
            return m_Code;
        }

        private List<ushort> AssembleJMI(ushort opcode, string param1, string param2, ParserState state)
        {
            Param p1, p2;
            p1 = ParseParam(param1);
            p2 = ParseParam(param2);

            // sanity check - immediate jump must have two params (first param being a 32-bit immediate), all others must have one.
            if (p1.AddressingMode == AddressingMode.Immediate && 
                p2 != null && p2.AddressingMode == AddressingMode.ImmediateBig && 
                ((opcode & 0x0100) == 0x0100))
            {
                if (p2 == null)
                    throw new Exception("Immediate far jumps require two immedate parameters. The first should be 16-bit, the second, 32-bit");
            }
            else
            {
                if (p2 != null)
                    throw new Exception("Only far jumps in immediate mode can have more than one parameter");
            }


            ushort addressingmode = 0x0000;
            switch (p1.AddressingMode)
            {
                case AddressingMode.None:
                    return null;
                case AddressingMode.Immediate:
                    if (p2 != null && p2.AddressingMode != AddressingMode.ImmediateBig)
                        throw new Exception("Immediate far jumps require two immedate parameters. The first should be 16-bit, the second, 32-bit");
                    addressingmode = 0x0000;
                    break;
                case AddressingMode.Absolute:
                    addressingmode = 0x0200;
                    if (p1.UsesExtraDataSegment)
                        addressingmode |= 0x8000;
                    break;
                case AddressingMode.Register:
                    addressingmode = 0x1000;
                    break;
                case AddressingMode.Indirect:
                    addressingmode = 0x2000;
                    if (p1.UsesExtraDataSegment)
                        addressingmode |= 0x8000;
                    break;
                case AddressingMode.IndirectOffset:
                    addressingmode = 0x3000;
                    if (p1.UsesExtraDataSegment)
                        addressingmode |= 0x8000;
                    break;
                case AddressingMode.IndirectIndexed:
                    int index_register = (p1.RegisterIndex & 0x0300) << 4;
                    addressingmode = (ushort)(0x4000 | index_register);
                    if (p1.UsesExtraDataSegment)
                        addressingmode |= 0x8000;
                    break;
                default:
                    throw new Exception("Addressing mode not usable with this instruction");
            }
            m_Code.Clear();
            m_Code.Add((ushort)(opcode | addressingmode | ((p1.RegisterIndex & 0x0007) << 9)));

            if (p1.HasLabel)
            {
                state.Labels.Add((ushort)(state.Code.Count + m_Code.Count * c_InstructionSize), p1.Label);
                m_Code.Add(0xDEAD);
            }
            else if (p1.HasImmediateWord)
            {
                m_Code.Add(p1.ImmediateWordShort);
            }

            if (p2 != null)
            {
                if (p1.AddressingMode != AddressingMode.Immediate)
                    throw new Exception("Only immediate far jumps can have a second operand");
                if (p2.AddressingMode != AddressingMode.ImmediateBig)
                    throw new Exception("The second operand in an immediate far jump must be 32-bit");
                m_Code.Add((ushort)(p2.ImmediateWordLong & 0x0000ffff));
                m_Code.Add((ushort)((p2.ImmediateWordLong & 0xffff0000) >> 16));
            }
            return m_Code;
        }

        private List<ushort> AssembleMMU(ushort opcode, string param1, ParserState state)
        {
            // param1 = segment register, MUST be segment register
            Param p1 = ParseParam(param1);

            if (p1 == null || p1.AddressingMode != AddressingMode.SegmentRegister)
                throw new Exception("Parameter of MMU instructions must be a single segment register");

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // u...rrro OOOO OOOO
            // ''''''' <- param bits - 7-bit number indicating a segment.

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | ((p1.RegisterIndex & 0x7f) << 9)));
            return m_Code;
        }

        private List<ushort> AssembleSEI(ushort opcode, string param1, string param2)
        {
            Param p1 = ParseParam(param1);
            Param p2 = ParseParam(param2);

            // p1 MUST be a register.
            if (p1.AddressingMode != AddressingMode.Register)
                return null;

            // p2 must be a value, and there is a limited range of allowed values.
            if (p2.AddressingMode != AddressingMode.Immediate)
                return null;

            ushort alternateValueBit, value;
            bool valueIsAllowed = TryGetSETValue(p2, out value, out alternateValueBit);

            if (!valueIsAllowed)
                throw new Exception($"SET instruction with invalid value parameter: {p2.ImmediateWordShort}");

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | alternateValueBit | (value << 8) | ((p1.RegisterIndex & 0x0007) << 13)));
            return m_Code;
        }

        private bool IsAcceptableSETValue(Param input)
        {
            ushort alternateValueBit, value;
            return TryGetSETValue(input, out value, out alternateValueBit);
        }

        private bool TryGetSETValue(Param input, out ushort value, out ushort alternateValueBit)
        {
            alternateValueBit = 0;
            value = 0;

            if (input.HasLabel)
            {
                // not allowed to use labels with SET instructions.
                return false;
            }

            if (input.ImmediateWordShort <= 0x1F)
            {
                value = input.ImmediateWordShort;
                return true;
            }
            if (input.ImmediateWordShort >= 0xFFEB)
            {
                value = (ushort)((input.ImmediateWordShort - 0xFFEB) + 0x000B);
                alternateValueBit = 1;
                return true;
            }
            for (int i = 0x05; i < 0x10; i++)
            {
                int allowedValue = (ushort)(Math.Pow(2, i));
                if (input.ImmediateWordShort == allowedValue)
                {
                    value = (ushort)(i - 5);
                    alternateValueBit = 1;
                    return true;
                }
            }
            return false;
        }

        private List<ushort> AssembleSHF(ushort opcode, string param1, string param2)
        {
            Param p1 = ParseParam(param1);
            Param p2 = ParseParam(param2);


            // p1 MUST be a register.
            if (p1.AddressingMode != AddressingMode.Register)
                return null;

            // p2 MUST be EITHER an immediate value, OR a register.
            // if p2 is immediate, it MUST be 1 - 16.
            // if p2 is a register, it MUST be 0 - 7.
            switch (p2.AddressingMode)
            {
                case AddressingMode.Immediate:
                    if (p2.ImmediateWordShort > 16 || p2.ImmediateWordShort == 0)
                        return null;
                    break;
                case AddressingMode.Register:
                    if (p2.RegisterIndex >= 8)
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

            ushort r_bits = (ushort)((p1.RegisterIndex & 0x0007) << 13);

            ushort s_bits = 0x0000;
            if (p2.AddressingMode == AddressingMode.Immediate)
                s_bits = (ushort)((p2.ImmediateWordShort - 1) << 8);
            if (p2.AddressingMode == AddressingMode.Register)
                s_bits = (ushort)((0x1000) | (p2.RegisterIndex << 8));

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | r_bits | s_bits));
            return m_Code;
        }

        private List<ushort> AssembleSFL(ushort opcode, string param1)
        {
            // param1 = index of operations, must be integer from 1-256
            Param p1 = ParseParam(param1);

            if (p1.AddressingMode != AddressingMode.Immediate)
                return null;
            if ((p1.ImmediateWordShort < 1) || (p1.ImmediateWordShort > 256))
                return null;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // iiii iiii OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | (((p1.ImmediateWordShort - 1) & 0x00FF) << 8)));
            return m_Code;
        }

        private List<ushort> AssembleSTK(ushort opcode, List<string> param, bool general_first)
        {
            ushort flags0 = 0x0000, flags1 = 0x0000;
            // there MUST be 1 - 10 params.
            // params MUST be one of:   R0, R1, R2, R3, R4, R5, R6, R7
            //                          A, B, C, I, J, X, Y, Z
            //                          FL, PC, PS, SP, USP
            for (int i = 0; i < param.Count; i++)
            {
                string p = param[i];
                Param op = ParseParam(p);
                if (op == null || 
                    ((op.AddressingMode != AddressingMode.Register) && (op.AddressingMode != AddressingMode.ControlRegister)))
                    throw new Exception($"STK operation with unknown register '{p}'");
                if (op.AddressingMode == AddressingMode.Register)
                {
                    flags0 |= (ushort)(1 << (op.RegisterIndex + 8));
                }
                else if (op.AddressingMode == AddressingMode.ControlRegister)
                {
                    flags1 |= (ushort)(1 << (op.RegisterIndex + 8));
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

        private List<ushort> AssembleSWO(ushort opcode, string param1, string param2, string param3)
        {
            // param1 = source register, MUST be register
            // param2 = dest register, MUST be register
            // param3 = flags, MUST be LR, HR, LW, HW
            Param p1 = ParseParam(param1);
            Param p2 = ParseParam(param2);
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
                    throw new Exception("Bad SWO flag");
            }

            if (p1.AddressingMode != AddressingMode.Register)
                return null;
            if (p2.AddressingMode != AddressingMode.Register)
                return null;

            // Bit pattern is:
            // FEDC BA98 7654 3210
            // RRRr rroo OOOO OOOO

            m_Code.Clear();
            m_Code.Add((ushort)(opcode | ((p1.RegisterIndex & 0x0007) << 13) | ((p2.RegisterIndex & 0x0007) << 10) | (flag_type << 8)));
            return m_Code;
        }
    }
}
