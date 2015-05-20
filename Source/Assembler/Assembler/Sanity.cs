using System;

namespace Ypsilon.Assembler
{
    static class Sanity
    {
        public static void RequireParamCountExact(string[] param, int count)
        {
            if (param.Length != count)
                throw new Exception(string.Format("Bad param count, expected {0}.", count));
        }

        public static void RequireParamCountMinMax(string[] param, int min, int max)
        {
            if ((param.Length < min) || (param.Length > max))
                throw new Exception(string.Format("Bad param count, expected {0}-{1}.", min, max));
        }

        public static void RequireOpcodeFlag(OpcodeFlag flag, OpcodeFlag[] acceptable)
        {
            for (int i = 0; i < acceptable.Length; i++)
                if (acceptable[i] == flag)
                    return;
            throw new Exception(string.Format("Opcode flag of '{0}' is unsupported for this opcode.", flag.ToString()));
        }
    }
}
