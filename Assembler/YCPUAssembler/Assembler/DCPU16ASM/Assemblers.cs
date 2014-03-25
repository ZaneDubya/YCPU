/*
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Assembler.DCPU16ASM
{
    public delegate ushort[] OpcodeAssembler(string param1, string param2);

    public partial class Parser
    {
        ushort[] AssembleSET(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.SET_OP, param1, param2);
        }

        ushort[] AssembleADD(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.ADD_OP, param1, param2);
        }

        ushort[] AssembleSUB(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.SUB_OP, param1, param2);
        }

        ushort[] AssembleMUL(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.MUL_OP, param1, param2);
        }

        ushort[] AssembleDIV(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.DIV_OP, param1, param2);
        }

        ushort[] AssembleMOD(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.MOD_OP, param1, param2);
        }

        ushort[] AssembleSHL(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.SHL_OP, param1, param2);
        }

        ushort[] AssembleSHR(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.SHR_OP, param1, param2);
        }

        ushort[] AssembleAND(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.AND_OP, param1, param2);
        }

        ushort[] AssembleBOR(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.BOR_OP, param1, param2);
        }

        ushort[] AssembleXOR(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.XOR_OP, param1, param2);
        }

        ushort[] AssembleIFE(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.IFE_OP, param1, param2);
        }

        ushort[] AssembleIFN(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.IFN_OP, param1, param2);
        }

        ushort[] AssembleIFG(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.IFG_OP, param1, param2);
        }

        ushort[] AssembleIFB(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.IFB_OP, param1, param2);
        }

        ushort[] AssembleJSR(string param1, string param2)
        {
            return AssembleInstruction((ushort)dcpuOpCode.JSR_OP, param1);
        }

        ushort[] AssembleInstruction(ushort opcode, string param1)
        {
            List<ushort> code = new List<ushort>();
            ParsedOpcode p1 = this.ParseParam(param1);
            opcode |= (ushort)(((uint)p1.Word << 10) & 0xFC00);

            // this.machineCode.Add((ushort)opcode);
            code.Add((ushort)opcode);

            if (p1.UsesNextWord)
            {
                if (p1.LabelName.Length > 0)
                {
                    this.m_LabelReferences.Add((ushort)(m_MachineCode.Count + code.Count),
                        p1.LabelName);
                }

                // this.machineCode.Add(p1.NextWord);
                code.Add((ushort)p1.NextWord);
            }
            return code.ToArray();
        }

        ushort[] AssembleInstruction(ushort opcode, string param1, string param2)
        {
            List<ushort> code = new List<ushort>();
            var p1 = this.ParseParam(param1);
            var p2 = this.ParseParam(param2);

            opcode |= (ushort)(((uint)p1.Word << 4) & 0x3F0);
            opcode |= (ushort)(((uint)p2.Word << 10) & 0xFC00);

            // this.machineCode.Add((ushort)opcode);
            code.Add((ushort)opcode);

            if (p1.UsesNextWord)
            {
                if (p1.LabelName.Length > 0)
                {
                    this.m_LabelReferences.Add((ushort)(this.m_MachineCode.Count + code.Count),
                        p1.LabelName);
                }

                code.Add(p1.NextWord);
            }

            if (p2.UsesNextWord)
            {
                if (p2.LabelName.Length > 0)
                {
                    this.m_LabelReferences.Add((ushort)(this.m_MachineCode.Count + code.Count),
                        p2.LabelName);
                }

                code.Add(p2.NextWord);
            }
            return code.ToArray();
        }
    }
}
