using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace YCPUXNA.Display
{
    class Curses
    {
        private Texture2D m_Texture;
        private byte[] m_CharBuffer; // buffer of screen contents
        private int m_Width, m_Height;

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
        public Curses(GraphicsDevice graphics, int width, int height)
        {
            m_Texture = Texture2D.FromStream(graphics, new FileStream(@"Content\BIOS.png", FileMode.Open));
            m_Width = width;
            m_Height = height;
            m_CharBuffer = new byte[m_Width * m_Height];
        }

        /// <summary>
        /// Clears the Curses display.
        /// </summary>
        public void Clear()
        {
            Array.Clear(m_CharBuffer, 0, m_CharBuffer.Length);
        }

        public void WriteString(int x, int y, string s)
        {
            int begin = y * m_Width + x;
            if (begin >= m_CharBuffer.Length)
                return;
            if (begin + s.Length >= m_CharBuffer.Length)
                s = s.Substring(0, m_CharBuffer.Length - begin);
            for (int i = 0; i < s.Length; i++)
            {
                m_CharBuffer[begin + i] = (byte)s[i];
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < m_Height; y++)
            {
                for (int x = 0; x < m_Width; x++)
                {
                    byte ch = m_CharBuffer[x + y * m_Width];
                    spriteBatch.Draw(m_Texture, new Rectangle(x * 8, y * 8, 8, 8), new Rectangle((ch % 16) * 8, (ch / 16) * 8, 8, 8), Color.LightGray);
                }
            }
        }
    }
}
