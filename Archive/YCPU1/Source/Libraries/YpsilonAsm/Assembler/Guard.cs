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
    internal static class Guard
    {
        public static void RequireParamCountExact(List<string> param, int count)
        {
            if (param.Count != count)
                throw new Exception($"Bad param count, expected {count}");
        }

        public static void RequireParamCountMinMax(List<string> param, int min, int max)
        {
            if ((param.Count < min) || (param.Count > max))
                throw new Exception($"Bad param count, expected {min}-{max}");
        }

        public static void RequireOpcodeFlag(OpcodeFlag flag, OpcodeFlag[] acceptable)
        {
            for (int i = 0; i < acceptable.Length; i++)
                if (flag.HasFlag(acceptable[i]))
                    return;
            throw new Exception($"Opcode flag of '{flag}' is unsupported for this opcode");
        }
    }
}
