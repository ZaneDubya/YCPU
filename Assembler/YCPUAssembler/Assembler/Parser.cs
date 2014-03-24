using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Assembler
{
    class Parser : DCPU16ASM.Parser
    {
        private Platform.YCPU m_YCPU;

        public Parser() : base()
        {
            m_YCPU = new Platform.YCPU();
        }

        protected override void InitOpcodeDictionary()
        {
            for (int i = 0; i < 0x100; i++)
            {
                
            }
        }

        protected override void InitRegisterDictionary()
        {

        }
    }
}
