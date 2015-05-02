/*
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ypsilon.Assembler
{
    class RegEx
    {
        private static Regex m_Regex_Label = new Regex("(^[A-Za-z][A-Za-z0-9]*(?:_[A-Za-z0-9]+)*:)|(^:[A-Za-z0-9]*(?:_[A-Za-z0-9]+)*)");
        private static Regex m_Regex_Label_local = new Regex("(^[_][A-Za-z0-9]*(?:_[A-Za-z0-9]+)*:)");

        public static bool MatchLabel(string line)
        {
            return (m_Regex_Label.IsMatch(line));
        }

        public static bool MatchLabelLocal(string line)
        {
            return (m_Regex_Label_local.IsMatch(line));
        }

        private static Regex m_Regex_Pragmas = new Regex(@"(^\.\b(target|alu_width|advance|alias|checkpc|dat8|dat16|incbin|include|macro|macend|org|require|reserve|scope|scend|data|text)\b)");

        public static bool MatchPragma(string line)
        {
            return (m_Regex_Pragmas.IsMatch(line));
        }
    }
}
