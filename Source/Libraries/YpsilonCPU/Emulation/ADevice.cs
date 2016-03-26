using System.Collections.Generic;
using Ypsilon.Emulation.Processor;

namespace Ypsilon.Emulation
{
    public abstract class ADevice
    {
        protected const ushort DeviceTypeGraphicsAdapter = 0x0001;
        protected const ushort DeviceTypeKeyboard = 0x0002;
        protected const ushort DeviceTypeStorage = 0x0003;

        public const ushort MSG_NO_DEVICE = 0x0000;
        public const ushort MSG_ACK = 0x0001;
        public const ushort MSG_WAIT = 0x0002;
        public const ushort MSG_ERROR = 0xFFFF;

        protected abstract ushort DeviceType { get; }
        protected abstract ushort ManufacturerID { get; }
        protected abstract ushort DeviceID { get; }
        protected abstract ushort DeviceRevision { get; }

        protected abstract void Initialize();
        protected abstract ushort ReceiveMessage(ushort param0, ushort param1);

        public virtual IMemoryInterface GetMemoryInterface()
        {
            return null;
        }

        public virtual void Dispose()
        {

        }

        public virtual void Update(IInputProvider input)
        {

        }

        public virtual void Display(int busIndex, List<ITexture> textures, IDisplayProvider renderer)
        {

        }

        protected YBUS BUS;

        private bool m_IRQ;
        public bool IRQ => m_IRQ;

        protected void RaiseIRQ()
        {
            m_IRQ = true;
            BUS.Device_RaiseIRQ(this);
        }

        public void IRQAcknowledged()
        {
            m_IRQ = false;
        }

        public ADevice(YBUS bus)
        {
            BUS = bus;
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

        public ushort Bus_SendMessage(ushort param0, ushort param1)
        {
            return ReceiveMessage(param0, param1);
        }
    }
}
