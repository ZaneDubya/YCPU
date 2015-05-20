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
        int ParseLabel(string line, bool local, ParserState state)
        {
            int colon_pos = line.IndexOf(':');
            string labelName = line.Substring(0, colon_pos);

            if (!state.Scopes.AddLabel(labelName.Trim().ToLower(), state.Code.Count, local))
                throw new Exception(string.Format("Error adding label '{0}'.", labelName));
            return colon_pos + 1;
        }
    }
}
