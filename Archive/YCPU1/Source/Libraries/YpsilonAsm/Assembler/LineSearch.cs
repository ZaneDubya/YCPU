/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

namespace Ypsilon.Assembler
{
    internal class LineSearch
    {
        public static bool MatchLabel(string line)
        {
            string trimmed = line.Trim();
            if (trimmed.Length == 0)
                return false;
            if (!char.IsLetter(trimmed[0]))
                return false;
            for (int i = 1; i < trimmed.Length; i++)
                if (trimmed.IndexOf(':') == i)
                    return true;
                else if (!char.IsLetterOrDigit(trimmed[i]) && !(trimmed.IndexOf('_') == i))
                    return false;
            return false;
        }

        private static string m_Pragmas = "advance|alias|alignglobals|checkpc|dat8|dat16|incbin|include|macro|macend|org|require|reserve|scope|scend|data|text";

        public static bool MatchPragma(string line)
        {
            string trimmed = line.Trim();
            if (trimmed.Length == 0)
                return false;
            if (trimmed == "{" || trimmed == "}")
                return true;
            if (trimmed[0] != '.')
                return false;
            if (m_Pragmas.Contains(trimmed.Substring(1)))
                return true;
            return false;
        }
    }
}
