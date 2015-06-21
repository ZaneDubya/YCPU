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
        bool ParsePragma(int lineIndex, string line, string opcode, string[] tokens, ParserState state)
        {
            if (LineSearch.MatchPragma(opcode.ToLower()))
            {
                switch (opcode.ToLower())
                {
                    case ".dat8":
                        ParseData8(line.Replace(".dat8", string.Empty), state);
                        return true;

                    case ".dat16":
                        ParseData16(line.Replace(".dat16", string.Empty), state);
                        return true;

                    case ".advance":

                        break;
                    case ".alias":

                        break;
                    case ".checkpc":

                        break;
                    case ".org":

                        break;

                    case ".incbin":
                        return IncludeBinary(tokens, state);

                    case ".include":

                        break;
                    case ".macro":

                        break;
                    case ".macend":

                        break;

                    case ".require":

                        break;
                    case ".reserve":

                        break;
                    case ".scope":
                    case "{":
                        state.Scopes.ScopeOpen(state.Code.Count, lineIndex);
                        return true;
                    case ".scend":
                    case "}":
                        return state.Scopes.ScopeClose(state.Code.Count);
                    default:
                        throw new Exception(string.Format("Unimplemented pragma in line {0}", line));
                }
            }
            return false;
        }
    }
}
