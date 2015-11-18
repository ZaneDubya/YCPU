using Microsoft.Xna.Framework;
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
        public Curses(GraphicsDevice graphics, int width, int height, string font)
        {
            ScreenWidth = width;
            ScreenHeight = height;
            m_CharBuffer = new byte[ScreenWidth * ScreenHeight];

            m_Texture = Texture2D.FromStream(graphics, new FileStream(font, FileMode.Open));
            CharWidth = m_Texture.Width / 16;
            CharHeight = m_Texture.Height / 16;
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

        public void Render(SpriteBatchExtended spriteBatch, Vector2 offset)
        {
            for (int y = 0; y < ScreenHeight; y++)
            {
                for (int x = 0; x < ScreenWidth; x++)
                {
                    byte ch = m_CharBuffer[x + y * ScreenWidth];
                    float sixteenth = (1 / 16f);
                    float u = (ch % 16) * sixteenth;
                    float v = (ch / 16) * sixteenth;

                    spriteBatch.DrawSprite(m_Texture,
                        new Vector3(offset.X + x * (CharWidth + 1), offset.Y + y * CharHeight, 0),
                        new Vector2(CharWidth, CharHeight),
                        new Vector4(u, v, u + sixteenth, v + sixteenth),
                        Color.LightGray);
                }
            }
        }
    }
}
