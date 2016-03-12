using System.IO;
using Ypsilon.Emulation.Hardware;

namespace YCPUXNA
{
    class Dsm
    {
        public bool TryDisassemble(string[] args)
        {
            string[] disassembly = null;

            if (args.Length <= 1)
                return false;

            string inPath = args[1];
            string outPath = inPath + ".disasm";

            if (File.Exists(inPath))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(inPath, FileMode.Open)))
                {
                    disassembly = Disassemble(reader);
                }
                File.WriteAllLines(outPath, disassembly);
                return true;
            }
            else
            {
                return false;
            }
        }

        private string[] Disassemble(BinaryReader reader)
        {
            YCPU ycpu = new YCPU();
            byte[] data = new byte[reader.BaseStream.Length];
            ycpu.BUS.SetROM((uint)data.Length, data);

            string[] disassembled;
            disassembled = ycpu.Disassemble(0x0000, 32000, false);

            return disassembled;
        }
    }
}
