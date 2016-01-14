﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Ypsilon.Core.Graphics
{
    public class Curses
    {
        private Texture2D m_Texture;
        private byte[] m_CharBuffer; // buffer of screen contents

        public int ScreenWidth, ScreenHeight;
        public int CharWidth, CharHeight;
        private bool m_AdditionalHorizSpacingPixel;

        public int DisplayWidth
        {
            get
            {
                return ScreenWidth * (CharWidth + (m_AdditionalHorizSpacingPixel ? 1 : 0));
            }
        }

        public int DisplayHeight
        {
            get
            {
                return ScreenHeight * CharHeight;
            }
        }

        /*public Color[] Colors = new Color[16] {
            new Color(0, 0, 0),
            new Color(168,222,242),
            new Color(0,159,246),
            new Color(0,85,134),
            new Color(25,37,51),
            new Color(142,216,29),
            new Color(7,145,27),
            new Color(39,73,79),
            new Color(248,232,98),
            new Color(251,132,1),
            new Color(174,98,6),
            new Color(75,61,41),
            new Color(243,95,134),
            new Color(209,0,28),
            new Color(255,255,255),
            new Color(158,158,158)
        };*/

        /// <summary>
        /// Creates a Curses display. Width and Height are in 8x8 character increments.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Curses(GraphicsDevice graphics, int width, int height, string font, bool useHorizSpacingPixel)
        {
            ScreenWidth = width;
            ScreenHeight = height;
            m_CharBuffer = new byte[ScreenWidth * ScreenHeight];

            m_Texture = Texture2D.FromStream(graphics, new FileStream(font, FileMode.Open));
            CharWidth = m_Texture.Width / 16;
            CharHeight = m_Texture.Height / 16;
            m_AdditionalHorizSpacingPixel = useHorizSpacingPixel;
        }

        /// <summary>
        /// Clears the Curses display.
        /// </summary>
        public void Clear()
        {
            Array.Clear(m_CharBuffer, 0, m_CharBuffer.Length);
        }

        public void Randomize()
        {
            Random rand = new Random();
            for (int i = 0; i < m_CharBuffer.Length; i++)
                m_CharBuffer[i] = (byte)rand.Next(255);
        }

        public void WriteString(int x, int y, string s)
        {
            int begin = y * ScreenWidth + x;
            if (begin >= m_CharBuffer.Length)
                return;
            if (begin + s.Length >= m_CharBuffer.Length)
                s = s.Substring(0, m_CharBuffer.Length - begin);
            for (int i = 0; i < s.Length; i++)
            {
                m_CharBuffer[begin + i] = (byte)s[i];
            }
        }

        public void WriteLines(int x, int y, int width, string s, char delim)
        {
            int i = 0, last = 0;
            int x0 = x, y0 = y;

            while (i < s.Length)
            {
                char c = s[i];
                bool breakHere = false;
                if (c == delim)
                {
                    x0 = x;
                    y0 += 1;
                    int length = i - last;
                    if (length > 1)
                    {
                        // draw word?
                    }
                    else
                    {
                        last += 1;
                    }
                }
                else
                {
                    if ((c == ' ') || (c == '-') || (c == '.') || i == (s.Length - 1)) // break here
                    {
                        breakHere = true;
                    }

                    if (breakHere)
                    {
                        bool trailingSpace = (c == ' ') ? true : false;
                        int length = (i - last) + (trailingSpace ? 0 : 1);
                        string word = s.Substring(last, length);

                        if (x0 + length > x + width)
                        {
                            x0 = x;
                            y0 += 1;
                            if (y0 >= ScreenHeight)
                                return;
                        }

                        int begin = y0 * ScreenWidth + x0;
                        for (int j = 0; j < length; j++)
                        {
                            m_CharBuffer[begin + j] = (byte)s[last + j];
                        }
                        last = i + 1;
                        x0 += length + (trailingSpace ? 1 : 0);
                    }
                }

                i++;
            }
        }

        public void WriteBox(int x, int y, int w, int h, CurseDecoration d)
        {
            byte[] deco;
            switch (d)
            {
                case CurseDecoration.DoubleLine:
                    deco = m_CurseDecorationDoubleLine;
                    break;
                case CurseDecoration.Block:
                    deco = m_CurseDecorationBlock;
                    break;
                default:
                    deco = m_CurseDecorationBlock;
                    break;
            }

            m_CharBuffer[x + y * ScreenWidth] = deco[0];
            m_CharBuffer[(x + w) + y * ScreenWidth] = deco[2];
            m_CharBuffer[x + (y + h) * ScreenWidth] = deco[6];
            m_CharBuffer[(x + w) + (y + h) * ScreenWidth] = deco[8];

            for (int i = 1; i < w; i++)
            {
                m_CharBuffer[(x + i) + y * ScreenWidth] = deco[1];
                m_CharBuffer[(x + i) + (y + h) * ScreenWidth] = deco[7];
            }

            for (int i = 1; i < h; i++)
            {
                m_CharBuffer[(x) + (y + i) * ScreenWidth] = deco[3];
                m_CharBuffer[(x + w) + (y + i) * ScreenWidth] = deco[5];
            }
        }

        public void Render(SpriteBatchExtended spriteBatch, Vector2 offset)
        {
            float sixteenth = (1 / 16f);
            float horizSpacer = m_AdditionalHorizSpacingPixel ? 1f : 0f;

            for (int y = 0; y < ScreenHeight; y++)
            {
                for (int x = 0; x < ScreenWidth; x++)
                {
                    byte ch = m_CharBuffer[x + y * ScreenWidth];
                    if (ch == 0)
                        continue;
                    float u = (ch % 16) * sixteenth;
                    float v = (ch / 16) * sixteenth;

                    spriteBatch.DrawSprite(m_Texture,
                        new Vector3(offset.X + x * (CharWidth + horizSpacer), offset.Y + y * CharHeight, 0),
                        new Vector2(CharWidth, CharHeight),
                        new Vector4(u, v, u + sixteenth, v + sixteenth),
                        Color.LightGray);
                }
            }
        }

        byte[] m_CurseDecorationDoubleLine = new byte[9] {
            0xC9, 0xCD, 0xBB,
            0xBA, 0x00, 0xBA,
            0xC8, 0xCD, 0xBC
        };

        byte[] m_CurseDecorationBlock = new byte[9] {
            0xDB, 0xDB, 0xDB,
            0xDB, 0x00, 0xDB,
            0xDB, 0xDB, 0xDB
        };

        public enum CurseDecoration
        {
            DoubleLine = 0,
            Block = 1
        }
    }
}