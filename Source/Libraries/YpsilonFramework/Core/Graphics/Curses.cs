﻿using System;
﻿using System.IO;
﻿using Microsoft.Xna.Framework;
﻿using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Core.Graphics
{
    public class Curses
    {
        private readonly Texture2D m_Texture;
        private readonly byte[] m_CharBuffer; // buffer of screen contents
        private readonly bool m_AdditionalHorizSpacingPixel;

        public readonly int CharsWide;
        public readonly int CharsHigh;
        public readonly int OneCharWidth;
        public readonly int OneCharHeight;

        public int DisplayWidth => CharsWide * (OneCharWidth + (m_AdditionalHorizSpacingPixel ? 1 : 0));

        public int DisplayHeight => CharsHigh * OneCharHeight;

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
        public Curses(GraphicsDevice graphics, int width, int height, string font, bool useHorizSpacingPixel)
        {
            CharsWide = width;
            CharsHigh = height;
            m_CharBuffer = new byte[CharsWide * CharsHigh];

            using (FileStream fs = new FileStream(font, FileMode.Open))
            {
                m_Texture = Texture2D.FromStream(graphics, fs);
            }
            OneCharWidth = m_Texture.Width / 16;
            OneCharHeight = m_Texture.Height / 16;
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
            int begin = y * CharsWide + x;
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
                        bool trailingSpace = (c == ' ');
                        int length = (i - last) + (trailingSpace ? 0 : 1);
                        // string word = s.Substring(last, length);

                        if (x0 + length > x + width)
                        {
                            x0 = x;
                            y0 += 1;
                            if (y0 >= CharsHigh)
                                return;
                        }

                        int begin = y0 * CharsWide + x0;
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

            m_CharBuffer[x + y * CharsWide] = deco[0];
            m_CharBuffer[(x + w) + y * CharsWide] = deco[2];
            m_CharBuffer[x + (y + h) * CharsWide] = deco[6];
            m_CharBuffer[(x + w) + (y + h) * CharsWide] = deco[8];

            for (int i = 1; i < w; i++)
            {
                m_CharBuffer[(x + i) + y * CharsWide] = deco[1];
                m_CharBuffer[(x + i) + (y + h) * CharsWide] = deco[7];
            }

            for (int i = 1; i < h; i++)
            {
                m_CharBuffer[(x) + (y + i) * CharsWide] = deco[3];
                m_CharBuffer[(x + w) + (y + i) * CharsWide] = deco[5];
            }
        }

        public void Render(SpriteBatchExtended spriteBatch, Vector2 offset)
        {
            const float sixteenth = (1 / 16f);
            float horizSpacer = m_AdditionalHorizSpacingPixel ? 1f : 0f;

            for (int y = 0; y < CharsHigh; y++)
            {
                for (int x = 0; x < CharsWide; x++)
                {
                    byte ch = m_CharBuffer[x + y * CharsWide];
                    if (ch == 0)
                        continue;
                    float u = (ch % 16) * sixteenth;
                    float v = (ch / 16) * sixteenth;

                    spriteBatch.DrawSprite(m_Texture,
                        new Vector3(offset.X + x * (OneCharWidth + horizSpacer), offset.Y + y * OneCharHeight, 0),
                        new Vector2(OneCharWidth, OneCharHeight),
                        new Vector4(u, v, u + sixteenth, v + sixteenth),
                        Color.LightGray);
                }
            }
        }

        private readonly byte[] m_CurseDecorationDoubleLine = {
            0xC9, 0xCD, 0xBB,
            0xBA, 0x00, 0xBA,
            0xC8, 0xCD, 0xBC
        };

        private readonly byte[] m_CurseDecorationBlock = {
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