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

                // test alu (except for sto)
                string[] alu_instructions = new string[] {
                    "cmp", "neg", "add", "sub", "adc", "sbc", "mul", "div",
                    "mli", "dvi", "mod", "mdi", "and", "orr", "eor", "not",
                    "lod" };
                ushort[] alu_codes = new ushort[] {
                    0x0000, 0x0008, 0x0010, 0x0018, 0x0020, 0x0028, 0x0030, 0x0038,
                    0x0040, 0x0048, 0x0050, 0x0058, 0x0060, 0x0068, 0x0070, 0x0078,
                    0x0080 };
                string[] reg_control = new string[] { "fl", "pc", "ps", "p2", "ii", "ia", "usp", "sp" };

                for (int ins = 0; ins < alu_instructions.Length; ins++)
                {
                    for (int r0 = 0; r0 < 8; r0++)
                    {
                        Test(string.Format("{0}     r{1}, $1234", alu_instructions[ins], r0), (ushort)(0x0000 | alu_codes[ins] | r0), 0x1234);
                        Test(string.Format("{0}.8   r{1}, $0034", alu_instructions[ins], r0), (ushort)(0x0100 | alu_codes[ins] | r0), 0x0034);
                        Test(string.Format("{0}     r{1}, [$1234]", alu_instructions[ins], r0), (ushort)(0x0200 | alu_codes[ins] | r0), 0x1234);
                        Test(string.Format("{0}     r{1}, ES[$1234]", alu_instructions[ins], r0), (ushort)(0x8200 | alu_codes[ins] | r0), 0x1234);
                        Test(string.Format("{0}.8   r{1}, [$1234]", alu_instructions[ins], r0), (ushort)(0x0300 | alu_codes[ins] | r0), 0x1234);
                        Test(string.Format("{0}.8   r{1}, ES[$1234]", alu_instructions[ins], r0), (ushort)(0x8300 | alu_codes[ins] | r0), 0x1234);
                        for (int cr = 0; cr < 8; cr++)
                            Test(string.Format("{0}     r{1}, {2}", alu_instructions[ins], r0, reg_control[cr]), (ushort)(0x0800 | alu_codes[ins] | r0 | (cr << 8)));
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
                if (assembled.Count != expected.Length * 2)
                    throw new Exception("Failure to match: instruction {0} did not match expected bit length.");
                for (int i = 0; i < expected.Length; i++)
                {
                    if (((expected[i] & 0x00ff) != assembled[i * 2]) || ((expected[i] >> 8) != assembled[i * 2 + 1]))
                    {
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
                        throw new Exception(string.Format("Failure to match.\n{0} did not match expected bit pattern.\n" + 
                            "Expected: {1}\n" +
                            "Actual:   {2}", asm,
                            sbCode.ToString(), sbAssembled.ToString()));
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
