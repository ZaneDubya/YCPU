using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Assembler
{
    class Parser
    {
        private DCPU16ASM.Parser m_Parser;
        private DCPU16ASM.Generator m_Generator;

        public Parser()
        {
            m_Parser = new DCPU16ASM.Parser();
            m_Generator = new DCPU16ASM.Generator();
        }

        public ushort[] Parse(string[] lines)
        {
            return m_Parser.Parse(lines);
        }

        public string Generate(ushort[] machinecode, string out_path)
        {
            return m_Generator.Generate(machinecode, out_path);
        }
    }
}
