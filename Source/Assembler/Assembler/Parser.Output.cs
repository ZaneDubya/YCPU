/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

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
    }
}
