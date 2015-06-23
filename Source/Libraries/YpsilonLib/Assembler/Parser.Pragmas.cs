/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;
using System.Collections.Generic;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        bool ParsePragma(int lineIndex, string line, string opcode, List<string> tokens, ParserState state)
        {
            string pragma = opcode.ToLowerInvariant();
            if (LineSearch.MatchPragma(pragma))
            {
                if (pragma == ".dat8")
                {
                    ParseData8(line.Replace(".dat8", string.Empty), state);
                    return true;
                }
                else if (pragma == ".dat16")
                {
                    ParseData16(line.Replace(".dat16", string.Empty), state);
                    return true;
                }
                else if (pragma == ".advance")
                {

                }
                else if (pragma == ".alias")
                {

                }
                else if (pragma == ".checkpc")
                {

                }
                else if (pragma == ".org")
                {

                }
                else if (pragma == ".incbin")
                {
                    return IncludeBinary(tokens, state);
                }
                else if (pragma == ".include")
                {

                }
                else if (pragma == ".macro")
                {

                }
                else if (pragma == ".macend")
                {

                }
                else if (pragma == ".require")
                {

                }
                else if (pragma == ".reserve")
                {

                }
                else if (pragma == ".scope" || pragma == "{")
                {
                    state.Scopes.ScopeOpen(state.Code.Count, lineIndex);
                    return true;
                }
                else if (pragma == ".scend" || pragma == "}")
                {
                    return state.Scopes.ScopeClose(state.Code.Count);
                }

                throw new Exception(string.Format("Unimplemented pragma in line {0}", line));
            }
            return false;
        }
    }
}
