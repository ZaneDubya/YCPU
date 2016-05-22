using System.Collections.Generic;
using Ypsilon.Emulation.Processor;

namespace Ypsilon.Emulation {
    public abstract class ADevice {
        public const ushort MSG_ACK = 0x0001;
        public const ushort MSG_ERROR = 0xFFFF;

        public const ushort MSG_NO_DEVICE = 0x0000;
        public const ushort MSG_WAIT = 0x0002;
        protected const ushort DeviceTypeGraphicsAdapter = 0x0001;
        protected const ushort DeviceTypeKeyboard = 0x0002;
        protected const ushort DeviceTypeStorage = 0x0003;

        protected YBUS BUS;

        public bool IRQ { get; private set; }

        protected abstract ushort DeviceID { get; }
        protected abstract ushort DeviceRevision { get; }

        protected abstract ushort DeviceType { get; }
        protected abstract ushort ManufacturerID { get; }

        public ADevice(YBUS bus) {
            BUS = bus;
            Initialize();
        }

        public ushort[] Bus_DeviceQuery() {
            ushort[] info = new ushort[0x04];
            info[0] = DeviceType;
            info[1] = ManufacturerID;
            info[2] = DeviceID;
            info[3] = DeviceRevision;
            return info;
        }

        public ushort Bus_SendMessage(ushort param0, ushort param1) {
            return ReceiveMessage(param0, param1);
        }

        public virtual void Display(int busIndex, List<ITexture> textures, IDisplayProvider renderer) {}

        public virtual void Dispose() {}

        public virtual IMemoryInterface GetMemoryInterface() {
            return null;
        }

        protected abstract void Initialize();

        public void IRQAcknowledged() {
            IRQ = false;
        }

        protected void RaiseIRQ() {
            IRQ = true;
            BUS.Device_RaiseIRQ(this);
        }

        protected abstract ushort ReceiveMessage(ushort param0, ushort param1);

        public virtual void Update(IInputProvider input) {}
    }
}