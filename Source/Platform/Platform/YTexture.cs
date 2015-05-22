using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Platform
{
    /// <summary>
    /// Wrapper for Texture2D
    /// </summary>
    public class YTexture
    {
        internal Texture2D m_Texture;

        public static YTexture Create(int width, int height)
        {
            YTexture t = new YTexture();
            t.m_Texture = Platform.Support.Library.CreateTexture(width, height);
            return t;
        }

        internal static YTexture CreateFromTexture(Texture2D texture)
        {
            YTexture t = new YTexture();
            t.m_Texture = texture;
            return t;
        }

        public void Dispose()
        {
            m_Texture.Dispose();
            m_Texture = null;
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
