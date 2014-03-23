using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Simware
{
    class YBUS
    {
        public void Update()
        {

        }

        public ushort DevicesConnected
        {
            get { return 0x00; }
        }

        public ushort[] QueryDevice(ushort index)
        {
            ushort[] info = new ushort[0x04];
            info[0] = 0x0000;
            info[1] = 0x0000;
            info[2] = 0x0000;
            info[3] = 0x0000;
            return info;
        }

        public void SendDeviceMessage(ushort device_index, ushort param_0, ushort param_1, ushort param_2)
        {

        }

        private List<ushort> m_DevicesRaisingIRQ = new List<ushort>();

        private void Device_RaiseIRQ(ushort device_index)
        {
            if (!m_DevicesRaisingIRQ.Contains(device_index))
                m_DevicesRaisingIRQ.Add(device_index);
        }

        public void AcknowledgeIRQ(ushort device_index)
        {
            if (m_DevicesRaisingIRQ.Contains(device_index))
            {
                m_DevicesRaisingIRQ.Remove(device_index);
            }
        }

        public bool IRQ
        {
            get { return m_DevicesRaisingIRQ.Count > 0; }
        }

        public ushort II
        {
            get
            {
                if (m_DevicesRaisingIRQ.Count == 0)
                    return 0xDEAD;
                return m_DevicesRaisingIRQ.Min();
            }
        }
    }
}
