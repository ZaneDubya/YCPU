using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Simware
{
    class YRTC
    {
        private long m_NextTickAtCycle = 0;
        private long m_TickRate = 0;

        public void SetTickRate(ushort value, long cycle)
        {
            if (value == 0xFFFF)
            {
                DisableInterrupt();
            }
            else
            {
                m_TickRate = (long)(Math.Pow(2, value));
                if (m_TickRate > 1024)
                    m_TickRate = 1024;
                m_NextTickAtCycle = 0;
                IRQ(cycle);
            }
        }

        public void DisableInterrupt()
        {
            m_NextTickAtCycle = long.MaxValue;
        }

        public bool IRQ(long cycle)
        {
            if (cycle >= m_NextTickAtCycle)
            {
                m_NextTickAtCycle += m_TickRate;
                return true;
            }
            else
            {
                return false;
            }
        }

        public ushort[] GetData()
        {
            ushort[] info = new ushort[0x04];
            DateTime now = DateTime.Now;
            byte year = (byte)(now.Year - 2000);
            byte month = (byte)(now.Month);
            byte day = (byte)(now.Day);
            byte hour = (byte)(now.Hour);
            byte minute = (byte)(now.Minute);
            byte second = (byte)(now.Second);
            ushort tick = (ushort)(now.Millisecond);
            info[0] = (ushort)((year << 8) + month);
            info[1] = (ushort)((day << 8) + hour);
            info[2] = (ushort)((minute << 8) + second);
            info[3] = (ushort)(tick);
            return info;
        }
    }
}
