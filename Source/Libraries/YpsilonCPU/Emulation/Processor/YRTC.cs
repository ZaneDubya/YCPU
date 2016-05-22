using System;

namespace Ypsilon.Emulation.Processor {
    internal class YRTC {
        public bool IsEnabled;
        private long m_CyclesPerTick;
        private long m_NextTickAtCycle;

        public void DisableInterrupt() {
            IsEnabled = false;
        }

        public ushort[] GetData() {
            ushort[] info = new ushort[0x04];
            DateTime now = DateTime.Now;
            byte year = (byte)(now.Year - 1900);
            byte month = (byte)now.Month;
            byte day = (byte)now.Day;
            byte hour = (byte)now.Hour;
            byte minute = (byte)now.Minute;
            byte second = (byte)now.Second;
            ushort tick = (ushort)now.Millisecond;
            info[0] = (ushort)((year << 8) + month);
            info[1] = (ushort)((day << 8) + hour);
            info[2] = (ushort)((minute << 8) + second);
            info[3] = tick;
            return info;
        }

        public ushort GetTickRate() {
            int tickHz = (int)(YCPU.ClockRateHz / m_CyclesPerTick);
            return (ushort)tickHz;
        }

        public bool IRQ(long cycle) {
            if (cycle >= m_NextTickAtCycle) {
                m_NextTickAtCycle += m_CyclesPerTick;
                return true;
            }
            return false;
        }

        public ushort SetTickRate(ushort value, long cycle) {
            if (value == 0x0000) {
                DisableInterrupt();
                return 0;
            }
            IsEnabled = true;
            double tickHz = value;
            double maxHz = YCPU.ClockRateHz / 1024d;
            if (tickHz > maxHz)
                tickHz = maxHz;
            m_CyclesPerTick = (int)(YCPU.ClockRateHz / tickHz);
            m_NextTickAtCycle = cycle + m_CyclesPerTick;
            return (ushort)tickHz;
        }
    }
}