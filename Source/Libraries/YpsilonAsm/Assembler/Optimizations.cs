/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

namespace Ypsilon.Assembler
{
    internal static class Optimizations
    {
        public static void DoOptimizations(ParserState state)
        {
            /* ALU
                    // special case: we can optimize an immediate add or subtract to an adi or sbi instruction. Saves us the immediate word!
                    if (opcode == 0x0010 && p2.ImmediateWord >= 1 && p2.ImmediateWord <= 32) // add rx, ##.
                    {
                        return state.Parser.AssembleIMM((ushort)0x00B8, param1, param2);
                    }
                    else if (opcode == 0x0018 && p2.ImmediateWord >= 1 && p2.ImmediateWord <= 32) // sub rx, ##.
                    {
                        return state.Parser.AssembleIMM((ushort)0x00B9, param1, param2);
                    }
                    
                    // special case: lod can be optimized with set for certain values.
                    else if (opcode == 0x0000 && IsAcceptableSETValue(p2))
                    {
                        return state.Parser.AssembleSET((ushort)0x00B6, param1, param2);
                    }
             */
        }
    }
}
