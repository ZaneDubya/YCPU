using System;
using System.Collections.Generic;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        bool ParsePragma(string line, string opcode, string[] tokens, ParserState state)
        {
            if (RegEx.MatchPragma(opcode.ToLower()))
            {
                switch (opcode.ToLower())
                {
                    case ".dat8":
                        m_IsNextLineData = ParseData8(line, state);
                        return true;

                    case ".dat16":
                        m_IsNextLineData = ParseData16(line, state);
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
                        state.m_Scopes.ScopeOpen(state.machineCode.Count);
                        return true;
                    case ".scend":
                        return state.m_Scopes.ScopeClose(state.machineCode.Count);
                    default:
                        throw new Exception(string.Format("Unimplemented pragma in line {0}", line));
                }
            }
            return false;
        }
    }
}
