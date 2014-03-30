using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Devices.Graphics
{
    class MemoryBankLEM : IMemoryBank
    {
        public MemoryBankLEM()
        {
            m_SCRRAM = new ushort[0x0200]; // 512 words (384 in original spec; last 0x80 are ignored).
            m_CHRRAM = new ushort[0x0100]; // 256 words
            m_PALRAM = new ushort[0x0010]; // 16 words
        }

        public bool SCRRAM_Delta, CHRRAM_Delta, PALRAM_Delta = false;

        public void ResetDeltas()
        {
            SCRRAM_Delta = CHRRAM_Delta = PALRAM_Delta = false;
        }

        // memory map is laid out as such:
        // 0x0000 - 0x07FF - SCRRAM, repeats every 0x0200 words.
        // 0x0800 - 0x0BFF - CHRRAM, repeats every 0x0100 words.
        // 0x0C00 - 0x0FFF - PALRAM, repeats every 0x0010 words.

        private ushort[] m_SCRRAM, m_CHRRAM, m_PALRAM;

        public bool ReadOnly
        {
            get { return false; }
            set { }
        }

        public ushort this[int i]
        {
            get
            {
                if (i < 0x0800)
                    return m_SCRRAM[i % 0x0200];
                else if (i < 0x0C00)
                    return m_CHRRAM[i % 0x0100];
                else
                    return m_PALRAM[i % 0x0010];
            }
            set
            {
                if (i < 0x0800)
                {
                    SCRRAM_Delta = true;
                    m_SCRRAM[i % 0x0200] = value;
                }
                else if (i < 0x0C00)
                {
                    CHRRAM_Delta = true;
                    m_CHRRAM[i % 0x0100] = value;
                }
                else
                {
                    PALRAM_Delta = true;
                    m_PALRAM[i % 0x0010] = value;
                }
            }
        }
    }
}
