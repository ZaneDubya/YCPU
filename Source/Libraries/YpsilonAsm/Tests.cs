using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Assembler;

namespace Ypsilon
{
    public static class Tests
    {
        public static string Run()
        {
            try
            {
                Test("cmp r0, $1234", 0x0000, 0x1234);

            }
            catch (Exception e)
            {
                return e.Message;
            }

            return "Test successful!";
        }

        private static void Test(string asm, params ushort[] code)
        {
            Parser p = new Parser();
            try
            {
                List<byte> assembled = p.Parse(asm, string.Empty);
                if (assembled.Count != code.Length * 2)
                    throw new Exception("Failure to match: instruction {0} did not match expected bit length.");
                for (int i = 0; i < code.Length; i++)
                {
                    if (((code[i] & 0x00ff) != assembled[i * 2]) || ((code[i] >> 8) != assembled[i * 2 + 1]))
                        throw new Exception("Failure to match: instruction {0} did not match expected bit pattern.");
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Failure to compile {0} : {1}", asm, e.Message), e);
            }
        }
    }
}
