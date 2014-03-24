using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YCPU.Assembler
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
    }
}
