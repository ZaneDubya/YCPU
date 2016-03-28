using Microsoft.Xna.Framework.Graphics;
using Ypsilon.Core.Graphics;
using Ypsilon.Emulation;

namespace Ypsilon.Providers
{
    public class DisplayProvider : IDisplayProvider
    {
        // YPSILON STUFF

        private Texture2D m_LEM;
        private uint[] m_LEMData;

        private readonly SpriteBatchExtended m_SpriteBatch;

        public ITexture RenderLEM(byte[] devicemem, uint[] chr, uint[] pal, bool selectPage1, bool doSprites)
        {
            if (m_LEM == null)
            {
                m_LEM = m_SpriteBatch.NewTexture(128, 96);
                m_LEMData = new uint[128 * 96];
            }

            uint tileBase = (uint)(selectPage1 ? 0x0400 : 0x0000);
            for (int y = 0; y < 12; y += 1)
            {
                for (int x = 0; x < 32; x += 1)
                {
                    byte data0 = devicemem[tileBase++];
                    byte data1 = devicemem[tileBase++];
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

            if (doSprites)
            {
                const uint oamByte0 = 0x0E00;
                const uint oamByte1 = 0x0E10;
                const uint oamByte2 = 0x0E20;
                const uint oamByte3 = 0x0E30;

                for (int oam = 0; oam < 16; oam++)
                {
                    const int width = 8, height = 8;

                    int y = devicemem[oamByte0 + oam];
                    int attr = devicemem[oamByte2 + oam];
                    int x = devicemem[oamByte3 + oam];
                    bool highnibble = false;

                    bool hflip = (attr & 0x01) != 0;
                    bool vflip = (attr & 0x02) != 0;

                    for (int iy = 0; iy < height; iy++)
                    {
                        int screeny = iy + y;

                        // Y clipping
                        if (screeny < 0 || screeny >= 96)
                            continue;

                        int baseSprite = 0x8000 + devicemem[oamByte1 + oam] * 32 + (vflip ?(7 - iy) * (width / 2) : iy * (width / 2));
                        int xInc = 1;
                        if (hflip)
                        {
                            baseSprite += (width / 2) - 1;
                            xInc = -xInc;
                            highnibble = true;
                        }

                        for (int screenx = x; screenx < x + width; screenx++)
                        {
                            // X clipping
                            if (screenx >= 0 && screenx < 128)
                            {
                                int sprdata = devicemem[baseSprite];
                                int color = (highnibble) ? sprdata >> 4 : sprdata & 0x0f;
                                //if (color != 0)
                                {
                                    m_LEMData[screeny * 128 + screenx] = pal[color];
                                }
                            }

                            if (hflip)
                            {
                                if (highnibble)
                                {
                                    highnibble = false;
                                }
                                else
                                {
                                    highnibble = true;
                                    baseSprite += xInc;
                                }
                            }
                            else
                            {
                                if (highnibble)
                                {
                                    highnibble = false;
                                    baseSprite += xInc;
                                }
                                else
                                {
                                    highnibble = true;
                                }
                            }
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
