using Microsoft.Xna.Framework.Graphics;
using Ypsilon.Core.Graphics;
using Ypsilon.Emulation;

namespace YCPUXNA.Providers
{
    public class DisplayProvider : IDisplayProvider
    {
        // YPSILON STUFF

        private Texture2D m_LEM;
        private uint[] m_LEMData;

        private readonly SpriteBatchExtended m_SpriteBatch;

        public ITexture RenderLEM(byte[] devicemem, uint[] chr, uint[] pal, bool selectPage1)
        {
            if (m_LEM == null)
            {
                m_LEM = m_SpriteBatch.NewTexture(128, 96);
                m_LEMData = new uint[128 * 96];
            }

            uint index = (uint)(selectPage1 ? 0x0400 : 0x0000);
            for (int y = 0; y < 12; y += 1)
            {
                for (int x = 0; x < 32; x += 1)
                {
                    byte data0 = devicemem[index++];
                    byte data1 = devicemem[index++];
                    uint color0 = pal[(data1 & 0x0f)];
                    uint color1 = pal[(data1 & 0xf0) >> 4];
                    uint character = chr[data0 & 0x7F];

                    int offset = y * 8 * 128 + x * 4;

                    for (int iy = 0; iy < 8; iy++)
                    {
                        for (int ix = 0; ix < 4; ix++)
                        {
                            m_LEMData[offset + iy * 128 + ix] = ((character & 0x00000001) == 1) ? color1 : color0;
                            character >>= 1;
                        }
                    }
                }
            }
            m_LEM.SetData(m_LEMData);

            return new YTexture(m_LEM);
        }

        // BASE STUFF

        public DisplayProvider(SpriteBatchExtended spriteBatch)
        {
            m_SpriteBatch = spriteBatch;
        }
    }
}
