using System;

namespace Ypsilon.Emulation.Processor {
    /// <summary>
    /// A segment is a window into memory.
    /// </summary>
    internal class Segment {
        // ======================================================================
        // Constants.
        // ======================================================================

        private const uint c_SegRegA = 0x10000000;
        private const uint c_SegRegD = 0x80000000;
        private const int c_SegRegDeviceBase = 0x0000FFFF;
        private const int c_SegRegDeviceIndex = 0x000F0000;
        private const int c_SegRegDeviceIndexShift = 16;
        private const int c_SegRegMemBase = 0x000FFFFF;
        private const uint c_SegRegP = 0x20000000;
        private const int c_SegRegSize = 0x0FF00000;
        private const int c_SegRegSizeShift = 12;
        private const uint c_SegRegW = 0x40000000;
        public MemoryReferenceInfo Reference = MemoryReferenceInfo.None;
        // ======================================================================
        // Public properties.
        // ======================================================================

        public readonly SegmentIndex SegmentType;
        private readonly YBUS m_Bus;

        // ======================================================================
        // Private vars.
        // ======================================================================

        private uint m_Register;
        private uint m_Size;

        public uint Base { get; private set; }

        public ushort DeviceIndex => (ushort)((m_Register & c_SegRegDeviceIndex) >> c_SegRegDeviceIndexShift);

        public bool IsAccessed {
            get { return (m_Register & c_SegRegA) != 0; }
            set {
                if (value)
                    m_Register |= c_SegRegA;
                else
                    m_Register &= ~c_SegRegA;
            }
        }

        public bool IsDevice {
            get { return (m_Register & c_SegRegD) != 0; }
            set {
                if (IsDevice != value) {
                    if (value)
                        m_Register |= c_SegRegD;
                    else
                        m_Register &= ~c_SegRegD;
                    RefreshMemoryReference();
                }
            }
        }

        public bool IsNotPresent {
            get { return (m_Register & c_SegRegP) != 0; }
            set {
                if (IsNotPresent != value) {
                    if (value)
                        m_Register |= c_SegRegP;
                    else
                        m_Register &= ~c_SegRegP;
                    RefreshMemoryReference();
                }
            }
        }

        public bool IsWriteProtected {
            get { return (m_Register & c_SegRegW) != 0; }
            set {
                if (IsWriteProtected != value) {
                    if (value)
                        m_Register |= c_SegRegW;
                    else
                        m_Register &= ~c_SegRegW;
                }
            }
        }

        // ======================================================================
        // Public methods.
        // ======================================================================

        public byte this[ushort i] {
            get {
                if (i >= m_Size) {
                    throw new SegFaultException(SegmentType, i);
                }
                return MemoryReference[i + Base];
            }
            set {
                if (i >= m_Size) {
                    throw new SegFaultException(SegmentType, i);
                }
                MemoryReference[i + Base] = value;
            }
        }

        public IMemoryInterface MemoryReference { get; private set; }

        public uint Register {
            get { return m_Register; }
            set {
                if (m_Register != value) {
                    m_Register = value;
                    RefreshMemoryReference();
                }
            }
        }

        // ======================================================================
        // Ctor and private methods.
        // ======================================================================

        public Segment(SegmentIndex segmentType, YBUS bus, uint register) {
            SegmentType = segmentType;
            m_Bus = bus;
            Register = register;
            RefreshMemoryReference();
        }

        public void SetMemoryReference(IMemoryInterface reference) {
            MemoryReference = reference;
        }

        public override string ToString() {
            return $"{Enum.GetName(typeof(SegmentIndex), SegmentType)} [{m_Register:X8}:{MemoryReference}]";
        }

        private void RefreshMemoryReference() {
            // set byte[] reference.
            if (IsNotPresent) {
                MemoryReference = null;
            }
            else {
                if (IsDevice) {
                    // will select rom if index == 0, device memory if index is between 1-15.
                    m_Bus.GetDeviceMemoryReference(this, DeviceIndex);
                }
                else {
                    m_Bus.GetRAMReference(this);
                }
            }

            // get size
            uint s = (m_Register & c_SegRegSize) >> c_SegRegSizeShift;
            m_Size = s == 0 ? ushort.MaxValue + 1 : s;

            // get base
            if (IsDevice) {
                uint b = m_Register & c_SegRegDeviceBase;
                Base = b << 8;
            }
            else {
                uint b = m_Register & c_SegRegMemBase;
                Base = b << 8;
            }
        }
    }

    public enum SegmentIndex {
        CS = 0,
        DS = 1,
        ES = 2,
        SS = 3,
        IS = 4
    }

    [Flags]
    public enum MemoryReferenceInfo {
        DeviceIndex = 0x00FF,
        ReferenceType = 0xFF00,
        None = 0x0000,
        RAM = 0x0100,
        ROM = 0x0200,
        Device = 0x0400
    }
}