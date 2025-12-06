using System.Collections.Generic;
using System.Linq;

namespace Ypsilon.Emulation.Processor {
    /// <summary>
    /// A representation of a address/data bus that has 16 generic slots. Device in slot zero is YCPU. Other slots can hold devices that inherit from ADevice.
    /// </summary>
    public class YBUS {
        public readonly YCPU CPU;
        private readonly ADevice[] m_Devices = new ADevice[15];
        private readonly List<ushort> m_DevicesRaisingIRQ = new List<ushort>();
        private IDisplayProvider m_DisplayProvider;
        private IInputProvider m_InputProvider;
        private MemoryChunk m_RAM, m_ROM;
        private readonly List<Segment> m_References = new List<Segment>();

        // ===================================================================================================
        // YCPU.HWQ Functions
        // ===================================================================================================

        public ushort DevicesConnected {
            get {
                ushort count = 0;
                for (int i = 0; i < m_Devices.Length; i++)
                    if (m_Devices[i] != null)
                        count++;
                return count;
            }
        }

        public ushort FirstIRQ {
            get {
                if (m_DevicesRaisingIRQ.Count == 0)
                    return 0xFFFF;
                return (ushort)(m_DevicesRaisingIRQ.Min() + 1);
            }
        }

        // ===================================================================================================
        // YCPU.IRQ Functions
        // ===================================================================================================

        public bool IsIRQ => m_DevicesRaisingIRQ.Count > 0;

        // ===================================================================================================
        // Memory Reference Functions
        // ===================================================================================================

        public int RAMSize => m_RAM.Size;

        public int ROMSize => m_ROM.Size;

        // ===================================================================================================
        // Emulator Functions
        // ===================================================================================================

        public YBUS(YCPU cpu) {
            CPU = cpu;
        }

        public void AcknowledgeIRQ(ushort deviceIndex) {
            deviceIndex -= 1;
            if (m_DevicesRaisingIRQ.Contains(deviceIndex)) {
                m_Devices[deviceIndex].IRQAcknowledged();
                m_DevicesRaisingIRQ.Remove(deviceIndex);
            }
        }

        /// <summary>
        /// Adds a device to specified slot index. INDEX MUST BE 1 - 16.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="index"></param>
        public void AddDevice(ADevice device, ushort index) {
            if (index == 0 || index > 16)
                return;
            if (m_Devices[index - 1] != null) {
                m_Devices[index - 1].Dispose();
                m_Devices[index - 1] = null;
            }
            m_Devices[index - 1] = device;
            foreach (Segment segment in m_References) {
                if (((segment.Reference & MemoryReferenceInfo.ReferenceType) == MemoryReferenceInfo.Device) &&
                    ((segment.Reference & MemoryReferenceInfo.DeviceIndex) == (MemoryReferenceInfo)index)) {
                    GetDeviceMemoryReference(segment, index);
                }
            }
        }

        internal void AddSegmentToReferences(Segment segment) {
            int index = m_References.IndexOf(segment);
            if (index == -1) {
                m_References.Add(segment);
            }
        }

        // ===================================================================================================
        // Device Functions
        // ===================================================================================================

        internal void Device_RaiseIRQ(ADevice device) {
            int device_index = -1;
            for (int i = 0; i < m_Devices.Length; i++)
                if (m_Devices[i] == device)
                    device_index = i + 1;
            if (device_index == -1) {
                // device raising irq does not exist on bus - should never occur
            }
            else {
                if (!m_DevicesRaisingIRQ.Contains((ushort)device_index))
                    m_DevicesRaisingIRQ.Add((ushort)device_index);
            }
        }

        public void Display(List<ITexture> textures) {
            for (int i = 0; i < m_Devices.Length; i++) {
                if (m_Devices[i] != null) {
                    m_Devices[i].Display(i, textures, m_DisplayProvider);
                }
            }
        }

        public void FillROM(byte[] rom) {
            if (m_ROM == null) {
                SetROM((uint)rom.Length);
            }
            m_ROM.ReadOnly = false;
            for (uint i = 0; i < rom.Length; i++)
                m_ROM[i] = rom[i];
            for (uint i = (uint)rom.Length; i < m_ROM.Size; i++)
                m_ROM[i] = 0x00;
            m_ROM.ReadOnly = true;
        }

        /// <summary>
        /// Returns  the byte array of memory attached to the device. Accepts values from 0 - 16. Device 0 is ROM.
        /// </summary>
        internal void GetDeviceMemoryReference(Segment segment, ushort deviceIndex) {
            if (deviceIndex == 0) {
                // request memory from ycpu
                GetROMReference(segment);
            }
            else if (deviceIndex > 16) {
                // this should never happen - requested memory from device index that cannot exist.
                segment.SetMemoryReference(null);
                segment.Reference = MemoryReferenceInfo.None;
                AddSegmentToReferences(segment);
            }
            else {
                // request memory from device in slot index.
                if (m_Devices[deviceIndex - 1] == null) {
                    segment.SetMemoryReference(null);
                    segment.Reference = MemoryReferenceInfo.None;
                }
                else {
                    segment.SetMemoryReference(m_Devices[deviceIndex - 1].GetMemoryInterface());
                    segment.Reference = MemoryReferenceInfo.Device | (MemoryReferenceInfo)deviceIndex;
                }
                AddSegmentToReferences(segment);
            }
        }

        internal void GetRAMReference(Segment segment) {
            segment.SetMemoryReference(m_RAM);
            segment.Reference = MemoryReferenceInfo.RAM;
            AddSegmentToReferences(segment);
        }

        internal void GetROMReference(Segment segment) {
            segment.SetMemoryReference(m_ROM);
            segment.Reference = MemoryReferenceInfo.ROM;
            AddSegmentToReferences(segment);
        }

        public ushort[] QueryDevice(ushort deviceIndex) {
            if (deviceIndex == 0) {
                // query YCPU
                ushort[] info = new ushort[0x04];
                info[0] = 0x0000;
                info[1] = 0x0000;
                info[2] = 0x0000;
                info[3] = 0x0000;
                return info;
            }
            if ((deviceIndex > 16) || m_Devices[deviceIndex] == null) {
                // query device index beyond number of slots, or empty device
                ushort[] info = new ushort[0x04];
                info[0] = 0x0000;
                info[1] = 0x0000;
                info[2] = 0x0000;
                info[3] = 0x0000;
                return info;
            }
            // query present device
            return m_Devices[deviceIndex - 1].Bus_DeviceQuery();
        }

        /// <summary>
        /// Adds a device in the specified slot index. INDEX MUST BE 1 - 16.
        /// </summary>
        public void RemoveDevice(ushort index) {
            if (index == 0 || index > 16)
                return;
            if (m_Devices[index - 1] != null) {
                m_Devices[index - 1].Dispose();
                m_Devices[index - 1] = null;
            }
            foreach (Segment segment in m_References) {
                if (((segment.Reference & MemoryReferenceInfo.ReferenceType) == MemoryReferenceInfo.Device) &&
                    ((segment.Reference & MemoryReferenceInfo.DeviceIndex) == (MemoryReferenceInfo)index)) {
                    GetDeviceMemoryReference(segment, index);
                }
            }
        }

        /// <summary>
        /// Removes all RAM, ROM, Devices, and any segment references to the same.
        /// </summary>
        public void Reset() {
            foreach (Segment segment in m_References) {
                segment.SetMemoryReference(null);
            }
            for (int i = 0; i < m_Devices.Length; i++) {
                if (m_Devices[i] != null) {
                    m_Devices[i].Dispose();
                    m_Devices[i] = null;
                }
            }
            m_RAM = null;
            m_ROM = null;
        }

        public ushort SendDeviceMessage(ushort deviceIndex, ushort param_0, ushort param_1) {
            if (deviceIndex == 0) {
                // send message to YCPU
                return ADevice.MSG_NO_DEVICE;
            }
            if ((deviceIndex > 16) || m_Devices[deviceIndex - 1] == null) {
                // send message to device index beyond number of slots, or empty device
                return ADevice.MSG_NO_DEVICE;
            }
            // send message to present device
            return m_Devices[deviceIndex - 1].Bus_SendMessage(param_0, param_1);
        }

        public void SetProviders(IDisplayProvider display, IInputProvider input) {
            m_DisplayProvider = display;
            m_InputProvider = input;
        }

        public void SetRAM(uint ramSize) {
            m_RAM = new MemoryChunk(ramSize);
            foreach (Segment segment in m_References) {
                if ((segment.Reference & MemoryReferenceInfo.ReferenceType) == MemoryReferenceInfo.RAM) {
                    GetRAMReference(segment);
                }
            }
        }

        public void SetROM(uint romSize) {
            m_ROM = new MemoryChunk(romSize);
            m_ROM.ReadOnly = true;
            foreach (Segment segment in m_References) {
                if ((segment.Reference & MemoryReferenceInfo.ReferenceType) == MemoryReferenceInfo.ROM) {
                    GetROMReference(segment);
                }
            }
        }

        public void Update() {
            for (int i = 0; i < m_Devices.Length; i++) {
                if (m_Devices[i] != null) {
                    m_Devices[i].Update(m_InputProvider);
                }
            }
        }
    }
}