using System;
using Ypsilon.Emulation.Processor;

namespace Ypsilon.Emulation.Devices.Storage {
    public class CRW200 : ADevice, IMemoryInterface {
        private const ushort ERROR_BAD_SECTOR = 0x0005;
        private const ushort ERROR_BROKEN = 0xFFFF;
        private const ushort ERROR_BUSY = 0x0001;
        private const ushort ERROR_EJECT = 0x0004;
        private const ushort ERROR_NO_MEDIA = 0x0002;

        private const ushort ERROR_NONE = 0x0000;
        private const ushort ERROR_PROTECTED = 0x0003;

        private readonly byte[] m_Buffer = new byte[512];
        private ushort m_DeviceState = 0x0000;
        private ushort m_ErrorCode = 0x0000;

        protected override ushort DeviceID => 0x0200;

        protected override ushort DeviceRevision => 0x0009;
        protected override ushort DeviceType => DeviceTypeStorage;

        protected override ushort ManufacturerID => 0x0000;

        public CRW200(YBUS bus)
            : base(bus) {}

        public byte this[uint address] {
            get {
                address &= 0x1ff;
                return m_Buffer[address];
            }

            set {
                address &= 0x1ff;
                m_Buffer[address] = value;
            }
        }

        public override IMemoryInterface GetMemoryInterface() {
            return this;
        }

        protected override void Initialize() {
            throw new NotImplementedException();
        }

        protected override ushort ReceiveMessage(ushort param0, ushort param1) {
            throw new NotImplementedException();
        }

        public override void Update(IInputProvider input) {}
    }
}