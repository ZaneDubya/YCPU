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

        public string Generate(ushort[] machineCode, string directory, string filename)
        {
            if (filename.Trim() == string.Empty)
            {
                filename = "out.bin";
            }
            else
            {
                filename = Path.GetFileNameWithoutExtension(filename) + ".bin";
            }

            if ((directory[directory.Length - 1] != '/') && (directory[directory.Length - 1] != '\\'))
                directory += '\\';

            try
            {
                MemoryStream outfile = new MemoryStream();
                foreach (var word in machineCode)
                {
                    var b = (byte)(word >> 8);
                    var a = (byte)(word & 0xFF);

                    outfile.WriteByte(b);
                    outfile.WriteByte(a);
                }

                File.WriteAllBytes(directory + filename, outfile.ToArray());
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
