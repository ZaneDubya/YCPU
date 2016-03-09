/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;
using System.IO;
using System.Collections.Generic;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        bool IncludeBinary(List<string> tokens, ParserState state)
        {
            if (tokens.Count == 1)
                throw new Exception(string.Format("No file specified for .incbin pragma.", tokens[1]));

            tokens[1] = tokens[1].Replace("\"", string.Empty);

            byte[] data = GetBytesFromFile(state.WorkingDirectory + @"\" + tokens[1]);
            if (data == null)
                throw new Exception(string.Format("Error loading file '{0}'.", tokens[1]));

            int begin = 0, length = data.Length;
            if (tokens.Count >= 3)
                if (!Int32.TryParse(tokens[2], out begin))
                    throw new Exception(string.Format("Third paramter for incbin must be numeric."));

            if (tokens.Count == 3)
            {
                length = length - begin;
            }

            if (tokens.Count == 4)
                if (!Int32.TryParse(tokens[3], out length))
                    throw new Exception(string.Format("Fourth paramter for incbin must be numeric."));

            if ((begin >= length) || (begin + length > data.Length))
                throw new Exception("Out of bounds for incbin.");

            for (int i = 0; i < length; i++)
                state.Code.Add(data[i + begin]);
            return true;
        }

        byte[] GetBytesFromFile(string path)
        {
            try
            {
                byte[] data = File.ReadAllBytes(path);
                return data;
            }
            catch
            {
                return null;
            }
        }
    }
}
