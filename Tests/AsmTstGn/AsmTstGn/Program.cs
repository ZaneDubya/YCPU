using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ypsilon.Assembler;

namespace AsmTstGn
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamWriter file;

            Directory.CreateDirectory("..\\..\\..\\..\\bld");

            using (file = new System.IO.StreamWriter("..\\..\\..\\..\\bld\\AsmTstGn-ALU.asm"))
                file.WriteLine(generateALU());
            using (file = new System.IO.StreamWriter("..\\..\\..\\..\\bld\\AsmTstGn-BRA.asm"))
                file.WriteLine(generateBRA());
            using (file = new System.IO.StreamWriter("..\\..\\..\\..\\bld\\AsmTstGn-SHF.asm"))
                file.WriteLine(generateSHF());
            using (file = new System.IO.StreamWriter("..\\..\\..\\..\\bld\\AsmTstGn-BTT.asm"))
                file.WriteLine(generateBTT());
        }

        static string generateALU()
        {
            // generate alu ops
            string[] ins = new string[] { 
                "cmp", "neg", "add", "sub",
                "adc", "sbc", "mul", "div",
                "mli", "dvi", "mod", "mdi",
                "and", "orr", "eor", "not",
                "lod", "sto" };

            AddressingMode[] modes = new AddressingMode[] {
                AddressingMode.Immediate, AddressingMode.Absolute, 
                AddressingMode.Register, AddressingMode.Indirect,
                AddressingMode.IndirectOffset, AddressingMode.StackAccess,
                AddressingMode.IndirectPostInc, AddressingMode.IndirectPreDec,
                AddressingMode.IndirectIndexed };

            StringBuilder sb = new StringBuilder();
            generateInstructions2p(sb, ins, modes, "$FFEE");
            return sb.ToString();
        }

        static string generateBRA()
        {
            string[] ins = new string[] { 
                "bcc", "buf", "bcs", "buh",
                "bne", "beq", "bpl", "bsf",
                "bmi", "bsh", "bvc", "bvs",
                "bug", "bsg", "baw" };

            AddressingMode[] modes = new AddressingMode[] {
                AddressingMode.Immediate };

            StringBuilder sb = new StringBuilder();
            generateInstructions1p(sb, ins, modes, "$7F");
            return sb.ToString();

        }

        static string generateSHF()
        {
            string[] ins = new string[] { 
                "asl", "lsl", "rol", "rnl",
                "asr", "lsr", "ror", "rnr" };

            AddressingMode[] modes = new AddressingMode[] {
                AddressingMode.Immediate, AddressingMode.Register };

            StringBuilder sb = new StringBuilder();
            generateInstructions2p(sb, ins, modes, "13");
            return sb.ToString();
        }

        static string generateBTT()
        {
            string[] ins = new string[] { 
                "btt", "btx", "btc", "bts" };

            AddressingMode[] modes = new AddressingMode[] {
                AddressingMode.Immediate, AddressingMode.Register };

            StringBuilder sb = new StringBuilder();
            generateInstructions2p(sb, ins, modes, "11");
            return sb.ToString();
        }

        static void generateInstructions1p(StringBuilder sb, string[] instructions, AddressingMode[] modes, string immediate)
        {
            foreach (string instruction in instructions)
            {
                foreach (AddressingMode mode in modes)
                {
                    string addr;
                    switch (mode)
                    {
                        case AddressingMode.Immediate:
                            if (instruction == "sto")
                                continue;
                            addr = immediate;
                            sb.AppendLine(string.Format("{0}     {1}", instruction, addr));
                            break;
                        case AddressingMode.Absolute:
                            addr = immediate;
                            sb.AppendLine(string.Format("{0}     [{1}]", instruction, addr));
                            break;
                        case AddressingMode.Register:
                            if (instruction == "sto")
                                continue;
                            for (int r1 = 0; r1 < 8; r1++)
                                sb.AppendLine(string.Format("{0}    {1}", instruction, r1));
                            break;
                        case AddressingMode.Indirect:
                            for (int r1 = 0; r1 < 8; r1++)
                                sb.AppendLine(string.Format("{0}     [{1}]", instruction, r1));
                            break;
                        case AddressingMode.IndirectOffset:
                            for (int r1 = 0; r1 < 8; r1++)
                                sb.AppendLine(string.Format("{0}     [r{1},$007F]", instruction, r1));
                            break;
                        case AddressingMode.StackAccess:
                            for (int s = 0; s < 8; s++)
                                sb.AppendLine(string.Format("{0}     S[${1}]", instruction, s));
                            break;
                        case AddressingMode.IndirectPostInc:
                            for (int r1 = 0; r1 < 8; r1++)
                                sb.AppendLine(string.Format("{0}     [r{1}+]", instruction, r1));
                            break;
                        case AddressingMode.IndirectPreDec:
                            for (int r1 = 0; r1 < 8; r1++)
                                sb.AppendLine(string.Format("{0}     [-r{1}]", instruction, r1));
                            break;
                        case AddressingMode.IndirectIndexed:
                            for (int r1 = 0; r1 < 8; r1++)
                                for (int r2 = 0; r2 < 8; r2++)
                                    sb.AppendLine(string.Format("{0}     [r{1},r{2}]", instruction, r1, r2));
                            break;
                        default:
                            throw new Exception();
                    }
                }
            }
        }

        static void generateInstructions2p(StringBuilder sb, string[] instructions, AddressingMode[] modes, string immediate)
        {
            foreach (string instruction in instructions)
            {
                for (int r = 0; r < 8; r++)
                {
                    foreach (AddressingMode mode in modes)
                    {
                        string addr;
                        switch (mode)
                        {
                            case AddressingMode.Immediate:
                                if (instruction == "sto")
                                    continue;
                                addr = immediate;
                                sb.AppendLine(string.Format("{0}     r{1}, {2}", instruction, r, addr));
                                break;
                            case AddressingMode.Absolute:
                                addr = immediate;
                                sb.AppendLine(string.Format("{0}     r{1}, [{2}]", instruction, r, addr));
                                break;
                            case AddressingMode.Register:
                                if (instruction == "sto")
                                    continue;
                                for (int r1 = 0; r1 < 8; r1++)
                                    sb.AppendLine(string.Format("{0}     r{1}, r{2}", instruction, r, r1));
                                break;
                            case AddressingMode.Indirect:
                                for (int r1 = 0; r1 < 8; r1++)
                                    sb.AppendLine(string.Format("{0}     r{1}, [r{2}]", instruction, r, r1));
                                break;
                            case AddressingMode.IndirectOffset:
                                for (int r1 = 0; r1 < 8; r1++)
                                    sb.AppendLine(string.Format("{0}     r{1}, [r{2},$DDCC]", instruction, r, r1));
                                break;
                            case AddressingMode.StackAccess:
                                for (int s = 0; s < 8; s++)
                                    sb.AppendLine(string.Format("{0}     r{1}, S[${2}]", instruction, r, s));
                                break;
                            case AddressingMode.IndirectPostInc:
                                for (int r1 = 0; r1 < 8; r1++)
                                    sb.AppendLine(string.Format("{0}     r{1}, [r{2}+]", instruction, r, r1));
                                break;
                            case AddressingMode.IndirectPreDec:
                                for (int r1 = 0; r1 < 8; r1++)
                                    sb.AppendLine(string.Format("{0}     r{1}, [-r{2}]", instruction, r, r1));
                                break;
                            case AddressingMode.IndirectIndexed:
                                for (int r1 = 0; r1 < 8; r1++)
                                    for (int r2 = 0; r2 < 8; r2++)
                                        sb.AppendLine(string.Format("{0}     r{1}, [r{2},r{3}]", instruction, r, r1, r2));
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                }
            }
        }
    }
}
