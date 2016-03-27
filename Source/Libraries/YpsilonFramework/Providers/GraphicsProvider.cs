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
                uint oamByte0 = 0x0E00;
                uint oamByte1 = 0x0E10;
                uint oamByte2 = 0x0E20;
                uint oamByte3 = 0x0E30;

                for (int oam = 0; oam < 16; oam++)
                {
                    int y = devicemem[oamByte0 + oam];
                    int baseSprite = 0x8000 + devicemem[oamByte1 + oam] * 32;
                    int attr = devicemem[oamByte2 + oam];
                    int x = devicemem[oamByte3 + oam];
                    int width = 8, height = 8;
                    bool hflip = (attr & 0x01) != 0;
                    bool vflip = (attr & 0x02) != 0;

                    for (int iy = 0; iy < 8; iy++)
                    {
                        int cy = iy + y;

                        // Y clipping
                        if (cy < 0 || cy >= 127)
                            continue;

                        int spritey = vflip ? (7 - iy) : iy;

                        int baseInc = 1;
                        if (hflip)
                        {
                            baseSprite += (width / 8) - 1;
                            baseInc = -baseInc;
                        }

                        // if ((attr0 & (1 << 13)) != 0)
                        // always 32bit color, always blend
                        for (int i = x; i < x + width; i++)
                        {
                            if ((i & 0x1ff) < width && (m_ScanLineWindows[i & 0x1ff] & sprites_in_window) != 0)
                            {
                                int tx = (i - x) & 7;
                                if (hflip)
                                    tx = 7 - tx;
                                int curIdx = baseSprite * 64 + ((spritey & 7) * 8) + tx;
                                uint pixel = tileram[curIdx];
                                // blend the pixel (if necessary) and write it the scanline.
                                uint alpha = pixel & 0xFF000000;
                                if (alpha == 0xFF000000)
                                {
                                    m_ScanLinePixels[(i & 0x1ff)] = pixel;
                                }
                                else if (alpha != 0)
                                {
                                    alpha = alpha >> 24;
                                    uint colora = m_ScanLinePixels[i]; // get the existing pixel
                                    uint rb = ((0x100 - alpha) * (colora & 0x00FF00FF)) + (alpha * (pixel & 0x00FF00FF));
                                    uint g = ((0x100 - alpha) * (colora & 0x0000FF00)) + (alpha * (pixel & 0x0000FF00));
                                    m_ScanLinePixels[(i & 0x1ff)] = 0xFF000000 | (((rb & 0xFF00FF00) + (g & 0x00FF0000)) >> 8);
                                }
                            }
                            if (((i - x) & 7) == 7)
                                baseSprite += baseInc;
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
