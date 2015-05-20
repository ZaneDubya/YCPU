using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Assembler
{
    static class Optimizations
    {
        public static void DoOptimizations(ParserState state)
        {
            /* ALU
                    // special case: we can optimize an immediate add or subtract to an adi or sbi instruction. Saves us the immediate word!
                    if (opcode == 0x0010 && p2.NextWord >= 1 && p2.NextWord <= 32) // add rx, ##.
                    {
                        return AssembleIMM((ushort)0x00B8, param1, param2);
                    }
                    else if (opcode == 0x0018 && p2.NextWord >= 1 && p2.NextWord <= 32) // sub rx, ##.
                    {
                        return AssembleIMM((ushort)0x00B9, param1, param2);
                    }
                    
                    // special case: lod can be optimized with set for certain values.
                    else if (opcode == 0x0000 && IsAcceptableSETValue(p2))
                    {
                        return AssembleSET((ushort)0x00B6, param1, param2);
                    }
             */
        }
    }
}
