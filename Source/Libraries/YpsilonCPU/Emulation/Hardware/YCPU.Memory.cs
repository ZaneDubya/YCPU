
namespace Ypsilon.Emulation.Hardware
{
    partial class YCPU
    {
        // Segment Registers used when MMU is inactive.
        private Segment m_CS_NoMMU, m_DS_NoMMU, m_ES_NoMMU, m_SS_NoMMU, m_IS_NoMMU;

        // Segment Registers used when MMU is active.
        private Segment m_CSS, m_CSU, m_DSS, m_DSU, m_ESS, m_ESU, m_SSS, m_SSU, m_IS;

        public uint CS
        {
            get
            {
                return (PS_M) ? ((PS_S) ? m_CSS.SegmentRegister : m_CSU.SegmentRegister) : m_CS_NoMMU.SegmentRegister;
            }
        }

        public uint DS
        {
            get
            {
                return (PS_M) ? ((PS_S) ? m_DSS.SegmentRegister : m_DSU.SegmentRegister) : m_DS_NoMMU.SegmentRegister;
            }
        }

        public uint ES
        {
            get
            {
                return (PS_M) ? ((PS_S) ? m_ESS.SegmentRegister : m_ESU.SegmentRegister) : m_ES_NoMMU.SegmentRegister;
            }
        }

        public uint SS
        {
            get
            {
                return (PS_M) ? ((PS_S) ? m_SSS.SegmentRegister : m_SSU.SegmentRegister) : m_SS_NoMMU.SegmentRegister;
            }
        }

        public uint IS
        {
            get
            {
                return (PS_M) ? m_IS.SegmentRegister : m_IS_NoMMU.SegmentRegister;
            }
        }

        /// <summary>
        /// Initializes arrays for memory banks, internal ram, and internal rom.
        /// Must be called before running any operations.
        /// </summary>
        public void InitializeMemory()
        {
            m_CS_NoMMU = new Segment(SegmentIndex.CS, m_Bus, 0x80000000);
            m_DS_NoMMU = new Segment(SegmentIndex.DS, m_Bus, 0x00000000);
            m_ES_NoMMU = new Segment(SegmentIndex.ES, m_Bus, 0x00000000);
            m_SS_NoMMU = new Segment(SegmentIndex.SS, m_Bus, 0x00000000);
            m_IS_NoMMU = new Segment(SegmentIndex.IS, m_Bus, 0x80000000);

            m_CSS = new Segment(SegmentIndex.CS, m_Bus, 0x80000000);
            m_CSU = new Segment(SegmentIndex.CS, m_Bus, 0x80000000);
            m_DSS = new Segment(SegmentIndex.DS, m_Bus, 0x00000000);
            m_DSU = new Segment(SegmentIndex.DS, m_Bus, 0x00000000);
            m_ESS = new Segment(SegmentIndex.ES, m_Bus, 0x00000000);
            m_ESU = new Segment(SegmentIndex.ES, m_Bus, 0x00000000);
            m_SSS = new Segment(SegmentIndex.SS, m_Bus, 0x00000000);
            m_SSU = new Segment(SegmentIndex.SS, m_Bus, 0x00000000);
            m_IS = new Segment(SegmentIndex.IS, m_Bus, 0x80000000);
        }

        private Segment GetSegment(SegmentIndex segment)
        {
            Segment s = null;

            switch (segment)
            {
                case SegmentIndex.CS:
                    s = (PS_M) ? (PS_S ? m_CSS : m_CSU) : m_CS_NoMMU;
                    break;
                case SegmentIndex.DS:
                    s = (PS_M) ? (PS_S ? m_DSS : m_DSU) : m_DS_NoMMU;
                    break;
                case SegmentIndex.ES:
                    s = (PS_M) ? (PS_S ? m_ESS : m_ESU) : m_ES_NoMMU;
                    break;
                case SegmentIndex.SS:
                    s = (PS_M) ? (PS_S ? m_SSS : m_SSU) : m_SS_NoMMU;
                    break;
                case SegmentIndex.IS:
                    s = (PS_M) ? m_IS : m_IS_NoMMU;
                    break;
            }

            return s;
        }

        /// <summary>
        /// Reads an 8-bit value from the address specified.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="execute"></param>
        /// <returns>Return the value of the Int8 at this address.</returns>
        public byte ReadMemInt8(ushort address, SegmentIndex segment)
        {
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            m_Cycles += 1;

            Segment s = GetSegment(segment);
            if (s != null)
                return s[address];
            else
                return 0x00;
        }

        /// <summary>
        /// Reads a 16-bit value from the address specified.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="execute"></param>
        /// <returns>Return the value of the Int16 at this address.</returns>
        public ushort ReadMemInt16(ushort address, SegmentIndex segment)
        {
            Segment s = GetSegment(segment);
            if (s == null)
                return 0x0000;

            if ((address & 0x0001) == 0x0001)
            {
                // This read is not 16-bit aligned. Two memory accesses needed.
                m_Cycles += 2;

                byte byte0 = s[address];
                byte byte1 = s[(ushort)(address + 1)];

                return (ushort)(byte1 << 8 + byte0);
            }
            else
            {
                // This read is 16-bit aligned. Only one memory access is needed.
                m_Cycles += 1;

                byte byte0 = s[address];
                byte byte1 = s[(ushort)(address + 1)];

                return (ushort)((byte1 << 8) + byte0);
            }
        }

        public void WriteMemInt8(ushort address, byte value, SegmentIndex segment)
        {
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            m_Cycles += 1;

            Segment s = GetSegment(segment);
            if (s == null)
                return;

            s[address] = value;
        }

        public void WriteMemInt16(ushort address, ushort value, SegmentIndex segment)
        {
            Segment s = GetSegment(segment);
            if (s == null)
                return;

            if ((address & 0x0001) == 0x0001)
            {
                // This read is not 16-bit aligned. Two memory accesses needed.
                m_Cycles += 2;

                s[address] = (byte)(value & 0x00ff);
                s[(ushort)(address + 1)] = (byte)(value >> 8);
            }
            else
            {
                // This read is 16-bit aligned. Only one memory access is needed.
                m_Cycles += 1;

                s[address] = (byte)(value & 0x00ff);
                s[(ushort)(address + 1)] = (byte)(value >> 8);
            }
        }

        public ushort DebugReadMemory(ushort address, SegmentIndex segmentType)
        {
            long cycles = m_Cycles;
            ushort memory = ReadMemInt16(address, segmentType);
            m_Cycles = cycles;
            return memory;
        }
    }
}
