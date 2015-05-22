using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Ypsilon.Platform.Graphics;
using Microsoft.Xna.Framework;
using Ypsilon.Hardware;

namespace Ypsilon
{
    public class DeviceRenderer : IDeviceRenderer
    {
        // YPSILON STUFF

        Texture2D m_LEM_chr, m_LEM_pal;

        public void RenderLEM(IMemoryBank bank, uint[] chr, uint[] pal)
        {
            if (m_LEM_chr == null)
            {
                m_LEM_chr = new Texture2D(m_SpriteBatch.GraphicsDevice, 128, 32);
                m_LEM_pal = new Texture2D(m_SpriteBatch.GraphicsDevice, 16, 1);
            }

            m_LEM_chr.SetData<uint>(chr);
            m_LEM_pal.SetData<uint>(pal);

            int current_word = 0;
            for (int y = 0; y < 12; y += 1)
            {
                for (int x = 0; x < 32; x += 1)
                {
                    byte data0 = bank[current_word++];
                    byte data1 = bank[current_word++];

                    int character = data0 & 0x7F;
                    m_SpriteBatch.GUIDrawSprite(m_LEM_chr, 
                        new Rectangle(x * 8, y * 16, 8, 16),
                        new Rectangle((character % 32) * 4, (character / 32) * 8, 4, 8),
                        shader: Platform.Graphics.Shader.LEM1802,
                        Palette0: (data1 & 0x0F),
                        Palette1: (data1 & 0xF0) >> 4);
                }
            }
        }

        // BASE STUFF

        private ExtendedSpriteBatch m_SpriteBatch;

        public DeviceRenderer(ExtendedSpriteBatch spriteBatch)
        {
            m_SpriteBatch = spriteBatch;
        }
    }
}
