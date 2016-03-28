/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        private bool ParsePragma(int lineIndex, string line, string opcode, List<string> tokens, ParserState state)
        {
            string pragma = opcode.ToLowerInvariant();
            if (!LineSearch.MatchPragma(pragma))
                return false;

            switch (pragma)
            {
                case ".dat8":
                    ParseData8(line.Replace(".dat8", string.Empty).Trim(), state);
                    return true;
                case ".dat16":
                    ParseData16(line.Replace(".dat16", string.Empty).Trim(), state);
                    return true;
                case ".advance":

                    break;
                case ".alias":
                {
                    int value;
                    if (tokens.Count != 3)
                        throw new Exception("alias pragma takes two parameters");
                    string alias = tokens[1];
                    if (!char.IsLetter(alias[0]))
                        throw new Exception("alias must begin with a letter");
                    if (alias.Any(t => !char.IsLetterOrDigit(t)))
                        throw new Exception("alias must be composed of letters and digits");

                    tokens[2] = tokens[2].Replace("$", "0x");
                    object convertFromString = new Int32Converter().ConvertFromString(tokens[2]);
                    if (convertFromString != null)
                        value = (int)convertFromString;
                    else
                        throw new Exception("alias pragma - ushort parameter must be an unsigned 16-bit integer");

                    Scopes.Scope scope = state.Scopes.GetLastOpenScope();
                    scope.AddAlias(alias, (ushort)value);

                    return true;
                }
                case ".alignglobals":
                {
                    int value;
                    if (tokens.Count != 2)
                        throw new Exception("alignglobals pragma takes a single parameter");
                    if (!int.TryParse(tokens[1], out value))
                        throw new Exception("alignglobals pragma parameter must be an integer");
                    if (value < 1 || value > 4)
                        throw new Exception("alignglobals pragma parameter must be an integer between 1 and 4");
                    m_Alignment = value;
                    return true;
                }
                case ".checkpc":

                    break;
                case ".org":

                    break;
                case ".incbin":
                    return IncludeBinary(tokens, state);
                case ".include":
                    return IncludeAsm(tokens, state);
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
            }

            throw new Exception($"Unimplemented pragma in line {line}");
        }
    }
}
