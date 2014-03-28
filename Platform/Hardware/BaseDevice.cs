using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Hardware
{
    abstract class BaseDevice
    {
        protected abstract ushort DeviceType { get; }
        protected abstract ushort ManufacturerID { get; }
        protected abstract ushort DeviceID { get; }
        protected abstract ushort DeviceRevision { get; }

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
            info[0] = DeviceType;
            info[1] = ManufacturerID;
            info[2] = DeviceID;
            info[3] = DeviceRevision;
            return info;
        }

        

        public void ReceiveMessage(ushort param_0, ushort param_1, ushort param_2)
        {

        }

        
    }
}
