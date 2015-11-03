using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace YCPUXNA.Display
{
    class Curses
    {
        private Texture2D m_Texture;
        private byte[] m_CharBuffer;
        private int m_Width, m_Height;

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

            Random rand = new Random();
            rand.NextBytes(m_CharBuffer);
        }

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
                    spriteBatch.Draw(m_Texture, new Rectangle(x * 8, y * 8, 8, 8), new Rectangle((ch % 16) * 8, (ch / 16) * 8, 8, 8), Color.White);
                }
            }
        }
    }
}
