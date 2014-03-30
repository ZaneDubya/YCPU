using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Hardware
{
    class MemoryBank : IMemoryBank
    {
        public MemoryBank()
        {
            m_Data = new ushort[0x1000];
        }

        private bool m_ReadOnly = false;
        private ushort[] m_Data;

        public bool ReadOnly
        {
            get { return m_ReadOnly; }
            set { m_ReadOnly = value; }
        }

        public ushort this[int i]
        {
            get { return m_Data[i & 0x0FFF]; }
            set { m_Data[i & 0x0FFF] = value; }
        }
    }
}
