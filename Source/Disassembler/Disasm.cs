using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ypsilon.Hardware;

namespace Ypsilon
{
    class Disasm
    {
        public string[] Disassemble(BinaryReader reader)
        {
            YCPU ycpu = new YCPU();
            LoadBinaryToCPU(reader, ycpu);

            string[] disassembled;
            disassembled = ycpu.Disassemble(0x0000, 10000, false);

            return disassembled;
        }

        private void LoadBinaryToCPU(BinaryReader reader, YCPU ycpu)
        {
            ushort address = 0;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                ycpu.WriteMemInt8((ushort)(address),reader.ReadByte());
                address += 1;
            }
        }
    }
}
