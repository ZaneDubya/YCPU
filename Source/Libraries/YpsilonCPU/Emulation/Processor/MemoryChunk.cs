using System;

namespace Ypsilon.Emulation.Processor
{
    class MemoryChunk : IMemoryInterface
    {
        public int Size
        {
            get { return m_Memory.Length; }
        }

        /// <summary>
        /// Set to true for ROM, false for RAM.
        /// </summary>
        public bool ReadOnly
        {
            get; set;
        }

        public MemoryChunk(uint size)
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
