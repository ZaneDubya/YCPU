namespace Ypsilon.Emulation.Processor {
    internal class MemoryChunk : IMemoryInterface {
        private readonly byte[] m_Memory;

        /// <summary>
        /// Set to true for ROM, false for RAM.
        /// </summary>
        public bool ReadOnly { get; set; }

        public int Size => m_Memory.Length;

        public MemoryChunk(uint size) {
            m_Memory = new byte[size];
        }

        public byte this[uint address] {
            get {
                if (address >= m_Memory.Length)
                    return 0;
                return m_Memory[address];
            }

            set {
                if (ReadOnly)
                    return;
                if (address < m_Memory.Length)
                    m_Memory[address] = value;
            }
        }
    }
}