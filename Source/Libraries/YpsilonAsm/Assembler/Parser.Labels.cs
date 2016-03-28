/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        private int ParseLabel(string line, ParserState state)
        {
            int colonPos = line.IndexOf(':');
            string labelName = line.Substring(0, colonPos);

            if (!state.Scopes.AddLabel(labelName.Trim().ToLowerInvariant(), state.Code.Count))
                throw new Exception($"Error adding label '{labelName}'");
            return colonPos + 1;
        }
    }
}
