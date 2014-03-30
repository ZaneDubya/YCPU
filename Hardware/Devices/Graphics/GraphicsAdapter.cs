using Microsoft.Xna.Framework.Graphics;

namespace YCPU.Devices.Graphics
{
    class GraphicsAdapter : BaseDevice
    {
        protected override ushort DeviceType
        {
            get { return 0x0000; }
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

        public GraphicsAdapter(Hardware.YBUS bus)
            : base(bus)
        {

        }

        protected override void Initialize()
        {
            SetMode_None();
        }

        protected override IMemoryBank GetMemoryBank(ushort bank_index)
        {
            return m_BankLEM;
        }

        protected override ushort ReceiveMessage(ushort param_0, ushort param_1, ushort param_2)
        {
            switch (param_0)
            {
                case 0x0000: // SET_MODE
                    SetMode(param_1);
                    break;
            }
            return 0x0000;
        }

        protected override void Update()
        {
            switch (m_GraphicsMode)
            {
                case GraphicsMode.None:
                    // do nothing;
                    return;
                case GraphicsMode.LEM1802:
                    Update_LEM();
                    return;
            }
        }

        protected override void Display(Platform.Graphics.SpriteBatchExtended spritebatch)
        {

        }

        // Internal Variables
        GraphicsMode m_GraphicsMode = GraphicsMode.None;

        MemoryBankLEM m_BankLEM;
        Texture2D m_LEM_CHRRAM, m_LEM_PALRAM;

        // Internal Routines
        private void SetMode(ushort i)
        {
            switch (i)
            {
                case 0x0000:
                    SetMode_None();
                    break;
                case 0x0001:
                    SetMode_LEM();
                    break;
            }
        }
        private void SetMode_None()
        {
            m_GraphicsMode = GraphicsMode.None;
        }

        private void SetMode_LEM()
        {
            if (m_GraphicsMode != GraphicsMode.LEM1802)
            {
                m_GraphicsMode = GraphicsMode.LEM1802;
                m_BankLEM = new MemoryBankLEM();
                m_LEM_CHRRAM = Platform.Support.Common.CreateTexture(128, 32);
                m_LEM_PALRAM = Platform.Support.Common.CreateTexture(16, 1);
            }
        }

        private void Update_LEM()
        {

        }

        enum GraphicsMode
        {
            None,
            LEM1802
        }
    }
}
