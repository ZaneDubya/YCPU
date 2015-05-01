using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Assembler
{
    partial class Parser
    {
        protected bool ParsePragma(string line, string opcode, string[] tokens)
        {
            if (RegEx.MatchPragma(opcode.ToLower()))
            {
                switch (opcode.ToLower())
                {
                    case ".dat8":
                        m_IsNextLineData = ParseData8(line);
                        return true;

                    case ".dat16":
                        m_IsNextLineData = ParseData16(line);
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
                        return IncludeBinary(tokens);

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
                        m_Scopes.ScopeOpen(m_MachineCodeOutput.Count);
                        return true;
                    case ".scend":
                        return m_Scopes.ScopeClose(m_MachineCodeOutput.Count);
                    case ".data":

                        break;
                    case ".text":

                        break;
                    default:
                        throw new Exception(string.Format("Unimplemented pragma in line {0}", line));
                }
            }
            return false;
        }
    }
}
