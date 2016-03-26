using System;

namespace Ypsilon.Emulation.Processor
{
    /// <summary>
    /// A segment is a window into memory.
    /// </summary>
    internal class Segment
    {
        // ======================================================================
        // Public properties.
        // ======================================================================

        public readonly SegmentIndex SegmentType;
        public MemoryReferenceInfo Reference = MemoryReferenceInfo.None;

        public uint Register
        {
            get { return m_Register; }
            set
            {
                if (m_Register != value)
                {
                    m_Register = value;
                    RefreshMemoryReference();
                }
            }
        }

        public bool IsDevice
        {
            get { return (m_Register & c_SegRegD) != 0; }
            set
            {
                if (IsDevice != value)
                {
                    if (value)
                        m_Register |= c_SegRegD;
                    else
                        m_Register &= ~c_SegRegD;
                    RefreshMemoryReference();
                }
            }
        }

        public ushort DeviceIndex => (ushort)((m_Register & c_SegRegDeviceIndex) >> c_SegRegDeviceIndexShift);

        public bool IsWriteProtected
        {
            get { return (m_Register & c_SegRegW) != 0; }
            set
            {
                if (IsWriteProtected != value)
                {
                    if (value)
                        m_Register |= c_SegRegW;
                    else
                        m_Register &= ~c_SegRegW;
                }
            }
        }

        public bool IsNotPresent
        {
            get { return (m_Register & c_SegRegP) != 0; }
            set
            {
                if (IsNotPresent != value)
                {
                    if (value)
                        m_Register |= c_SegRegP;
                    else
                        m_Register &= ~c_SegRegP;
                    RefreshMemoryReference();
                }
            }
        }

        public bool IsAccessed
        {
            get { return (m_Register & c_SegRegA) != 0; }
            set
            {
                if (value)
                    m_Register |= c_SegRegA;
                else
                    m_Register &= ~c_SegRegA;
            }
        }

        public uint Base => m_Base;

        public IMemoryInterface MemoryReference => m_MemoryReference;

        // ======================================================================
        // Public methods.
        // ======================================================================

        public byte this[ushort i]
        {
            get
            {
                if (i >= m_Size)
                {
                    throw new SegFaultException(SegmentType, i);
                }
                return m_MemoryReference[i + m_Base];
            }
            set
            {
                if (i >= m_Size)
                {
                    throw new SegFaultException(SegmentType, i);
                }
                m_MemoryReference[i + m_Base] = value;
            }
        }

        // ======================================================================
        // Private vars.
        // ======================================================================

        private uint m_Register;
        private readonly YBUS m_Bus;
        private IMemoryInterface m_MemoryReference;
        private uint m_Size, m_Base;

        // ======================================================================
        // Constants.
        // ======================================================================

        private const uint c_SegRegA = 0x10000000;
        private const uint c_SegRegP = 0x20000000;
        private const uint c_SegRegW = 0x40000000;
        private const uint c_SegRegD = 0x80000000;
        private const int c_SegRegSize = 0x0FF00000;
        private const int c_SegRegSizeShift = 12;
        private const int c_SegRegMemBase = 0x000FFFFF;
        private const int c_SegRegDeviceIndex = 0x000F0000;
        private const int c_SegRegDeviceIndexShift = 16;
        private const int c_SegRegDeviceBase = 0x0000FFFF;

        // ======================================================================
        // Ctor and private methods.
        // ======================================================================

        public Segment(SegmentIndex segmentType, YBUS bus, uint register)
        {
            SegmentType = segmentType;
            m_Bus = bus;
            Register = register;
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
            uint s = (m_Register & c_SegRegSize) >> c_SegRegSizeShift;
            m_Size = (s == 0) ? ushort.MaxValue + 1 : s;

            // get base
            if (IsDevice)
            {
                uint b = (m_Register & c_SegRegDeviceBase);
                m_Base = b << 8;
            }
            else
            {
                uint b = (m_Register & c_SegRegMemBase);
                m_Base = b << 8;
            }
        }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof (SegmentIndex), SegmentType)} [{m_Register:X8}:{m_MemoryReference}]";
        }
    }

    public enum SegmentIndex
    {
        CS = 0,
        DS = 1,
        ES = 2,
        SS = 3,
        IS = 4
    }

    [Flags]
    public enum MemoryReferenceInfo
    {
        DeviceIndex = 0x00FF,
        ReferenceType = 0xFF00,
        None = 0x0000,
        RAM = 0x0100,
        ROM = 0x0200,
        Device = 0x0400
    }
}
