using System;
using System.Collections.Generic;
using System.IO;

namespace Ypsilon
{
    class Program
    {
        static void Main(string[] args)
        {
            string inPath = "../../../../Tests/bld/AsmTstGn-0.asm.bin";
            string outPath = inPath + ".asm";

            string[] disassembly;
            if (tryDisassemble(inPath, out disassembly))
                Console.WriteLine("Success!");
            else
                Console.WriteLine("Error. :(");

            Console.ReadKey();
        }

        private static bool tryDisassemble(string inPath, out string[] disassembly)
        {
            disassembly = null;

            if (File.Exists(inPath))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(inPath, FileMode.Open)))
                {
                    Disasm disasm = new Disasm();
                    disassembly = disasm.Disassemble(reader);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
