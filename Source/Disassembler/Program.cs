using System;
using System.Collections.Generic;
using System.IO;

namespace Ypsilon
{
    class Program
    {
        static void Main(string[] args)
        {
            string inPath = "../../../../../Tests/bld/AsmTstGn-0.asm.bin";

            if (tryDisassemble(inPath))
                Console.WriteLine("Success!");
            else
                Console.WriteLine("Error. :(");

            Console.ReadKey();
        }

        private static bool tryDisassemble(string inPath)
        {
            string[] disassembly = null;
            string outPath = inPath + ".asm";

            if (File.Exists(inPath))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(inPath, FileMode.Open)))
                {
                    Disasm disasm = new Disasm();
                    disassembly = disasm.Disassemble(reader);
                }
                File.WriteAllLines(outPath, disassembly);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
