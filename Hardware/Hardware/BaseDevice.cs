using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Hardware
{
    class BaseDevice
    {
        protected ushort m_DeviceType = 0x0000;
        protected ushort m_DeviceID = 0x0000;
        protected ushort m_ManufacturerID = 0x0000;
        protected ushort m_HardwareRevision = 0x0000;

        private Platform.YBUS m_BUS;
        private bool m_IRQ = false;
        public bool IRQ
        {
            get
            {
                return m_IRQ;
            }
        }

        protected void RaiseIRQ()
        {
            m_IRQ = true;
        }

        public void IRQAcknowledged()
        {
            m_IRQ = false;
        }

        public BaseDevice(Platform.YBUS bus)
        {
            m_BUS = bus;
            Initialize();
        }

        protected virtual void Initialize()
        {

        }

        public ushort[] DeviceQuery()
        {
            ushort[] info = new ushort[0x04];
            info[0] = m_DeviceType;
            info[1] = m_DeviceID;
            info[2] = m_ManufacturerID;
            info[3] = m_HardwareRevision;
            return info;
        }

        

        public void ReceiveMessage(ushort param_0, ushort param_1, ushort param_2)
        {

        }

        
    }
}
