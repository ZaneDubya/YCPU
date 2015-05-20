using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Assembler
{
    public partial class Parser
    {
        public string MessageOutput
        {
            get;
            set;
        }

        void AddMessageLine(string input)
        {
            if (MessageOutput == null)
                MessageOutput = string.Empty;
            else
                MessageOutput += "\r\n";
            MessageOutput += input;
        }

        void AddMessage(string input)
        {
            if (MessageOutput == null)
                MessageOutput = string.Empty;
            MessageOutput += input;
        }
    }
}
