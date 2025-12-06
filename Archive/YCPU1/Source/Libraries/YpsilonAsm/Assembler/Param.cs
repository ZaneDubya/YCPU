/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

namespace Ypsilon.Assembler
{
    public class Param
    {
        public AddressingMode AddressingMode = AddressingMode.None;
        public bool UsesExtraDataSegment = false;
        public ushort RegisterIndex = 0;

        public bool HasImmediateWord { get; private set; }
        public bool HasImmediateWordLong { get; private set; }
        public bool HasLabel { get; private set; }

        public ushort ImmediateWordShort
        {
            get { return m_ImmediateWord; }
            set
            {
                m_ImmediateWord = value;
                HasImmediateWord = true;
            }
        }

        public uint ImmediateWordLong
        {
            get { return m_ImmediateWordLong; }
            set
            {
                m_ImmediateWordLong = value;
                HasImmediateWord = true;
                HasImmediateWordLong = true;
            }
        }

        public string Label
        {
            get { return m_Label; }
            set
            {
                m_Label = value;
                HasLabel = true;
            }
        }
        
        private ushort m_ImmediateWord;
        private uint m_ImmediateWordLong;
        private string m_Label;
    }
}
