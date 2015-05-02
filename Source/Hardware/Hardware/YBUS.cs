using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Hardware
{
    public class YBUS
    {
        private List<Devices.BaseDevice> m_Devices;

        public YBUS()
        {
            m_Devices = new List<Devices.BaseDevice>();
        }

        public void SetupDebugDevices()
        {
            // DEBUG set up of devices.
            m_Devices.Add(new Devices.Graphics.GraphicsAdapter(this));
            SendDeviceMessage(0x0000, 0x0000, 0x0001, 0x0000); // set graphics adapter to LEM mode.
        }

        public void Update()
        {
            for (int i = 0; i < m_Devices.Count; i += 1)
                m_Devices[i].Update();
        }

        public void Display(IRenderer renderer)
        {
            for (int i = 0; i < m_Devices.Count; i += 1)
                m_Devices[i].Display(renderer);
        }

        public ushort DevicesConnected
        {
            get { return (ushort)m_Devices.Count; }
        }

        public ushort[] QueryDevice(ushort device_index)
        {
            if (m_Devices.Count <= device_index)
            {
                return m_Devices[device_index].Bus_DeviceQuery();
            }
            else
            {
                ushort[] info = new ushort[0x04];
                info[0] = 0x0000;
                info[1] = 0x0000;
                info[2] = 0x0000;
                info[3] = 0x0000;
                return info;
            }
        }

        public void SendDeviceMessage(ushort device_index, ushort param_0, ushort param_1, ushort param_2)
        {
            if (device_index < m_Devices.Count)
                m_Devices[device_index].Bus_SendMessage(param_0, param_1, param_2);
        }

        private List<ushort> m_DevicesRaisingIRQ = new List<ushort>();

        public void Device_RaiseIRQ(Devices.BaseDevice device)
        {
            int device_index = (int)m_Devices.IndexOf(device);
            if (device_index == -1)
            {
                // should never occur
                return; 
            }
            else
            {
                if (!m_DevicesRaisingIRQ.Contains((ushort)device_index))
                    m_DevicesRaisingIRQ.Add((ushort)device_index);
            }
        }

        public void AcknowledgeIRQ(ushort device_index)
        {
            if (m_DevicesRaisingIRQ.Contains(device_index))
            {
                m_Devices[device_index].IRQAcknowledged();
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
                    return 0xFFFF;
                return m_DevicesRaisingIRQ.Min();
            }
        }

        public IMemoryBank GetMemoryBank(ushort device_index, ushort bank_index)
        {
            if (device_index >= m_Devices.Count)
                return null;
            return m_Devices[device_index].GetMemoryBank((ushort)(bank_index & 0x0FFF));
        }
    }
}
