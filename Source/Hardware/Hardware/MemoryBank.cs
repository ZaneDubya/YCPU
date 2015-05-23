
namespace Ypsilon.Hardware
{
    /// <summary>
    /// A bank of 0x1000 = 4096 bytes.
    /// </summary>
    class MemoryBank : IMemoryBank
    {
        public MemoryBank()
        {
            m_Data = new byte[0x4000];
        }

        private bool m_ReadOnly = false;
        private byte[] m_Data;

        public bool ReadOnly
        {
            get { return m_ReadOnly; }
            set { m_ReadOnly = value; }
        }

        public byte this[int i]
        {
            get { return m_Data[i & 0x3FFF]; }
            set { m_Data[i & 0x3FFF] = value; }
        }
    }
}
