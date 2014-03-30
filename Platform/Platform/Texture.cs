using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace YCPU.Platform
{
    /// <summary>
    /// Wrapper for Texture 2D
    /// </summary>
    public class Texture
    {
        internal Texture2D m_Texture;

        public static Texture Create(int width, int height)
        {
            Texture t = new Texture();
            t.m_Texture = Platform.Support.Library.CreateTexture(width, height);
            return t;
        }

        internal static Texture CreateFromTexture(Texture2D texture)
        {
            Texture t = new Texture();
            t.m_Texture = texture;
            return t;
        }

        public void SetData(uint[] data)
        {
            m_Texture.SetData<uint>(data);
        }

        public int Height
        {
            get { return m_Texture.Height; }
        }

        public int Width
        {
            get { return m_Texture.Width; }
        }
    }
}
