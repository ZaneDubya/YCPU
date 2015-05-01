/*
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

using System;
using System.IO;

namespace YCPU.Assembler
{
    class Generator
    {
        public string MessageOuput { get; private set; }

        public string Generate(byte[] machineCode, string directory, string filename)
        {
            if (filename.Trim() == string.Empty)
            {
                filename = "out.bin";
            }
            else
            {
                filename = Path.GetFileNameWithoutExtension(filename) + ".bin";
            }

            if (directory != string.Empty)
            {
                if ((directory[directory.Length - 1] != '/') && (directory[directory.Length - 1] != '\\'))
                    directory += '\\';
            }

            try
            {
                MemoryStream outfile = new MemoryStream();
                foreach (byte word in machineCode)
                {
                    outfile.WriteByte(word);
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
