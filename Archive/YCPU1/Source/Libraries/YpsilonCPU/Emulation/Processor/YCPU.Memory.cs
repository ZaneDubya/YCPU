using System;

namespace Ypsilon.Emulation.Processor {
    partial class YCPU {
        // Segment Registers used when MMU is inactive.
        private Segment m_CS_NoMMU, m_DS_NoMMU, m_ES_NoMMU, m_SS_NoMMU, m_IS_NoMMU;

        // Segment Registers used when MMU is active.
        private Segment m_CSS, m_CSU, m_DSS, m_DSU, m_ESS, m_ESU, m_SSS, m_SSU, m_IS;

        public uint CS => PS_M ? (PS_S ? m_CSS.Register : m_CSU.Register) : m_CS_NoMMU.Register;

        public uint DS => PS_M ? (PS_S ? m_DSS.Register : m_DSU.Register) : m_DS_NoMMU.Register;

        public uint ES => PS_M ? (PS_S ? m_ESS.Register : m_ESU.Register) : m_ES_NoMMU.Register;

        public uint IS => PS_M ? m_IS.Register : m_IS_NoMMU.Register;

        public uint SS => PS_M ? (PS_S ? m_SSS.Register : m_SSU.Register) : m_SS_NoMMU.Register;

        public ushort DebugReadMemory(ushort address, SegmentIndex segmentType) {
            Segment seg = GetSegment(segmentType);
            if (seg == null || seg.MemoryReference == null)
                throw new Exception("Unknown segment referenced in DebugReadMemory");
            byte lo = seg.MemoryReference[seg.Base + address];
            byte hi = seg.MemoryReference[seg.Base + address + 1];
            return (ushort)((hi << 8) + lo);
        }

        /// <summary>
        /// Initializes arrays for memory banks, internal ram, and internal rom.
        /// Must be called before running any operations.
        /// </summary>
        public void InitializeMemory() {
            m_CS_NoMMU = new Segment(SegmentIndex.CS, BUS, 0x80000000);
            m_DS_NoMMU = new Segment(SegmentIndex.DS, BUS, 0x00000000);
            m_ES_NoMMU = new Segment(SegmentIndex.ES, BUS, 0x00000000);
            m_SS_NoMMU = new Segment(SegmentIndex.SS, BUS, 0x00000000);
            m_IS_NoMMU = new Segment(SegmentIndex.IS, BUS, 0x80000000);
            m_CSS = new Segment(SegmentIndex.CS, BUS, 0x80000000);
            m_CSU = new Segment(SegmentIndex.CS, BUS, 0x80000000);
            m_DSS = new Segment(SegmentIndex.DS, BUS, 0x00000000);
            m_DSU = new Segment(SegmentIndex.DS, BUS, 0x00000000);
            m_ESS = new Segment(SegmentIndex.ES, BUS, 0x00000000);
            m_ESU = new Segment(SegmentIndex.ES, BUS, 0x00000000);
            m_SSS = new Segment(SegmentIndex.SS, BUS, 0x00000000);
            m_SSU = new Segment(SegmentIndex.SS, BUS, 0x00000000);
            m_IS = new Segment(SegmentIndex.IS, BUS, 0x80000000);
        }

        /// <summary>
        /// Reads a 16-bit value from the address specified.
        /// </summary>
        public ushort ReadMemInt16(ushort address, SegmentIndex segment) {
            Segment s = GetSegment(segment);
            if (s == null)
                throw new Exception("Unknown segment referenced in DebugReadMemory");
            if ((address & 0x0001) == 0x0001) {
                // This read is not 16-bit aligned. Two memory accesses needed.
                Cycles += 2;
                byte byte0 = s[address];
                byte byte1 = s[(ushort)(address + 1)];
                return (ushort)((byte1 << 8) + byte0);
            }
            else {
                // This read is 16-bit aligned. Only one memory access is needed.
                Cycles += 1;
                byte byte0 = s[address];
                byte byte1 = s[(ushort)(address + 1)];
                return (ushort)((byte1 << 8) + byte0);
            }
        }

        /// <summary>
        /// Reads an 8-bit value from the address specified.
        /// </summary>
        public byte ReadMemInt8(ushort address, SegmentIndex segment) {
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            Cycles += 1;
            Segment s = GetSegment(segment);
            if (s == null)
                throw new Exception("Unknown segment referenced in DebugReadMemory");
            return s[address];
        }

        public void WriteMemInt16(ushort address, ushort value, SegmentIndex segment) {
            Segment s = GetSegment(segment);
            if (s == null)
                throw new Exception("Unknown segment referenced in DebugReadMemory");
            if ((address & 0x0001) == 0x0001) {
                // This read is not 16-bit aligned. Two memory accesses needed.
                Cycles += 2;
                s[address] = (byte)(value & 0x00ff);
                s[(ushort)(address + 1)] = (byte)(value >> 8);
            }
            else {
                // This read is 16-bit aligned. Only one memory access is needed.
                Cycles += 1;
                s[address] = (byte)(value & 0x00ff);
                s[(ushort)(address + 1)] = (byte)(value >> 8);
            }
        }

        public void WriteMemInt8(ushort address, byte value, SegmentIndex segment) {
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            Cycles += 1;
            Segment s = GetSegment(segment);
            if (s == null)
                throw new Exception("Unknown segment referenced in DebugReadMemory");
            s[address] = value;
        }

        private Segment GetSegment(SegmentIndex segment) {
            Segment s = null;
            if (PS_I && segment == SegmentIndex.CS)
                segment = SegmentIndex.IS;
            switch (segment) {
                case SegmentIndex.CS:
                    s = PS_M ? (PS_S ? m_CSS : m_CSU) : m_CS_NoMMU;
                    break;
                case SegmentIndex.DS:
                    s = PS_M ? (PS_S ? m_DSS : m_DSU) : m_DS_NoMMU;
                    break;
                case SegmentIndex.ES:
                    s = PS_M ? (PS_S ? m_ESS : m_ESU) : m_ES_NoMMU;
                    break;
                case SegmentIndex.SS:
                    s = PS_M ? (PS_S ? m_SSS : m_SSU) : m_SS_NoMMU;
                    break;
                case SegmentIndex.IS:
                    s = PS_M ? m_IS : m_IS_NoMMU;
                    break;
            }
            return s;
        }
    }
}