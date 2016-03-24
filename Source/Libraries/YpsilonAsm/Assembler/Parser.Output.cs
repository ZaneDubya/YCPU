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
        public string ErrorMsg
        {
            get;
            private set;
        }

        public int ErrorLine
        {
            get;
            private set;
        }

        private void AddMessageLine(string input)
        {
            if (ErrorMsg == null)
                ErrorMsg = string.Empty;
            else
                ErrorMsg += "\n";
            ErrorMsg += input;
        }
    }
}
