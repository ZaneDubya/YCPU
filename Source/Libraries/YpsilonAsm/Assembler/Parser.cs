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
    public partial class Parser
    {
        private List<string> m_Lines;
        private int m_CurrentLine;
        private int m_Alignment = 1;

        public Parser()
        {
            Initialize();
        }

        public List<byte> Parse(string code, string workingDirectory)
        {
            ParserState state = new ParserState(this);
            state.WorkingDirectory = workingDirectory;

            m_Lines = Common.SplitString(code, "\n");
            m_CurrentLine = 0;

            // pass 1: assemble the assembly file into machine code
            for (int i = 0; i < m_Lines.Count; i++)
            {
                string line = m_Lines[i];
                m_CurrentLine++;

                // trim whitespace at tail/end and discard empty lines
                string stripped = StripComments(line);
                if (string.IsNullOrEmpty(stripped))
                {
                    // do nothing, empty line
                }
                else
                {
                    string currentLine = stripped.Trim();
                    if (currentLine.Length == 0)
                        continue;

                    try
                    {
                        AssembleLine(m_CurrentLine, currentLine, state);
                    }
                    catch (Exception ex)
                    {
                        AddMessageLine($"Line {m_CurrentLine} ({m_Lines[m_CurrentLine].Trim()}): {ex.Message}");
                        ErrorLine = m_CurrentLine;
                        return null;
                    }
                }
            }

            // check for open scopes...
            Scopes.Scope openScope;
            if (state.Scopes.TryGetOpenScope(out openScope))
            {
                AddMessageLine($"Unclosed scope beginning at line {openScope.StartLine}");
                ErrorLine = openScope.StartLine;
                return null;
            }

            // pass 2: update all labels.
            try
            {
                state.UpdateLabelReferences();
            }
            catch (Exception ex)
            {
                AddMessageLine($"Error while updating labels. {ex.Message}");
                ErrorLine = m_CurrentLine;
                return null;
            }

            // return the assembled code!
            return state.Code;
        }

        /// <summary>
        /// Strip comments from line. If line is empty, returns empty string.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string StripComments(string line)
        {
            string clearedLine = line;
            int commentIndex = line.IndexOf(';');

            if (commentIndex == 0)
                return string.Empty;
            if (commentIndex > 0)
                clearedLine = line.Substring(0, commentIndex).Trim();

            return clearedLine;
        }

        private void AssembleLine(int lineIndex, string line, ParserState state)
        {
            line = line.Trim();

            if (LineSearch.MatchLabel(line))
            {
                if (!state.Scopes.IsScopeOpen) // global scope
                {
                    while ((state.Code.Count % m_Alignment) != 0)
                        state.Code.Add(0x00);
                }

                // parse label and determine if there is anything else to parse on this line.
                int remaiderLineContentIndex = ParseLabel(line, state);
                if (remaiderLineContentIndex <= 0)
                    return;
                // if there is something left to parse, trim it and then interpret it as its own line.
                line = line.Remove(0, remaiderLineContentIndex).Trim();
                if (line.Length == 0)
                    return;
            }

            List<string> tokens = Tokenize(line);
            string opcode = tokens[0];
            opcode = opcode.Trim();

            if (ParsePragma(lineIndex, line, opcode, tokens, state))
            {
                // Successfully parsed a pragma, no need to continue with this line.
                return;
            }

            OpcodeFlag opcodeFlag = OpcodeFlag.BitWidth16; // default to operating on 16 bits

            // Look for flags on operands (xxx.yyy, where y is the flag).
            if (opcode.IndexOf('.') != -1 && (opcode.IndexOf('.') > 1) && (opcode.Length - opcode.IndexOf('.') - 1 > 0))
            {
                // opcode has a flag
                string flag = opcode.Substring(opcode.IndexOf('.') + 1);
                if (!ParseOpcodeFlag(flag, ref opcodeFlag))
                    throw new Exception($"Unknown bit width flag '{flag}' for instruction '{line}'");
                opcode = opcode.Substring(0, opcode.IndexOf('.'));
            }

            // get the assembler for this opcode. If no assembler exists, throw error.
            Func<List<string>, OpcodeFlag, ParserState, List<ushort>> assembler;
            if (m_Opcodes.ContainsKey(opcode.ToLowerInvariant()))
            {
                assembler = m_Opcodes[opcode.ToLowerInvariant()];
            }
            else
            {
                throw new Exception($"Undefined instruction in line \"{line}\"");
            }

            // get the parameters
            List<string> param = new List<string>();
            for (int i = 1; i < tokens.Count; i++)
                param.Add(tokens[i].Trim());

            // pass the params to the opcode's assembler. If no output, throw error.
            List<ushort> code = assembler(param, opcodeFlag, state);
            if (code == null)
                throw new Exception($"Error assembling line {line}");

            // add the output of the assembler to the machine code output. 
            for (int i = 0; i < code.Count; i++)
            {
                ushort this_opcode = code[i];
                state.Code.Add((byte)(this_opcode & 0x00ff));
                state.Code.Add((byte)((this_opcode & 0xff00) >> 8));
            }
        }

        private bool ParseOpcodeFlag(string value, ref OpcodeFlag opcodeFlag)
        {
            switch (value)
            {
                case "8":
                    opcodeFlag &= ~OpcodeFlag.BitWidthsAll;
                    opcodeFlag |= OpcodeFlag.BitWidth8;
                    return true;
                case "16":
                    opcodeFlag &= ~OpcodeFlag.BitWidthsAll;
                    opcodeFlag |= OpcodeFlag.BitWidth16;
                    return true;
                case "f":
                    opcodeFlag |= OpcodeFlag.FarJump;
                    return true;
                default:
                    return false;
            }
        }

        private List<string> Tokenize(string data)
        {
            List<string> tokens = Common.SplitString(data, " ", "\t", ",");
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                string token = tokens[i];
                string nextToken = tokens[i + 1];
                if (((token.Length <= 0) || (nextToken.Length <= 0)) || (token.IndexOf('[') == -1) || (nextToken.IndexOf(']') != nextToken.Length - 1))
                    continue;

                tokens[i] = tokens[i] + "," + tokens[i + 1];
                tokens[i + 1] = string.Empty;
            }

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] != string.Empty)
                    continue;
                tokens.RemoveAt(i);
                i -= 1;
            }

            return tokens;
        }
    }
}
