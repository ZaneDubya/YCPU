using Ypsilon.Emulation.Processor;

namespace Ypsilon.Emulation.Devices.Input {
    public class KeyboardDevice : ADevice {
        private const ushort AltDown = 0x2000;

        private const ushort CtrlDown = 0x1000;
        private const ushort EventDown = 0x0200;
        private const ushort EventPress = 0x0300;

        private const ushort EventUp = 0x0100;
        private const ushort ExtendedKey = 0x8000;
        private const ushort ShiftDown = 0x4000;
        private ushort m_BufferCount;
        private readonly ushort[] m_CommandBuffer = new ushort[16];

        private bool m_ReportUpDownEvents, m_ReportPressEvents, m_TranslateASCII;

        protected override ushort DeviceID => 0x0000;

        protected override ushort DeviceRevision => 0x0000;
        protected override ushort DeviceType => DeviceTypeKeyboard;

        protected override ushort ManufacturerID => 0x0000;

        public KeyboardDevice(YBUS bus)
            : base(bus) {}

        protected override void Initialize() {
            m_ReportUpDownEvents = false;
            m_ReportPressEvents = false;
            m_TranslateASCII = false;
        }

        protected override ushort ReceiveMessage(ushort param0, ushort param1) {
            switch (param0) {
                case 0x0000: // SET MODE
                    m_BufferCount = 0;
                    for (int i = 0; i < m_CommandBuffer.Length; i++)
                        m_CommandBuffer[i] = 0;
                    m_ReportUpDownEvents = (param1 & 0x0001) != 0;
                    m_ReportPressEvents = (param1 & 0x0002) != 0;
                    m_TranslateASCII = (param1 & 0x0004) != 0;
                    return MSG_ACK;
                case 0x0001: // GET PENDING EVENTS, param1 is ptr to buffer in memory.
                    ushort address = param1;
                    for (int i = 0; i < m_BufferCount; i++) {
                        address += 2;
                        BUS.CPU.WriteMemInt16(address, m_CommandBuffer[i], SegmentIndex.DS);
                    }
                    for (int i = 0; i < m_CommandBuffer.Length; i++)
                        m_CommandBuffer[i] = 0;
                    ushort r0 = m_BufferCount;
                    m_BufferCount = 0;
                    return r0;
                default:
                    return MSG_ERROR;
            }
        }

        public override void Update(IInputProvider input) {
            ushort keycode;
            while (input.TryGetKeyboardEvent(m_TranslateASCII, out keycode)) {
                if (m_BufferCount >= 16)
                    continue;
                if (m_ReportUpDownEvents && ((keycode & 0x0F00) == EventUp) || ((keycode & 0x0F00) == EventDown)) {
                    m_CommandBuffer[m_CommandBuffer[m_BufferCount++]] = keycode;
                }
                else if (m_ReportPressEvents && ((keycode & 0x0F00) == EventPress)) {
                    m_CommandBuffer[m_CommandBuffer[m_BufferCount++]] = keycode;
                }
            }
        }
    }
}