using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ypsilon;

namespace YCPUXNA.ServiceProviders
{
    public class DeviceRenderService : IDeviceRenderer
    {
        // YPSILON STUFF

        Texture2D m_LEM;
        uint[] m_LEM_Data;

        SpriteBatch m_SpriteBatch;

        public void RenderLEM(IMemoryBank bank, uint[] chr, uint[] pal)
        {
            if (m_LEM == null)
            {
                m_LEM = new Texture2D(m_SpriteBatch.GraphicsDevice, 128, 96);
                m_LEM_Data = new uint[128 * 96];
            }

            int current_word = 0;
            for (int y = 0; y < 12; y += 1)
            {
                for (int x = 0; x < 32; x += 1)
                {
                    byte data0 = bank[current_word++];
                    byte data1 = bank[current_word++];
                    uint color0 = pal[(data1 & 0x0f)];
                    uint color1 = pal[(data1 & 0xf0) >> 4];
                    uint character = chr[data0 & 0x7F];

                    int offset = y * 8 * 128 + x * 4;

                    for (int iy = 0; iy < 8; iy++)
                    {
                        for (int ix = 0; ix < 4; ix++)
                        {
                            m_LEM_Data[offset + iy * 128 + ix] = ((character & 0x00000001) == 1) ? color1 : color0;
                            character >>= 1;
                        }
                    }
                }
            }
            m_LEM.SetData<uint>(m_LEM_Data);
            m_SpriteBatch.Draw(m_LEM, new Rectangle(0, 0, 256, 192), Color.White);
        }

        // BASE STUFF

        public DeviceRenderService(SpriteBatch spriteBatch)
        {
            m_SpriteBatch = spriteBatch;
        }
    }
}
