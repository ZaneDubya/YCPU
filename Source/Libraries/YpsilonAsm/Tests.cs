using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Assembler;

namespace Ypsilon
{
    public static class Tests
    {
        private static int m_TestCount = 0;

        public static string Run()
        {
            m_TestCount = 0;

            try
            {
                // test number formats
                Test(string.Format("cmp r7, 1234"), (ushort)(0x0007), 1234); // test non-hex numbers
                Test(string.Format("cmp r7, $1234"), (ushort)(0x0007), 0x1234); // test non-hex numbers
                Test(string.Format("cmp r7, 0x1234"), (ushort)(0x0007), 0x1234); // test non-hex numbers

                // test alu
                string[] alu_instructions = new string[] {
                    "cmp", "neg", "add", "sub", "adc", "sbc", "mul", "div",
                    "mli", "dvi", "mod", "mdi", "and", "orr", "eor", "not",
                    "lod", "sto" };
                ushort[] alu_codes = new ushort[] {
                    0x0000, 0x0008, 0x0010, 0x0018, 0x0020, 0x0028, 0x0030, 0x0038,
                    0x0040, 0x0048, 0x0050, 0x0058, 0x0060, 0x0068, 0x0070, 0x0078,
                    0x0080, 0x0088 };
                string[] reg_control = new string[] { "fl", "pc", "ps", "p2", "ii", "ia", "usp", "sp" };

                for (int ins = 0; ins < alu_instructions.Length; ins++)
                {
                    for (int r0 = 0; r0 < 8; r0++)
                    {
                        if (alu_instructions[ins] != "sto")
                        {
                            Test(string.Format("{0}     r{1}, $1234", alu_instructions[ins], r0), (ushort)(0x0000 | alu_codes[ins] | r0), 0x1234);
                            Test(string.Format("{0}.8   r{1}, $0034", alu_instructions[ins], r0), (ushort)(0x0100 | alu_codes[ins] | r0), 0x0034);
                        }
                        Test(string.Format("{0}     r{1}, [$1234]", alu_instructions[ins], r0), (ushort)(0x0200 | alu_codes[ins] | r0), 0x1234);
                        Test(string.Format("{0}     r{1}, ES[$1234]", alu_instructions[ins], r0), (ushort)(0x8200 | alu_codes[ins] | r0), 0x1234);
                        Test(string.Format("{0}.8   r{1}, [$1234]", alu_instructions[ins], r0), (ushort)(0x0300 | alu_codes[ins] | r0), 0x1234);
                        Test(string.Format("{0}.8   r{1}, ES[$1234]", alu_instructions[ins], r0), (ushort)(0x8300 | alu_codes[ins] | r0), 0x1234);
                        for (int cr = 0; cr < 8; cr++)
                        {
                            Test(string.Format("{0}     r{1}, {2}", alu_instructions[ins], r0, reg_control[cr]), (ushort)(0x0800 | alu_codes[ins] | r0 | (cr << 8)));
                        }
                        for (int r1 = 0; r1 < 8; r1++)
                        {
                            if (alu_instructions[ins] != "sto")
                            {
                                Test(string.Format("{0}     r{1}, r{2}", alu_instructions[ins], r0, r1), (ushort)(0x1000 | alu_codes[ins] | r0 | (r1 << 9)));
                                Test(string.Format("{0}.8   r{1}, r{2}", alu_instructions[ins], r0, r1), (ushort)(0x1100 | alu_codes[ins] | r0 | (r1 << 9)));
                            }
                            Test(string.Format("{0}     r{1}, [r{2}]", alu_instructions[ins], r0, r1), (ushort)(0x2000 | alu_codes[ins] | r0 | (r1 << 9)));
                            Test(string.Format("{0}.8   r{1}, [r{2}]", alu_instructions[ins], r0, r1), (ushort)(0x2100 | alu_codes[ins] | r0 | (r1 << 9)));
                            Test(string.Format("{0}     r{1}, ES[r{2}]", alu_instructions[ins], r0, r1), (ushort)(0xA000 | alu_codes[ins] | r0 | (r1 << 9)));
                            Test(string.Format("{0}.8   r{1}, ES[r{2}]", alu_instructions[ins], r0, r1), (ushort)(0xA100 | alu_codes[ins] | r0 | (r1 << 9)));
                            Test(string.Format("{0}     r{1}, [r{2},$1234]", alu_instructions[ins], r0, r1), (ushort)(0x3000 | alu_codes[ins] | r0 | (r1 << 9)), 0x1234);
                            Test(string.Format("{0}.8   r{1}, [r{2},$1234]", alu_instructions[ins], r0, r1), (ushort)(0x3100 | alu_codes[ins] | r0 | (r1 << 9)), 0x1234);
                            Test(string.Format("{0}     r{1}, ES[r{2},$1234]", alu_instructions[ins], r0, r1), (ushort)(0xB000 | alu_codes[ins] | r0 | (r1 << 9)), 0x1234);
                            Test(string.Format("{0}.8   r{1}, ES[r{2},$1234]", alu_instructions[ins], r0, r1), (ushort)(0xB100 | alu_codes[ins] | r0 | (r1 << 9)), 0x1234);
                            for (int r2 = 4; r2 < 8; r2++)
                            {
                                Test(string.Format("{0}     r{1}, [r{2},r{3}]", alu_instructions[ins], r0, r1, r2), (ushort)(0x4000 | alu_codes[ins] | r0 | (r1 << 9) | ((r2 - 4) << 12)));
                                Test(string.Format("{0}.8   r{1}, [r{2},r{3}]", alu_instructions[ins], r0, r1, r2), (ushort)(0x4100 | alu_codes[ins] | r0 | (r1 << 9) | ((r2 - 4) << 12)));
                                Test(string.Format("{0}     r{1}, ES[r{2},r{3}]", alu_instructions[ins], r0, r1, r2), (ushort)(0xC000 | alu_codes[ins] | r0 | (r1 << 9) | ((r2 - 4) << 12)));
                                Test(string.Format("{0}.8   r{1}, ES[r{2},r{3}]", alu_instructions[ins], r0, r1, r2), (ushort)(0xC100 | alu_codes[ins] | r0 | (r1 << 9) | ((r2 - 4) << 12)));
                            }
                        }

                    }
                }

                // test branch
                string[] bra_instructions = new string[] {
                    "bcc", "bcs", "bne", "beq", "bpl", "bmi", "bvc", "bvs",
                    "bug", "bsg", "baw" };
                ushort[] bra_codes = new ushort[] {
                    0x0090, 0x0091, 0x0092, 0x0093, 0x0094, 0x0095, 0x0096, 0x0097,
                    0x0098, 0x0099, 0x009F };

                for (int ins = 0; ins < bra_instructions.Length; ins++)
                {
                    for (int offset = sbyte.MinValue; offset <= sbyte.MaxValue; offset++)
                    {
                        Test(string.Format("{0}     {1}", bra_instructions[ins], offset), (ushort)(bra_codes[ins] | ((byte)offset << 8)));
                    }
                }

                // test shift
                string[] shf_instructions = new string[] {
                    "asl", "lsl", "rol", "rnl",
                    "asr", "lsr", "ror", "rnr"
                };
                ushort[] shf_codes = new ushort[] {
                    0x00A0, 0x00A1, 0x00A2, 0x00A3,
                    0x00A4, 0x00A5, 0x00A6, 0x00A7
                };

                for (int ins = 0; ins < shf_instructions.Length; ins++)
                {
                    for (int r0 = 0; r0 < 8; r0++)
                    {
                        for (int offset = 1; offset <= 16; offset++)
                        {
                            Test(string.Format("{0}     r{1}, {2}", shf_instructions[ins], r0, offset), 
                                (ushort)(shf_codes[ins] | ((byte)(offset - 1) << 8) | 0x0000 | (r0 << 13)));
                        }

                        for (int r1 = 0; r1 < 8; r1++)
                        {
                            Test(string.Format("{0}     r{1}, r{2}", shf_instructions[ins], r0, r1),
                                (ushort)(shf_codes[ins] | (r1 << 8) | 0x1000 | (r0 << 13)));
                        }
                    }
                }

                // test bit-testing, setting, clearing
                string[] bti_instructions = new string[] {
                    "btt", "btx", "btc", "bts"
                };
                ushort[] bti_codes = new ushort[] {
                    0x00A8, 0x00A9, 0x00AA, 0x00AB
                };

                for (int ins = 0; ins < bti_instructions.Length; ins++)
                {
                    for (int r0 = 0; r0 < 8; r0++)
                    {
                        for (int offset = 0; offset < 16; offset++)
                        {
                            Test(string.Format("{0}     r{1}, {2}", bti_instructions[ins], r0, offset),
                                (ushort)(bti_codes[ins] | ((byte)(offset) << 8) | 0x0000 | (r0 << 13)));
                        }

                        for (int r1 = 0; r1 < 8; r1++)
                        {
                            Test(string.Format("{0}     r{1}, r{2}", bti_instructions[ins], r0, r1),
                                (ushort)(bti_codes[ins] | (r1 << 8) | 0x1000 | (r0 << 13)));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return string.Format("Test successful! {0} tests completed.", m_TestCount);
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
                    throw new Exception("Failure to match: instruction {0} did not match expected bit length.");
                for (int i = 0; i < expected.Length; i++)
                {
                    if (((expected[i] & 0x00ff) != assembled[i * 2]) || ((expected[i] >> 8) != assembled[i * 2 + 1]))
                    {
                        // format the expected and actual code values:
                        StringBuilder sbCode = new StringBuilder(), sbAssembled = new StringBuilder();
                        for (int j = 0; j < expected.Length; j++)
                            sbCode.Append(string.Format("{0:X4} ", expected[j]));
                        for (int j = 0; j < assembled.Count; j++)
                        {
                            int k = ((j % 2) == 0) ? 1 : -1;
                            sbAssembled.Append(string.Format("{0:X2}", assembled[j + k]));
                            if (j % 2 == 1)
                                sbAssembled.Append(" ");
                        }
                        throw new Exception(string.Format("Failure to match expected bit pattern.\n" + 
                            "Expected: {1}\n" + "Actual:   {2}", 
                            asm, sbCode.ToString(), sbAssembled.ToString()));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("{0}\n{1}", asm, e.Message), e);
            }

            m_TestCount++;

        }
    }
}
