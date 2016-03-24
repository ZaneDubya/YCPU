namespace Ypsilon.Emulation.Devices.Graphics
{
    public class MemoryLEM : IMemoryInterface
    {
        // memory map is laid out as follows:
        // 0x0000 - 0x07FF - SCRRAM, repeats every 0x0400 bytes.
        // 0x0800 - 0x0BFF - CHRRAM, repeats every 0x0200 bytes.
        // 0x0C00 - 0x0FFF - PALRAM, repeats every 0x0020 bytes.

        public MemoryLEM()
        {
            m_SCRRAM = new byte[0x0400]; // 512 words (384 in original spec; last 0x80 are ignored).
            m_CHRRAM = new byte[0x0200]; // 256 words
            m_PALRAM = new byte[0x0020]; // 16 words
        }

        public bool SCRRAM_Delta, CHRRAM_Delta, PALRAM_Delta;

        public void ResetDeltas()
        {
            SCRRAM_Delta = CHRRAM_Delta = PALRAM_Delta = false;
        }

        private byte[] m_SCRRAM, m_CHRRAM, m_PALRAM;

        public byte this[uint i]
        {
            get
            {
                i &= 0x0FFF;
                if (i < 0x0800)
                    return m_SCRRAM[i % 0x0400];
                if (i < 0x0C00)
                    return m_CHRRAM[i % 0x0200];
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
