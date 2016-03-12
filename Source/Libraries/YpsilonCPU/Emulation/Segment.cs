
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

        public readonly SegmentIndex SegmentType;

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

        public ushort DeviceIndex
        {
            get { return (ushort)((m_SegmentRegister & c_SEGREG_DeviceIndex) >> c_SEGREG_DeviceIndexShift); }
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

        public byte this[uint i]
        {
            get
            {
                i &= 0x0000ffff;
                if (i >= m_Size)
                {
                    m_Bus.CPU.Interrupt_SegFault(SegmentType);
                    return 0;
                }
                else
                {
                    return m_MemoryReference[i + m_Base];
                }
            }
            set
            {
                m_MemoryReference[i] = value;
            }
        }

        // ======================================================================
        // Private vars.
        // ======================================================================

        private uint m_SegmentRegister;
        private readonly YBUS m_Bus;
        private IMemoryInterface m_MemoryReference;
        private uint m_Size, m_Base;

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

        public Segment(SegmentIndex segmentType, YBUS bus, uint register)
        {
            SegmentType = segmentType;
            m_Bus = bus;
            SegmentRegister = register;
            RefreshMemoryReference();
        }

        public void SetMemoryReference(IMemoryInterface reference)
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
                    // will select rom if index == 0, device memory if index is between 1-15.
                    m_Bus.GetDeviceMemoryReference(this, DeviceIndex);
                }
                else
                {
                    m_Bus.GetRAMReference(this);
                }
            }

            // get size
            uint s = (m_SegmentRegister & c_SEGREG_Size) >> c_SEGREG_SizeShift;
            m_Size = (s == 0) ? ushort.MaxValue : s;

            // get base
            if (IsDevice)
            {
                uint b = (m_SegmentRegister & c_SEGREG_MemBase);
                m_Base = b;
            }
            else
            {
                uint b = (m_SegmentRegister & c_SEGREG_DeviceBase);
                m_Base = b;
            }
        }
    }
}
