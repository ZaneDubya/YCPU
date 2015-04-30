/*
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

using System;
using System.Collections.Generic;

namespace YCPU.Assembler
{
    public partial class Parser : DCPU16ASM.Parser
    {
        protected Dictionary<ushort, string> m_BranchReferences;
        protected Scopes m_Scopes;
        protected string m_Directory;

        public Parser() : base()
        {
            m_BranchReferences = new Dictionary<ushort, string>();
            m_Scopes = new Scopes();
        }

        public override byte[] Parse(string[] lines, string working_directory)
        {
            m_MachineCode.Clear();
            m_BranchReferences.Clear();
            m_LabelReferences.Clear();
            MessageOuput = string.Empty;

            m_Directory = working_directory;

            try
            {
                foreach (var line in lines)
                {
                    LineCounter++;

                    string currentLine = line.Trim();
                    if (currentLine.Length < 1 || line[0] == ';')
                        continue;

                    currentLine = RemoveLineComments(currentLine);
                    if (currentLine.Trim().Length == 0)
                        continue;

                    AssembleLine(currentLine);
                }

                if (m_Scopes.OpenScopes())
                    throw new Exception("Unclosed scope.");

                SetLabelAddressReferences();
                SetBranchAddressReferences();
                SetDataFieldLabelAddressReferences();

                var count = 1;

                foreach (ushort code in m_MachineCode)
                {
                    AddMessage(string.Format("{0:X4} ", code));
                    count++;
                }

                MessageOuput = this.MessageOuput.Substring(0, MessageOuput.Length - 2);

                return m_MachineCode.ToArray();
            }
            catch (Exception ex)
            {
                AddMessageLine(string.Format("Line {0}: {1}", LineCounter, ex.Message));
                return null;
            }
        }

        protected override int ParseLabel(string line, bool local)
        {
            int index1 = line.IndexOf(' ');
            int index2 = line.IndexOf('\t');
            int index = index1 < index2 || index2 == -1 ? index1 : index2 < index1 || index1 != -1 ? index2 : -1;

            int colon_pos = line.IndexOf(':');
            string labelName;
            if (colon_pos == 0)
                labelName = index > 1 ? line.Substring(1, index - 1) : line.Substring(1, line.Length - 1);
            else
                labelName = line.Substring(0, colon_pos);

            if (!m_Scopes.AddLabel(labelName.Trim(), m_MachineCode.Count, local))
                throw new Exception(string.Format("Error adding label '{0}'.", labelName));
            return index;
        }

        protected void SetBranchAddressReferences()
        {
            foreach (ushort index in m_BranchReferences.Keys)
            {
                string labelName = m_BranchReferences[index];

                if (!m_Scopes.ContainsLabel(labelName, index))
                {
                    throw new Exception(string.Format("Unknown label reference '{0}'.", labelName));
                }

                ushort label_address = (ushort)m_Scopes.LabelAddress(labelName, index);
                int delta = label_address - index;
                if ((delta > sbyte.MaxValue) || (delta < sbyte.MinValue))
                    throw new Exception("Branch operation out of range.");
                m_MachineCode[index] = m_MachineCode[index]; // same operand; this line may not be necessary.
                m_MachineCode[index + 1] = (byte)((sbyte)delta);
            }
        }

        protected override void SetLabelAddressReferences()
        {
            foreach (ushort index in m_LabelReferences.Keys)
            {
                string labelName = this.m_LabelReferences[index];

                if (!m_Scopes.ContainsLabel(labelName, index))
                {
                    throw new Exception(string.Format("Unknown label reference '{0}'", labelName));
                }

                ushort address = (ushort)m_Scopes.LabelAddress(labelName, index);
                m_MachineCode[index] = (byte)(address & 0x00ff);
                m_MachineCode[index + 1] = (byte)((address & 0xff00) >> 8);
            }
        }

        protected override void SetDataFieldLabelAddressReferences()
        {
            foreach (ushort key in m_LabelDataFieldReferences.Keys)
            {
                string labelName = m_LabelDataFieldReferences[key];

                if (m_Scopes.ContainsLabel(labelName, key) != true)
                {
                    throw new Exception(string.Format("Unknown label '{0}' referenced in data field", labelName));
                }

                ushort address = (ushort)m_Scopes.LabelAddress(labelName, key);
                m_MachineCode[key] = (byte)(address & 0x00ff);
                m_MachineCode[key + 1] = (byte)((address & 0xff00) >> 8);
            }
        }

        protected override bool ParsePragma(string line, string opcode, string[] tokens)
        {
            if (RegEx.MatchPragma(opcode.ToLower()))
            {
                switch (opcode.ToLower())
                {
                    case ".target":
                        m_DefaultTarget = tokens[1];
                        return true;
                    case ".alu_width":
                        int bit_width;
                        if (!ParseBitWidth(tokens[1], out bit_width))
                            throw new Exception(string.Format("Unknown bit width flag '{0}' for pragma '{1}'.\nAcceptable bit widths are 8, 16, and 32.", tokens[1], opcode));
                        m_DefaultBitWidth = bit_width;
                        return true;
                    case ".dat8":
                        m_DataNextLine = ParseData8(line);
                        return true;

                    case ".dat16":
                        m_DataNextLine = ParseData16(line);
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
                        m_Scopes.ScopeOpen(m_MachineCode.Count);
                        return true;
                    case ".scend":
                        return m_Scopes.ScopeClose(m_MachineCode.Count);
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

        protected bool IncludeBinary(string[] tokens)
        {
            if (tokens.Length == 1)
                throw new Exception(string.Format("No file specified for .incbin pragma.", tokens[1]));

            tokens[1] = tokens[1].Replace("\"", string.Empty);

            byte[] data = YCPU.Platform.Common.GetBytesFromFile(m_Directory + @"\" + tokens[1]);
            if (data == null)
                throw new Exception(string.Format("Error loading file '{0}'.", tokens[1]));

            int begin = 0, length = data.Length;
            if (tokens.Length >= 3)
                if (!Int32.TryParse(tokens[2], out begin))
                    throw new Exception(string.Format("Third paramter for incbin must be numeric."));

            if (tokens.Length == 3)
            {
                length = length - begin;
            }

            if (tokens.Length == 4)
                if (!Int32.TryParse(tokens[3], out length))
                    throw new Exception(string.Format("Fourth paramter for incbin must be numeric."));

            if ((begin >= length) || (begin + length > data.Length))
                throw new Exception("Out of bounds for incbin.");

            for (int i = 0; i < length; i++)
                m_MachineCode.Add(data[i + begin]);
            return true;
        }

        

        #region Initialization of opcode and register names
        protected override void InitOpcodeDictionary()
        {
            // alu instructions
            m_OpcodeAssemblers.Add("lod", AssembleLOD);
            m_OpcodeAssemblers.Add("sto", AssembleSTO);
            m_OpcodeAssemblers.Add("add", AssembleADD);
            m_OpcodeAssemblers.Add("sub", AssembleSUB);
            m_OpcodeAssemblers.Add("adc", AssembleADC);
            m_OpcodeAssemblers.Add("sbc", AssembleSBC);
            m_OpcodeAssemblers.Add("mul", AssembleMUL);
            m_OpcodeAssemblers.Add("div", AssembleDIV);
            m_OpcodeAssemblers.Add("mli", AssembleMLI);
            m_OpcodeAssemblers.Add("dvi", AssembleDVI);
            m_OpcodeAssemblers.Add("mod", AssembleMOD);
            m_OpcodeAssemblers.Add("mdi", AssembleMDI);
            m_OpcodeAssemblers.Add("and", AssembleAND);
            m_OpcodeAssemblers.Add("orr", AssembleORR);
            m_OpcodeAssemblers.Add("eor", AssembleEOR);
            m_OpcodeAssemblers.Add("not", AssembleNOT);
            m_OpcodeAssemblers.Add("cmp", AssembleCMP);
            m_OpcodeAssemblers.Add("neg", AssembleNEG);
            // branch instructions
            m_OpcodeAssemblers.Add("bcc", AssembleBCC);
            m_OpcodeAssemblers.Add("buf", AssembleBCC);
            m_OpcodeAssemblers.Add("bcs", AssembleBCS);
            m_OpcodeAssemblers.Add("buh", AssembleBCS);
            m_OpcodeAssemblers.Add("bne", AssembleBNE);
            m_OpcodeAssemblers.Add("beq", AssembleBEQ);
            m_OpcodeAssemblers.Add("bpl", AssembleBPL);
            m_OpcodeAssemblers.Add("bsf", AssembleBPL);
            m_OpcodeAssemblers.Add("bmi", AssembleBMI);
            m_OpcodeAssemblers.Add("bsh", AssembleBMI);
            m_OpcodeAssemblers.Add("bvc", AssembleBVC);
            m_OpcodeAssemblers.Add("bvs", AssembleBVS);
            m_OpcodeAssemblers.Add("bug", AssembleBUG);
            m_OpcodeAssemblers.Add("bsg", AssembleBSG);
            m_OpcodeAssemblers.Add("baw", AssembleBAW);
            // shift instructions
            m_OpcodeAssemblers.Add("asl", AssembleASL);
            m_OpcodeAssemblers.Add("lsl", AssembleLSL);
            m_OpcodeAssemblers.Add("rol", AssembleROL);
            m_OpcodeAssemblers.Add("rnl", AssembleRNL);
            m_OpcodeAssemblers.Add("asr", AssembleASR);
            m_OpcodeAssemblers.Add("lsr", AssembleLSR);
            m_OpcodeAssemblers.Add("ror", AssembleROR);
            m_OpcodeAssemblers.Add("rnr", AssembleRNR);
            // bit testing operations
            m_OpcodeAssemblers.Add("bit", AssembleBIT);
            m_OpcodeAssemblers.Add("btx", AssembleBTX);
            m_OpcodeAssemblers.Add("btc", AssembleBTC);
            m_OpcodeAssemblers.Add("bts", AssembleBTS);
            // switch octet
            m_OpcodeAssemblers.Add("swo", AssembleSWO);
            // fpu testing operations
            m_OpcodeAssemblers.Add("fpa", null);
            m_OpcodeAssemblers.Add("fps", null);
            m_OpcodeAssemblers.Add("fpm", null);
            m_OpcodeAssemblers.Add("fpd", null);
            // flag operations
            m_OpcodeAssemblers.Add("sef", AssembleSEF);
            m_OpcodeAssemblers.Add("clf", AssembleCLF);
            // stack operations
            m_OpcodeAssemblers.Add("psh", AssemblePSH);
            m_OpcodeAssemblers.Add("pop", AssemblePOP);
            // increment / decrement
            m_OpcodeAssemblers.Add("inc", AssembleINC);
            m_OpcodeAssemblers.Add("adi", AssembleADI);
            m_OpcodeAssemblers.Add("dec", AssembleDEC);
            m_OpcodeAssemblers.Add("sbi", AssembleSBI);
            // transfer special
            m_OpcodeAssemblers.Add("tsr", AssembleTSR);
            m_OpcodeAssemblers.Add("trs", AssembleTRS);
            // MMU operations
            m_OpcodeAssemblers.Add("mmr", AssembleMMR);
            m_OpcodeAssemblers.Add("mmw", AssembleMMW);
            m_OpcodeAssemblers.Add("mml", AssembleMML);
            m_OpcodeAssemblers.Add("mms", AssembleMMS);
            // jump operations
            m_OpcodeAssemblers.Add("jmp", AssembleJMP);
            m_OpcodeAssemblers.Add("jsr", AssembleJSR);
            m_OpcodeAssemblers.Add("jum", AssembleJUM);
            m_OpcodeAssemblers.Add("jcx", AssembleJCX);
            // other instructions
            m_OpcodeAssemblers.Add("hwq", AssembleHWQ);
            m_OpcodeAssemblers.Add("slp", AssembleSLP);
            m_OpcodeAssemblers.Add("swi", AssembleSWI);
            m_OpcodeAssemblers.Add("rti", AssembleRTI);
            // macros
            m_OpcodeAssemblers.Add("rts", AssembleRTS);
        }

        protected override void InitRegisterDictionary()
        {
            m_RegisterDictionary.Add("r0", (ushort)YCPUReg.R0);
            m_RegisterDictionary.Add("r1", (ushort)YCPUReg.R1);
            m_RegisterDictionary.Add("r2", (ushort)YCPUReg.R2);
            m_RegisterDictionary.Add("r3", (ushort)YCPUReg.R3);
            m_RegisterDictionary.Add("r4", (ushort)YCPUReg.R4);
            m_RegisterDictionary.Add("r5", (ushort)YCPUReg.R5);
            m_RegisterDictionary.Add("r6", (ushort)YCPUReg.R6);
            m_RegisterDictionary.Add("r7", (ushort)YCPUReg.R7);

            m_RegisterDictionary.Add("a", (ushort)YCPUReg.R0);
            m_RegisterDictionary.Add("b", (ushort)YCPUReg.R1);
            m_RegisterDictionary.Add("c", (ushort)YCPUReg.R2);
            m_RegisterDictionary.Add("i", (ushort)YCPUReg.R3);
            m_RegisterDictionary.Add("j", (ushort)YCPUReg.R4);
            m_RegisterDictionary.Add("x", (ushort)YCPUReg.R5);
            m_RegisterDictionary.Add("y", (ushort)YCPUReg.R6);
            m_RegisterDictionary.Add("z", (ushort)YCPUReg.R7);

            m_RegisterDictionary.Add("sp", (ushort)YCPUReg.R7);
        }
        #endregion
    }
}
