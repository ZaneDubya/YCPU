/*
 * DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

namespace YCPU.Assembler.DCPU16ASM
{
    using System;
    using System.IO;

    public class Generator
    {
        public string MessageOuput { get; private set; }

        public string Generate(ushort[] machineCode, string filename)
        {
            if (filename.Trim() == string.Empty)
            {
                filename = "Default.bin";
            }
            else
            {
                filename = filename.Split('.')[0] + ".bin";
            }

            try
            {
                var outfile = new MemoryStream();
                foreach (var word in machineCode)
                {
                    var b = (byte)(word >> 8);
                    var a = (byte)(word & 0xFF);

                    outfile.WriteByte(b);
                    outfile.WriteByte(a);
                }

                File.WriteAllBytes(filename, outfile.ToArray());
            }
            catch (Exception e)
            {
                this.AddMessageLine(string.Format("{0}", e.Message));
                return string.Empty;
            }

            this.AddMessageLine();
            this.AddMessageLine(string.Format("Saved to '{0}", filename));

            return filename;
        }

        private void AddMessageLine()
        {
            this.MessageOuput += "\r\n";
        }

        private void AddMessageLine(string input)
        {
            this.MessageOuput += string.Format("{0}\r\n", input);
        }
    }
}
