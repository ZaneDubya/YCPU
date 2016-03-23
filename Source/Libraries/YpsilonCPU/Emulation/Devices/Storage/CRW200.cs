using System;
using Ypsilon.Emulation.Processor;

namespace Ypsilon.Emulation.Devices.Storage
{
    public class CRW200 : ADevice, IMemoryInterface
    {
        protected override ushort DeviceType
        {
            get { return DeviceTypeStorage; }
        }
        protected override ushort ManufacturerID
        {
            get { return 0x0000; }
        }
        protected override ushort DeviceID
        {
            get { return 0x0200; }
        }
        protected override ushort DeviceRevision
        {
            get { return 0x0009; }
        }

        public byte this[uint address]
        {
            get
            {
                address &= 0x1ff;
                return m_Buffer[address];
            }

            set
            {
                address &= 0x1ff;
                m_Buffer[address] = value;
            }
        }

        public CRW200(YBUS bus)
            : base(bus)
        {

        }

        public override IMemoryInterface GetMemoryInterface()
        {
            return this;
        }

        private byte[] m_Buffer = new byte[512];
        private ushort m_DeviceState = 0x0000;
        private ushort m_ErrorCode = 0x0000;

        private const ushort ERROR_NONE = 0x0000;
        private const ushort ERROR_BUSY = 0x0001;
        private const ushort ERROR_NO_MEDIA = 0x0002;
        private const ushort ERROR_PROTECTED = 0x0003;
        private const ushort ERROR_EJECT = 0x0004;
        private const ushort ERROR_BAD_SECTOR = 0x0005;
        private const ushort ERROR_BROKEN = 0xFFFF;

        protected override void Initialize()
        {
            throw new NotImplementedException();
        }

        protected override ushort ReceiveMessage(ushort param_0, ushort param_1)
        {
            throw new NotImplementedException();
        }

        public override void Update(IInputProvider input)
        {

        }
    }
}
