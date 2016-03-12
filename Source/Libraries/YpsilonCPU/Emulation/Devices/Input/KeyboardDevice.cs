using Ypsilon.Emulation.Hardware;

namespace Ypsilon.Emulation.Devices.Input
{
    public class KeyboardDevice : ADevice
    {
        protected override ushort DeviceType
        {
            get { return DeviceTypeKeyboard; }
        }
        protected override ushort ManufacturerID
        {
            get { return 0x0000; }
        }
        protected override ushort DeviceID
        {
            get { return 0x0000; }
        }
        protected override ushort DeviceRevision
        {
            get { return 0x0000; }
        }

        public KeyboardDevice(YBUS bus)
            : base(bus)
        {
            m_CommandBuffer = new ushort[16];
        }

        private bool m_GetOnlyPressEvents = false;
        private ushort[] m_CommandBuffer;

        private const ushort EventUp = 0x0100;
        private const ushort EventDown = 0x0200;
        private const ushort EventPress = 0x0300;
        private const ushort EventAscii = 0x0400;

        private const ushort CtrlDown = 0x1000;
        private const ushort AltDown = 0x2000;
        private const ushort ShiftDown = 0x4000;

        protected override void Initialize()
        {

        }

        protected override ushort ReceiveMessage(ushort param_0, ushort param_1)
        {
            switch (param_0)
            {
                case 0x0000: // SET MODE
                    for (int i = 0; i < m_CommandBuffer.Length; i++)
                        m_CommandBuffer[i] = 0;
                    if (param_1 == 0)
                    {
                        m_GetOnlyPressEvents = true;
                    }
                    else if (param_1 == 1)
                    {
                        m_GetOnlyPressEvents = false;
                    }
                    else
                    {
                        return MSG_ERROR;
                    }
                    break;
                case 0x0001: // GET PENDING EVENTS, R1 is ptr.
                    ushort address = param_1;
                    BUS.CPU.WriteMem16(address, m_CommandBuffer[0], SegmentIndex.DS);
                    for (int i = 0; i < m_CommandBuffer[0]; i++)
                    {
                        address += 2;
                        BUS.CPU.WriteMem16(address, m_CommandBuffer[i + 1], SegmentIndex.DS);
                    }
                    for (int i = 0; i < m_CommandBuffer.Length; i++)
                        m_CommandBuffer[i] = 0;
                    break;
                default:
                    return MSG_ERROR;
            }
            return MSG_ACK;
        }

        public override void Update(IInputProvider input)
        {
            ushort keycode;
            while (input.TryGetKeyboardEvent(out keycode))
            {
                if (m_GetOnlyPressEvents && ((keycode & 0x0F00) == EventUp) || ((keycode & 0x0F00) == EventDown))
                    continue;
                if (m_CommandBuffer[0] < m_CommandBuffer.Length - 1)
                {
                    m_CommandBuffer[0]++;
                    m_CommandBuffer[m_CommandBuffer[0]] = keycode;
                }
            }
        }
    }
}
