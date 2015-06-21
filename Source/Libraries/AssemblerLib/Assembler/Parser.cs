/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ypsilon.Assembler
{
    public partial class Parser
    {
        public Parser()
        {
            Initialize();
        }

        public byte[] Parse(string code, string workingDirectory)
        {
            ParserState state = new ParserState();
            state.WorkingDirectory = workingDirectory;
            string[] lines = code.Split('\n');

            int indexOfCurrentLine = 0;

            // pass 1: assemble the assembly file into machine code
            foreach (var line in lines)
            {
                indexOfCurrentLine++;

                // trim whitespace at tail/end and discard empty lines
                string currentLine = StripComments(line).Trim();
                if (currentLine.Length == 0 || currentLine[0] == ';')
                    continue;

                try
                {
                    AssembleLine(indexOfCurrentLine, currentLine, state);
                }
                catch (Exception ex)
                {
                    AddMessageLine(string.Format("Line {0}: {1}.", indexOfCurrentLine, ex.Message));
                    ErrorLine = indexOfCurrentLine;
                    return null;
                }
            }

            // check for open scopes...
            Scopes.Scope openScope;
            if (state.Scopes.TryGetOpenScope(out openScope))
            {
                AddMessageLine(string.Format("Unclosed scope beginning at line {0}.", openScope.StartLine));
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
                AddMessageLine(string.Format("Error while updating labels. {0}", ex.Message));
                ErrorLine = indexOfCurrentLine;
                return null;
            }

            // return the assembled code!
            return state.Code.ToArray();
        }

        string StripComments(string line)
        {
            string clearedLine = line;
            int commentIndex = line.IndexOf(';');

            if (commentIndex == 0)
                return string.Empty;
            else if (commentIndex > 0)
                clearedLine = line.Substring(0, commentIndex).Trim();

            return clearedLine;
        }

        void AssembleLine(int lineIndex, string line, ParserState state)
        {
            line = line.Trim();

            if (LineSearch.MatchLabel(line))
            {
                // parse label and determine if there is anything else to parse on this line.
                int remaiderLineContentIndex = ParseLabel(line, state);
                if (remaiderLineContentIndex <= 0)
                    return;
                // if there is something left to parse, trim it and then interpret it as its own line.
                line = line.Remove(0, remaiderLineContentIndex).Trim();
                if (line.Length == 0)
                    return;
            }

            string[] tokens = this.Tokenize(line);
            string opcode = tokens[0].Trim();

            if (ParsePragma(lineIndex, line, opcode, tokens, state))
            {
                // Successfully parsed a pragma, no need to continue with this line.
                return;
            }

            OpcodeFlag opcodeFlag = OpcodeFlag.BitWidth16; // default to operating on 16 bits

            // Look for flags on operands (xxx.yyy, where y is the flag).
            if (opcode.Contains('.') && (opcode.IndexOf('.') > 1) && (opcode.Length - opcode.IndexOf('.') - 1 > 0))
            {
                // opcode has a flag
                string flag = opcode.Substring(opcode.IndexOf('.') + 1);
                if (!ParseOpcodeFlag(flag, ref opcodeFlag))
                    throw new Exception(string.Format("Unknown bit width flag '{0}' for instruction '{1}'.", flag, line));
                opcode = opcode.Substring(0, opcode.IndexOf('.'));
            }

            // get the assembler for this opcode. If no assembler exists, throw error.
            Func<string[], OpcodeFlag, ParserState, ushort[]> assembler;
            if (!m_Opcodes.TryGetValue(opcode.ToLower(), out assembler))
            {
                throw new Exception(string.Format("Undefined command in line {0}", line));
            }

            // get the parameters
            List<string> param = new List<string>();
            for (int i = 1; i < tokens.Length; i++)
                param.Add(tokens[i].Trim());

            // pass the params to the opcode's assembler. If no output, throw error.
            ushort[] code = assembler(param.ToArray(), opcodeFlag, state);
            if (code == null)
                throw new Exception(string.Format("Error assembling line {0}", line));

            // add the output of the assembler to the machine code output. 
            for (int i = 0; i < code.Length; i++)
            {
                ushort this_opcode = code[i];
                state.Code.Add((byte)(this_opcode & 0x00ff));
                state.Code.Add((byte)((this_opcode & 0xff00) >> 8));
            }
        }

        bool ParseOpcodeFlag(string value, ref OpcodeFlag opcodeFlag)
        {
            switch (value)
            {
                case "8":
                    opcodeFlag &= ~OpcodeFlag.BitWidthsAll;
                    opcodeFlag = OpcodeFlag.BitWidth8;
                    return true;
                case "16":
                    opcodeFlag &= ~OpcodeFlag.BitWidthsAll;
                    opcodeFlag = OpcodeFlag.BitWidth16;
                    return true;
                case "f":
                    opcodeFlag |= OpcodeFlag.FarJump;
                    return true;
                default:
                    return false;
            }
        }

        string[] Tokenize(string data)
        {
            string[] tokens = data.Split(new[] { ' ', '\t', ',' });
            for (int i = 0; i < tokens.Length - 1; i++)
                if (((tokens[i].Length > 0) && (tokens[i + 1].Length > 0)) && (tokens[i][0] == '[') && (tokens[i + 1][tokens[i + 1].Length - 1] == ']'))
                {
                    tokens[i] = tokens[i] + ',' + tokens[i + 1];
                    tokens[i + 1] = string.Empty;
                }
            return tokens.Where(t => t.Trim() != string.Empty).ToArray();
        }
    }
}
