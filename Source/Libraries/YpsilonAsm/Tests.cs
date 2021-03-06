﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Assembler;

namespace Ypsilon
{
    public static class Tests
    {
        private static int m_TestCount;

        public static string Run()
        {
            m_TestCount = 0;
            List<string> list = new List<string>();

            try
            {
                // test number formats
                Test("cmp r7, 1234", (ushort)(0x0007), 1234); // test non-hex numbers
                Test("cmp r7, $1234", (ushort)(0x0007), 0x1234); // test non-hex numbers
                Test("cmp r7, 0x1234", (ushort)(0x0007), 0x1234); // test non-hex numbers

                // test alu
                string[] alu_instructions = {
                    "cmp", "neg", "add", "sub", "adc", "sbc", "mul", "div",
                    "mli", "dvi", "mod", "mdi", "and", "orr", "eor", "not",
                    "lod", "sto" };
                ushort[] alu_codes = {
                    0x0000, 0x0008, 0x0010, 0x0018, 0x0020, 0x0028, 0x0030, 0x0038,
                    0x0040, 0x0048, 0x0050, 0x0058, 0x0060, 0x0068, 0x0070, 0x0078,
                    0x0080, 0x0088 };
                string[] reg_control = { "fl", "pc", "ps", string.Empty, string.Empty, string.Empty, "usp", "sp" };

                for (int ins = 0; ins < alu_instructions.Length; ins++)
                {
                    for (int r0 = 0; r0 < 8; r0++)
                    {
                        if (alu_instructions[ins] != "sto")
                        {
                            Test($"{alu_instructions[ins]}     r{r0}, $1234", (ushort)(0x0000 | alu_codes[ins] | r0), 0x1234);
                            Test($"{alu_instructions[ins]}.8   r{r0}, $0034", (ushort)(0x0100 | alu_codes[ins] | r0), 0x0034);
                        }
                        Test($"{alu_instructions[ins]}     r{r0}, [$1234]", (ushort)(0x0200 | alu_codes[ins] | r0), 0x1234);
                        Test($"{alu_instructions[ins]}     r{r0}, ES[$1234]", (ushort)(0x8200 | alu_codes[ins] | r0), 0x1234);
                        Test($"{alu_instructions[ins]}.8   r{r0}, [$1234]", (ushort)(0x0300 | alu_codes[ins] | r0), 0x1234);
                        Test($"{alu_instructions[ins]}.8   r{r0}, ES[$1234]", (ushort)(0x8300 | alu_codes[ins] | r0), 0x1234);
                        for (int cr = 0; cr < 8; cr++)
                        {
                            if (reg_control[cr] == string.Empty)
                                continue;
                            Test($"{alu_instructions[ins]}     r{r0}, {reg_control[cr]}", (ushort)(0x0800 | alu_codes[ins] | r0 | (cr << 8)));
                        }
                        for (int r1 = 0; r1 < 8; r1++)
                        {
                            if (alu_instructions[ins] != "sto")
                            {
                                Test($"{alu_instructions[ins]}     r{r0}, r{r1}", (ushort)(0x1000 | alu_codes[ins] | r0 | (r1 << 9)));
                                Test($"{alu_instructions[ins]}.8   r{r0}, r{r1}", (ushort)(0x1100 | alu_codes[ins] | r0 | (r1 << 9)));
                            }
                            Test($"{alu_instructions[ins]}     r{r0}, [r{r1}]", (ushort)(0x2000 | alu_codes[ins] | r0 | (r1 << 9)));
                            Test($"{alu_instructions[ins]}.8   r{r0}, [r{r1}]", (ushort)(0x2100 | alu_codes[ins] | r0 | (r1 << 9)));
                            Test($"{alu_instructions[ins]}     r{r0}, ES[r{r1}]", (ushort)(0xA000 | alu_codes[ins] | r0 | (r1 << 9)));
                            Test($"{alu_instructions[ins]}.8   r{r0}, ES[r{r1}]", (ushort)(0xA100 | alu_codes[ins] | r0 | (r1 << 9)));
                            Test($"{alu_instructions[ins]}     r{r0}, [r{r1},$1234]", (ushort)(0x3000 | alu_codes[ins] | r0 | (r1 << 9)), 0x1234);
                            Test($"{alu_instructions[ins]}.8   r{r0}, [r{r1},$1234]", (ushort)(0x3100 | alu_codes[ins] | r0 | (r1 << 9)), 0x1234);
                            Test($"{alu_instructions[ins]}     r{r0}, ES[r{r1},$1234]", (ushort)(0xB000 | alu_codes[ins] | r0 | (r1 << 9)), 0x1234);
                            Test($"{alu_instructions[ins]}.8   r{r0}, ES[r{r1},$1234]", (ushort)(0xB100 | alu_codes[ins] | r0 | (r1 << 9)), 0x1234);
                            for (int r2 = 4; r2 < 8; r2++)
                            {
                                Test($"{alu_instructions[ins]}     r{r0}, [r{r1},r{r2}]", (ushort)(0x4000 | alu_codes[ins] | r0 | (r1 << 9) | ((r2 - 4) << 12)));
                                Test($"{alu_instructions[ins]}.8   r{r0}, [r{r1},r{r2}]", (ushort)(0x4100 | alu_codes[ins] | r0 | (r1 << 9) | ((r2 - 4) << 12)));
                                Test($"{alu_instructions[ins]}     r{r0}, ES[r{r1},r{r2}]", (ushort)(0xC000 | alu_codes[ins] | r0 | (r1 << 9) | ((r2 - 4) << 12)));
                                Test($"{alu_instructions[ins]}.8   r{r0}, ES[r{r1},r{r2}]", (ushort)(0xC100 | alu_codes[ins] | r0 | (r1 << 9) | ((r2 - 4) << 12)));
                            }
                        }

                    }
                }

                // test branch
                string[] bra_instructions = {
                    "bcc", "bcs", "bne", "beq", "bpl", "bmi", "bvc", "bvs",
                    "bug", "bsg", "baw" };
                ushort[] bra_codes = {
                    0x0090, 0x0091, 0x0092, 0x0093, 0x0094, 0x0095, 0x0096, 0x0097,
                    0x0098, 0x0099, 0x009F };

                for (int ins = 0; ins < bra_instructions.Length; ins++)
                {
                    for (int offset = sbyte.MinValue; offset <= sbyte.MaxValue; offset++)
                    {
                        Test($"{bra_instructions[ins]}     {offset}", (ushort)(bra_codes[ins] | ((byte)offset << 8)));
                    }
                }

                // test shift
                string[] shf_instructions = {
                    "asl", "lsl", "rol", "rnl",
                    "asr", "lsr", "ror", "rnr"
                };
                ushort[] shf_codes = {
                    0x00A0, 0x00A1, 0x00A2, 0x00A3,
                    0x00A4, 0x00A5, 0x00A6, 0x00A7
                };

                for (int ins = 0; ins < shf_instructions.Length; ins++)
                {
                    for (int r0 = 0; r0 < 8; r0++)
                    {
                        for (int offset = 1; offset <= 16; offset++)
                        {
                            Test($"{shf_instructions[ins]}     r{r0}, {offset}", 
                                (ushort)(shf_codes[ins] | ((byte)(offset - 1) << 8) | 0x0000 | (r0 << 13)));
                        }

                        for (int r1 = 0; r1 < 8; r1++)
                        {
                            Test($"{shf_instructions[ins]}     r{r0}, r{r1}",
                                (ushort)(shf_codes[ins] | (r1 << 8) | 0x1000 | (r0 << 13)));
                        }
                    }
                }

                // test bit-testing, setting, clearing
                string[] bti_instructions = {
                    "btt", "btx", "btc", "bts"
                };
                ushort[] bti_codes = {
                    0x00A8, 0x00A9, 0x00AA, 0x00AB
                };

                for (int ins = 0; ins < bti_instructions.Length; ins++)
                {
                    for (int r0 = 0; r0 < 8; r0++)
                    {
                        for (int offset = 0; offset < 16; offset++)
                        {
                            Test($"{bti_instructions[ins]}     r{r0}, {offset}",
                                (ushort)(bti_codes[ins] | ((byte)(offset) << 8) | 0x0000 | (r0 << 13)));
                        }

                        for (int r1 = 0; r1 < 8; r1++)
                        {
                            Test($"{bti_instructions[ins]}     r{r0}, r{r1}",
                                (ushort)(bti_codes[ins] | (r1 << 8) | 0x1000 | (r0 << 13)));
                        }
                    }
                }

                // test set
                for (int v = 0; v < 0x20; v++)
                    for (int r = 0; r < 8; r++)
                        Test($"set     r{r}, {v}",
                            (ushort)(0x00AC | (v << 8) | (r << 13)));
                for (int v = 0; v < 0x20; v++)
                {
                    ushort v0 = (v <= 0x0A) ?
                            (ushort)Math.Pow(2, 5 + v) :
                            (ushort)(0xFFE0 + v);
                    for (int r = 0; r < 8; r++)
                    {
                        Test($"set     r{r}, {v0}",
                            (ushort)(0x00AD | (v << 8) | (r << 13)));
                    }
                }

                // test sef / clf
                for (int flg = 1; flg < 16; flg++)
                {
                    list.Clear();
                    if ((flg & 0x01) != 0)
                        list.Add("v");
                    if ((flg & 0x02) != 0)
                        list.Add("c");
                    if ((flg & 0x04) != 0)
                        list.Add("z");
                    if ((flg & 0x08) != 0)
                        list.Add("n");
                    string flg_str = list.Select(i => i).
                        Aggregate((i, j) => i + "," + j);
                    Test($"sef     {flg_str}",
                            (ushort)(0x00AE | (flg << 12)));
                    Test($"clf     {flg_str}",
                            (ushort)(0x00AF | (flg << 12)));
                }

                // test push / pop
                for (int stk = 1; stk < 256; stk++)
                {
                    list.Clear();
                    for (int i = 0; i < 8; i++)
                    {
                        int p = (int)Math.Pow(2, i);
                        if ((stk & p) == p)
                            list.Add($"r{i}");
                    }
                    string regs = list.Select(i => i).
                        Aggregate((i, j) => i + "," + j);
                    Test($"psh     {regs}",
                            (ushort)(0x00B0 | (stk << 8)));
                    Test($"pop     {regs}",
                            (ushort)(0x00B2 | (stk << 8)));
                }

                string[] stk_crs = {
                    "fl", "pc", "ps", string.Empty, string.Empty, string.Empty, "usp", "sp" };
                for (int stk = 1; stk < 256; stk++)
                {
                    list.Clear();
                    for (int i = 0; i < 8; i++)
                    {
                        int p = (int)Math.Pow(2, i);
                        if ((stk & p) == p)
                        {
                            if (stk_crs[i] == string.Empty)
                                goto notThisReg;
                            list.Add(stk_crs[i]);
                        }
                    }
                    string regs = list.Select(i => i).
                        Aggregate((i, j) => i + "," + j);
                    Test($"psh     {regs}",
                            (ushort)(0x00B1 | (stk << 8)));
                    Test($"pop     {regs}",
                            (ushort)(0x00B3 | (stk << 8)));
                notThisReg:;
                }

                // test rts / rts.f / slp / swi / rti
                Test("rts", 0x00B4);
                Test("rts.f", 0x01B4);
                Test("rti", 0x02B4);
                Test("swi", 0x03B4);
                Test("slp", 0x04B4);

                // test lsg / ssg
                string[] mmu_instructions = { "lsg", "ssg" };
                ushort[] mmu_codes = { 0x00B5, 0x01B5 };
                string[] seg_regs = { "cs", "ds", "es", "ss" };

                for (int ins = 0; ins < mmu_instructions.Length; ins++)
                {
                    for (int sr = 0; sr < seg_regs.Length; sr++)
                    {
                        Test($"{mmu_instructions[ins]}     {seg_regs[sr]}s",
                            (ushort)(0x0000 | mmu_codes[ins] | (sr << 9)));
                        Test($"{mmu_instructions[ins]}     {seg_regs[sr]}u",
                            (ushort)(0x8000 | mmu_codes[ins] | (sr << 9)));
                    }
                    Test($"{mmu_instructions[ins]}     is",
                            (ushort)(0x0800 | mmu_codes[ins]));
                }

                // test inc / adi / dec / sbi
                for (int r = 0; r < 8; r++)
                {
                    Test($"inc r{r}",
                        (ushort)(0x00B6 | (r << 13)));
                    Test($"dec r{r}",
                        (ushort)(0x00B7 | (r << 13)));

                    for (int imm = 1; imm <= 0x20; imm++)
                    {
                        Test($"adi r{r}, {imm}",
                        (ushort)(0x00B6 | (r << 13) | ((imm - 1) << 8)));
                        Test($"sbi r{r}, {imm}",
                            (ushort)(0x00B7 | (r << 13) | ((imm - 1) << 8)));
                    }
                }

                // test jmp / jsr
                string[] jmi_instructions = { "jmp", "jsr" };
                ushort[] jmi_codes = { 0x00B8, 0x00B9 };

                for (int ins = 0; ins < jmi_instructions.Length; ins++)
                {
                    Test($"{jmi_instructions[ins]}     $1234",
                        (ushort)(0x0000 | jmi_codes[ins]), 0x1234);
                    Test($"{jmi_instructions[ins]}.f   $1234, $56789abc", 
                        (ushort)(0x0100 | jmi_codes[ins]), 0x1234, 0x9abc, 0x5678);
                    Test($"{jmi_instructions[ins]}     [$1234]",
                        (ushort)(0x0200 | jmi_codes[ins]), 0x1234);
                    Test($"{jmi_instructions[ins]}     ES[$1234]",
                        (ushort)(0x8200 | jmi_codes[ins]), 0x1234);
                    Test($"{jmi_instructions[ins]}.f   [$1234]",
                        (ushort)(0x0300 | jmi_codes[ins]), 0x1234);
                    Test($"{jmi_instructions[ins]}.f   ES[$1234]",
                        (ushort)(0x8300 | jmi_codes[ins]), 0x1234);

                    for (int r1 = 0; r1 < 8; r1++)
                    {
                        // register
                        Test($"{jmi_instructions[ins]}     r{r1}", 
                            (ushort)(0x1000 | jmi_codes[ins] | (r1 << 9)));
                        // indirect
                        Test($"{jmi_instructions[ins]}     [r{r1}]",
                            (ushort)(0x2000 | jmi_codes[ins] | (r1 << 9)));
                        Test($"{jmi_instructions[ins]}.f   [r{r1}]",
                            (ushort)(0x2100 | jmi_codes[ins] | (r1 << 9)));
                        Test($"{jmi_instructions[ins]}     ES[r{r1}]",
                            (ushort)(0xA000 | jmi_codes[ins] | (r1 << 9)));
                        Test($"{jmi_instructions[ins]}.f   ES[r{r1}]",
                            (ushort)(0xA100 | jmi_codes[ins] | (r1 << 9)));
                        // indirect offset
                        Test($"{jmi_instructions[ins]}     [r{r1},$1234]",
                            (ushort)(0x3000 | jmi_codes[ins] | (r1 << 9)), 0x1234);
                        Test($"{jmi_instructions[ins]}.f   [r{r1},$1234]",
                            (ushort)(0x3100 | jmi_codes[ins] | (r1 << 9)), 0x1234);
                        Test($"{jmi_instructions[ins]}     ES[r{r1},$1234]",
                            (ushort)(0xB000 | jmi_codes[ins] | (r1 << 9)), 0x1234);
                        Test($"{jmi_instructions[ins]}.f   ES[r{r1},$1234]",
                            (ushort)(0xB100 | jmi_codes[ins] | (r1 << 9)), 0x1234);
                        // indirect indexed
                        for (int r2 = 4; r2 < 8; r2++)
                        {
                            Test($"{jmi_instructions[ins]}     [r{r1},r{r2}]", 
                                (ushort)(0x4000 | jmi_codes[ins] | (r1 << 9) | ((r2 - 4) << 12)));
                            Test($"{jmi_instructions[ins]}.f   [r{r1},r{r2}]", 
                                (ushort)(0x4100 | jmi_codes[ins] | (r1 << 9) | ((r2 - 4) << 12)));
                            Test($"{jmi_instructions[ins]}     ES[r{r1},r{r2}]", 
                                (ushort)(0xC000 | jmi_codes[ins] | (r1 << 9) | ((r2 - 4) << 12)));
                            Test($"{jmi_instructions[ins]}.f   ES[r{r1},r{r2}]", 
                                (ushort)(0xC100 | jmi_codes[ins] | (r1 << 9) | ((r2 - 4) << 12)));
                        }
                    }
                }

                // test hwq
                for (int i = 0; i < 256; i++)
                    Test($"hwq {i}",
                        (ushort)(0x00BA | (i << 8)));

                // test stx
                for (int i = sbyte.MinValue; i <= sbyte.MaxValue; i++)
                    Test($"stx {i}",
                        (ushort)(0x00BB | (((sbyte)i) << 8)));

            }
            catch (Exception e)
            {
                return e.Message;
            }

            return $"Test successful! {m_TestCount} tests completed";
        }

        private static void Test(string asm, params ushort[] expected)
        {
            Parser p = new Parser();
            try
            {
                List<byte> assembled = p.Parse(asm, string.Empty);
                if (assembled == null)
                    throw new Exception(p.ErrorMsg);
                if (assembled.Count != expected.Length * 2)
                    throw new Exception("Failure to match: instruction {0} did not match expected bit length");
                for (int i = 0; i < expected.Length; i++)
                {
                    if (((expected[i] & 0x00ff) != assembled[i * 2]) || ((expected[i] >> 8) != assembled[i * 2 + 1]))
                    {
                        // format the expected and actual code values:
                        StringBuilder sbCode = new StringBuilder(), sbAssembled = new StringBuilder();
                        for (int j = 0; j < expected.Length; j++)
                            sbCode.Append($"{expected[j]:X4} ");
                        for (int j = 0; j < assembled.Count; j++)
                        {
                            int k = ((j % 2) == 0) ? 1 : -1;
                            sbAssembled.Append($"{assembled[j + k]:X2}");
                            if (j % 2 == 1)
                                sbAssembled.Append(" ");
                        }
                        throw new Exception(string.Format("Failure to match expected bit pattern\n" + 
                            "Expected: {1}\n" + "Actual:   {2}", 
                            asm, sbCode, sbAssembled));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{asm}\n{e.Message}", e);
            }

            m_TestCount++;

        }
    }
}
