using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Hardware;
using Ypsilon.Platform.Support;

namespace Ypsilon.Devices.Graphics
{
    public class GraphicsAdapter : ADevice
    {
        protected override ushort DeviceType
        {
            get { return DeviceTypeGraphicsAdapter; }
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
            m_Bank = new MemoryBankLEM();
        }

        protected override void Initialize()
        {
            SetMode_None();
        }

        public override void Dispose()
        {
            m_Bank = null;

            if (m_LEM_CHRRAM != null)
            {
                m_LEM_CHRRAM.Dispose();
                m_LEM_CHRRAM = null;
            }

            if (m_LEM_PALRAM != null)
            {
                m_LEM_PALRAM.Dispose();
                m_LEM_PALRAM = null;
            }
        }

        public override IMemoryBank GetMemoryBank(ushort bank_index)
        {
            return m_Bank;
        }

        protected override ushort ReceiveMessage(ushort param_0, ushort param_1)
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
                default:
                    return MSG_ERROR;
            }
            return MSG_ACK;
        }

        public override void Display(IRenderer spritebatch)
        {
            switch (m_GraphicsMode)
            {
                case GraphicsMode.None:
                    // do nothing;
                    return;
                case GraphicsMode.LEM1802:
                    Update_LEM();
                    Draw_LEM(spritebatch);
                    return;
            }
        }

        // Internal Variables
        GraphicsMode m_GraphicsMode = GraphicsMode.None;

        IMemoryBank m_Bank;
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
                m_LEM_CHRRAM = Platform.YTexture.Create(128, 32);
                m_LEM_PALRAM = Platform.YTexture.Create(16, 1);

                byte[] chrram_default = new byte[512];
                System.Buffer.BlockCopy(ResContent.lem1802_charset, 0, chrram_default, 0, 512);
                for (int i = 0; i < 512; i += 1)
                    m_Bank[0x0800 + i] = chrram_default[i];

                byte[] palram_default = new byte[32];
                System.Buffer.BlockCopy(ResContent.lem1802_16bitpal, 0, palram_default, 0, 32);
                for (int i = 0; i < 32; i += 1)
                    m_Bank[0x0C00 + i] = palram_default[i];

                m_GraphicsMode = GraphicsMode.LEM1802;
            }
        }

        private void Update_LEM()
        {
            MemoryBankLEM lem = (MemoryBankLEM)m_Bank;
            if (lem.SCRRAM_Delta)
            {

            }

            if (lem.CHRRAM_Delta)
            {
                Update_LEM_CHRRAM();
                lem.CHRRAM_Delta = false;
            }

            if (lem.PALRAM_Delta)
            {
                Update_LEM_PALRAM();
                lem.PALRAM_Delta = false;
            }
        }

        private void Update_LEM_CHRRAM()
        {
            // Assume CHRRAM format is Color (ARGB8888)
            // Each character is 4x8 pixels at 1bit depth, 4 bytes total.
            // byte 0, bit 0-3: 3210
            // byte 0, bit 4-7: 7654
            // byte 1, bit 0-3: 3210
            // byte 1, bit 4-7: 7654
            // ... same for bytes 2 and 3.
            uint[] data = new uint[128 * 32];
            int data_index = 0;
            for (int iTile = 0; iTile < 128; iTile += 1)
            {
                int y = (iTile / 32) * 8;
                int x = (iTile % 32) * 4;
                for (int i = 0; i < 4; i += 1)
                {
                    byte binary_data = m_Bank[0x0800 + data_index];
                    data_index += 1;
                    int bit_index = 0;
                    for (int iY = 0; iY < 2; iY += 1)
                    {
                        for (int iX = 0; iX < 4; iX += 1)
                        {
                            bool bit = ((binary_data & (1 << bit_index)) != 0);
                            bit_index += 1;
                            data[(y + i * 2 + iY) * 128 + (x + iX)] = (bit) ? 0xFFFFFFFF : 0xFF000000;
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
            for (int i = 0; i < 0x10; i += 1)
            {
                ushort color = (ushort)(m_Bank[0x0C00 + i * 2] + (m_Bank[0x0C00 + i * 2 + 1] << 8));
                data[i] = (uint)(0xFF000000) | ((uint)(color & 0x0F00) << 12) | ((uint)(color & 0x00F0) << 8) | ((uint)(color & 0x000F) << 4);
            }

            m_LEM_PALRAM.SetData(data);
        }

        private void Draw_LEM(IRenderer renderer)
        {
            renderer.Palette_LEM = m_LEM_PALRAM;
            int current_word = 0;
            for (int y = 0; y < 12; y += 1)
                for (int x = 0; x < 32; x += 1)
                {
                    byte byte0 = m_Bank[current_word++];
                    byte byte1 = m_Bank[current_word++];
                    m_LEM_CHRRAM.DrawLEM(renderer, x, y, byte0, byte1);
                }
        }

        enum GraphicsMode
        {
            None,
            LEM1802
        }
    }
}
