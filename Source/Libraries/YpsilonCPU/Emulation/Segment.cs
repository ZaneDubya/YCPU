
using Ypsilon.Emulation.Hardware;

namespace Ypsilon.Emulation
{
    /// <summary>
    /// A segment is a window into memory.
    /// </summary>
    class Segment
    {
        // ======================================================================
        // Public properties.
        // ======================================================================

        public uint SegmentRegister
        {
            get { return m_SegmentRegister; }
            set
            {
                if (m_SegmentRegister != value)
                {
                    m_SegmentRegister = value;
                    RefreshMemoryReference();
                }
            }
        }

        public bool IsDevice
        {
            get { return (m_SegmentRegister & c_SEGREG_D) != 0; }
            set
            {
                if (IsDevice != value)
                {
                    if (value)
                        m_SegmentRegister |= c_SEGREG_D;
                    else
                        m_SegmentRegister &= ~c_SEGREG_D;
                    RefreshMemoryReference();
                }
            }
        }

        public bool IsWriteProtected
        {
            get { return (m_SegmentRegister & c_SEGREG_W) != 0; }
            set
            {
                if (IsWriteProtected != value)
                {
                    if (value)
                        m_SegmentRegister |= c_SEGREG_W;
                    else
                        m_SegmentRegister &= ~c_SEGREG_W;
                }
            }
        }

        public bool IsNotPresent
        {
            get { return (m_SegmentRegister & c_SEGREG_P) != 0; }
            set
            {
                if (IsNotPresent != value)
                {
                    if (value)
                        m_SegmentRegister |= c_SEGREG_P;
                    else
                        m_SegmentRegister &= ~c_SEGREG_P;
                    RefreshMemoryReference();
                }
            }
        }

        public bool IsAccessed
        {
            get { return (m_SegmentRegister & c_SEGREG_A) != 0; }
            set
            {
                if (value)
                    m_SegmentRegister |= c_SEGREG_A;
                else
                    m_SegmentRegister &= ~c_SEGREG_A;
            }
        }

        // ======================================================================
        // Public methods.
        // ======================================================================

        public byte this[int i]
        {
            get
            {
                return m_MemoryReference[i & 0x3FFF];
            }
            set
            {
                m_MemoryReference[i & 0x3FFF] = value;
            }
        }

        // ======================================================================
        // Private vars.
        // ======================================================================

        private uint m_SegmentRegister;
        private YBUS m_Bus;
        private byte[] m_MemoryReference;
        private int m_Size, m_Base;

        // ======================================================================
        // Constants.
        // ======================================================================

        private const uint c_SEGREG_A = 0x10000000, c_SEGREG_P = 0x20000000, c_SEGREG_W = 0x40000000, c_SEGREG_D = 0x80000000;
        private const int c_SEGREG_Size = 0x0FF00000, c_SEGREG_SizeShift = 20;
        private const int c_SEGREG_MemBase = 0x000FFFFF;
        private const int c_SEGREG_DeviceIndex = 0x000F0000, c_SEGREG_DeviceIndexShift = 16;
        private const int c_SEGREG_DeviceBase = 0x0000FFFF;

        // ======================================================================
        // Ctor and private methods.
        // ======================================================================

        public Segment(YBUS bus, uint register)
        {
            m_Bus = bus;
            SegmentRegister = register;
            RefreshMemoryReference();
        }

        public void SetMemoryReference(byte[] reference)
        {
            m_MemoryReference = reference;
        }

        private void RefreshMemoryReference()
        {
            // set byte[] reference.
            if (IsNotPresent)
            {
                m_MemoryReference = null;
            }
            else
            {
                if (IsDevice)
                {
                    
                }
                else
                {
                    m_MemoryReference = m_Bus.GetMemoryReference();
                }
            }

            // get size
            uint s = (m_SegmentRegister & c_SEGREG_Size) >> c_SEGREG_SizeShift;
            m_Size = (s == 0) ? ushort.MaxValue : (int)s;

            // get base
            if (IsDevice)
            {
                uint b = (m_SegmentRegister & c_SEGREG_MemBase);
                m_Base = (int)b;
            }
            else
            {
                uint b = (m_SegmentRegister & c_SEGREG_DeviceBase);
                m_Base = (int)b;
            }
        }
    }
}
