using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Devices
{
    public abstract class BaseDevice
    {
        protected abstract ushort DeviceType { get; }
        protected abstract ushort ManufacturerID { get; }
        protected abstract ushort DeviceID { get; }
        protected abstract ushort DeviceRevision { get; }

        protected abstract void Initialize();
        protected abstract IMemoryBank GetMemoryBank(ushort bank_index);
        protected abstract ushort ReceiveMessage(ushort param_0, ushort param_1, ushort param_2);
        public abstract void Update();
        public abstract void Display(Platform.Graphics.SpriteBatchExtended spritebatch);

        private Hardware.YBUS m_BUS;
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
            m_BUS.Device_RaiseIRQ(this);
        }

        public void IRQAcknowledged()
        {
            m_IRQ = false;
        }

        public BaseDevice(Hardware.YBUS bus)
        {
            m_BUS = bus;
            Initialize();
        }

        public ushort[] Bus_DeviceQuery()
        {
            ushort[] info = new ushort[0x04];
            info[0] = DeviceType;
            info[1] = ManufacturerID;
            info[2] = DeviceID;
            info[3] = DeviceRevision;
            return info;
        }

        public ushort Bus_SendMessage(ushort param_0, ushort param_1, ushort param_2)
        {
            return ReceiveMessage(param_0, param_1, param_2);
        }
    }
}
