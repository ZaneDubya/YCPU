using System;

namespace Ypsilon.Emulation.Hardware
{
    class YMemory : IMemoryInterface
    {
        public YMemory(uint size)
        {
            m_Memory = new byte[size];
        }

        private byte[] m_Memory;

        public byte this[uint address]
        {
            get
            {
                if (address >= m_Memory.Length)
                    return 0;
                return m_Memory[address];
            }

            set
            {
                if (address < m_Memory.Length)
                    m_Memory[address] = value;
            }
        }
    }
}
