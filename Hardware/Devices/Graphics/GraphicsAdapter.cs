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

        public override IMemoryBank GetMemoryBank(ushort bank_index)
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
                case 0x0001: // READ CHR RAM
                    break;
                case 0x0002: // READ PAL RAM
                    break;
                case 0x0003: // SET BORDER COLOR
                    break;
                case 0x0004: // DUMP CHR RAM
                    break; 
                case 0x0005: // DUMP PAL RAM
                    break;
            }
            return 0x0000;
        }

        public override void Update()
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

        public override void Display(IRenderer spritebatch)
        {
            switch (m_GraphicsMode)
            {
                case GraphicsMode.None:
                    // do nothing;
                    return;
                case GraphicsMode.LEM1802:
                    Draw_LEM(spritebatch);
                    return;
            }
        }

        // Internal Variables
        GraphicsMode m_GraphicsMode = GraphicsMode.None;

        MemoryBankLEM m_BankLEM;
        Platform.YTexture m_LEM_CHRRAM, m_LEM_PALRAM;

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

                m_LEM_CHRRAM = Platform.YTexture.Create(128, 32);
                m_LEM_PALRAM = Platform.YTexture.Create(16, 1);

                ushort[] chrram_default = new ushort[256];
                System.Buffer.BlockCopy(YCPU.ResContent.lem1802_charset, 0, chrram_default, 0, 512);
                for (int i = 0; i < 256; i++)
                    m_BankLEM[0x0800 + i] = chrram_default[i];

                ushort[] palram_default = new ushort[16];
                System.Buffer.BlockCopy(YCPU.ResContent.lem1802_16bitpal, 0, palram_default, 0, 32);
                for (int i = 0; i < 16; i++)
                    m_BankLEM[0x0C00 + i] = palram_default[i];
            }
        }

        private void Update_LEM()
        {
            if (m_BankLEM.SCRRAM_Delta)
            {

            }

            if (m_BankLEM.CHRRAM_Delta)
            {
                Update_LEM_CHRRAM();
                m_BankLEM.CHRRAM_Delta = false;
            }

            if (m_BankLEM.PALRAM_Delta)
            {
                Update_LEM_PALRAM();
                m_BankLEM.PALRAM_Delta = false;
            }
        }

        private void Update_LEM_CHRRAM()
        {
            // Assume CHRRAM format is Color (ARGB8888)
            uint[] data = new uint[128 * 32];
            int data_index = 0;
            for (int iTile = 0; iTile < 128; iTile++)
            {
                int y = (iTile / 32) * 8;
                int x = (iTile % 32) * 4;
                for (int i = 0; i < 2; i++)
                {
                    ushort binary_data = m_BankLEM[0x0800 + data_index++];
                    int bit_index = 0;
                    for (int iY = 0; iY < 4; iY++)
                    {
                        for (int iX = 0; iX < 4; iX++)
                        {
                            bool bit = ((binary_data & (1 << bit_index++)) != 0);
                            data[(y + i * 4 + iY) * 128 + (x + iX)] = (bit) ? 0xFFFFFFFF : 0xFF000000;
                        }
                    }
                }
            }
            m_LEM_CHRRAM.SetData(data);
        }

        private void Update_LEM_PALRAM()
        {
            // Assume PALRAM format is Color (ARGB8888)
            uint[] data = new uint[0x10];
            for (int i = 0; i < 0x10; i++)
            {
                ushort color = m_BankLEM[0x0C00 + i];
                data[i] = (uint)(0xFF000000) | ((uint)(color & 0x0F00) << 12) | ((uint)(color & 0x00F0) << 8) | ((uint)(color & 0x000F) << 4);
            }

            m_LEM_PALRAM.SetData(data);
        }

        private void Draw_LEM(IRenderer renderer)
        {
            renderer.Palette_LEM = m_LEM_PALRAM;
            int current_word = 0;
            for (int y = 0; y < 12; y++)
                for (int x = 0; x < 32; x++)
                    m_LEM_CHRRAM.DrawLEM(renderer, x, y, m_BankLEM[current_word++]);
        }

        enum GraphicsMode
        {
            None,
            LEM1802
        }
    }
}
