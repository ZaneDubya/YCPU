using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Ypsilon.Core.Graphics;
using Ypsilon.Emulation;

namespace Ypsilon.Providers {
    public class DisplayProvider : IDisplayProvider {
        private readonly List<GraphicsDataForDevice> m_Devices;
        private readonly SpriteBatchExtended m_SpriteBatch;

        public DisplayProvider(SpriteBatchExtended spriteBatch) {
            m_SpriteBatch = spriteBatch;
            m_Devices = new List<GraphicsDataForDevice>();
        }

        public ITexture RenderLEM(int busIndex, int chrsWidth, int chrsHeight, byte[] devicemem, uint[] chr, uint[] pal, bool selectPage1, bool doSprites) {
            int pxWidth = chrsWidth * 4;
            int pxHeight = chrsHeight * 8;
            GraphicsDataForDevice data = GetDataForDeviceIndex(busIndex);
            if (data == null || data.Width != pxWidth || data.Height != pxHeight) {
                if (data != null) {
                    m_Devices.Remove(data);
                }
                m_Devices.Add(data = new GraphicsDataForDevice(m_SpriteBatch, busIndex, pxWidth, pxHeight));
            }
            uint[] lemData = data.Data;
            Texture2D lemTexture = data.Texture;
            uint tileBase = (uint)(selectPage1 ? 0x0400 : 0x0000);
            for (int y = 0; y < chrsHeight; y += 1) {
                for (int x = 0; x < chrsWidth; x += 1) {
                    byte data0 = devicemem[tileBase++];
                    byte data1 = devicemem[tileBase++];
                    uint color0 = pal[data1 & 0x0f];
                    uint color1 = pal[(data1 & 0xf0) >> 4];
                    uint character = chr[data0 & 0x7F];
                    int offset = y * 8 * pxWidth + x * 4;
                    for (int iy = 0; iy < 8; iy++) {
                        for (int ix = 0; ix < 4; ix++) {
                            lemData[offset + iy * pxWidth + ix] = (character & 0x00000001) == 1 ? color1 : color0;
                            character >>= 1;
                        }
                    }
                }
            }
            if (doSprites) {
                const uint oamByte0 = 0x0E00;
                const uint oamByte1 = 0x0E10;
                const uint oamByte2 = 0x0E20;
                const uint oamByte3 = 0x0E30;
                for (int oam = 0; oam < 16; oam++) {
                    const int sprWidth = 8, sprHeight = 8;
                    int y = devicemem[oamByte0 + oam];
                    int attr = devicemem[oamByte2 + oam];
                    int x = devicemem[oamByte3 + oam];
                    bool highnibble = false;
                    bool hflip = (attr & 0x01) != 0;
                    bool vflip = (attr & 0x02) != 0;
                    for (int iy = 0; iy < sprHeight; iy++) {
                        int screeny = iy + y;

                        // Y clipping
                        if (screeny < 0 || screeny >= pxHeight)
                            continue;
                        int baseSprite = 0x8000 + devicemem[oamByte1 + oam] * 32 + (vflip ? (7 - iy) * (sprWidth / 2) : iy * (sprWidth / 2));
                        int xInc = 1;
                        if (hflip) {
                            baseSprite += sprWidth / 2 - 1;
                            xInc = -xInc;
                            highnibble = true;
                        }
                        for (int screenx = x; screenx < x + sprWidth; screenx++) {
                            // X clipping
                            if (screenx >= 0 && screenx < pxWidth) {
                                int sprdata = devicemem[baseSprite];
                                int color = highnibble ? sprdata >> 4 : sprdata & 0x0f;
                                //if (color != 0)
                                {
                                    lemData[screeny * pxWidth + screenx] = pal[color];
                                }
                            }
                            if (hflip) {
                                if (highnibble) {
                                    highnibble = false;
                                }
                                else {
                                    highnibble = true;
                                    baseSprite += xInc;
                                }
                            }
                            else {
                                if (highnibble) {
                                    highnibble = false;
                                    baseSprite += xInc;
                                }
                                else {
                                    highnibble = true;
                                }
                            }
                        }
                    }
                }
            }
            lemTexture.SetData(lemData);
            return new YTexture(busIndex, lemTexture);
        }

        private GraphicsDataForDevice GetDataForDeviceIndex(int busIndex) {
            foreach (GraphicsDataForDevice gdfd in m_Devices) {
                if (gdfd.DeviceBusIndex == busIndex)
                    return gdfd;
            }
            return null;
        }

        private class GraphicsDataForDevice {
            public readonly uint[] Data;
            public readonly int DeviceBusIndex;
            public readonly Texture2D Texture;
            public readonly int Width, Height;

            public GraphicsDataForDevice(SpriteBatchExtended sb, int busIndex, int width, int height) {
                DeviceBusIndex = busIndex;
                Width = width;
                Height = height;
                Texture = sb.NewTexture(Width, Height);
                Data = new uint[Width * Height];
            }
        }
    }
}