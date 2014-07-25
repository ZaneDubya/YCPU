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
            m_SCRRAM = new byte[0x0400]; // 512 words (384 in original spec; last 0x80 are ignored).
            m_CHRRAM = new byte[0x0200]; // 256 words
            m_PALRAM = new byte[0x0020]; // 16 words
        }

        public bool SCRRAM_Delta, CHRRAM_Delta, PALRAM_Delta = false;

        public void ResetDeltas()
        {
            SCRRAM_Delta = CHRRAM_Delta = PALRAM_Delta = false;
        }

        // memory map is laid out as such:
        // 0x0000 - 0x07FF - SCRRAM, repeats every 0x0400 bytes.
        // 0x0800 - 0x0BFF - CHRRAM, repeats every 0x0200 bytes.
        // 0x0C00 - 0x0FFF - PALRAM, repeats every 0x0020 bytes.

        private byte[] m_SCRRAM, m_CHRRAM, m_PALRAM;

        public bool ReadOnly
        {
            get { return false; }
            set { }
        }

        public byte this[int i]
        {
            get
            {
                i &= 0x0FFF;
                if (i < 0x0800)
                    return m_SCRRAM[i % 0x0400];
                else if (i < 0x0C00)
                    return m_CHRRAM[i % 0x0200];
                else
                    return m_PALRAM[i % 0x0020];
            }
            set
            {
                i &= 0x0FFF;
                if (i < 0x0800)
                {
                    SCRRAM_Delta = true;
                    m_SCRRAM[i % 0x0400] = value;
                }
                else if (i < 0x0C00)
                {
                    CHRRAM_Delta = true;
                    m_CHRRAM[i % 0x0200] = value;
                }
                else
                {
                    PALRAM_Delta = true;
                    m_PALRAM[i % 0x0020] = value;
                }
            }
        }
    }
}
