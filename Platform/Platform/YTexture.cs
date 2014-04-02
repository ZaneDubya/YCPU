using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YCPU.Platform
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

        public void DrawLEM(IRenderer renderer, int x, int y, ushort word)
        {
            int character = word & 0x007F;
            renderer.GUIDrawSprite(this,
                new Rectangle(x * 8, y * 16, 8, 16),
                new Rectangle((character % 32) * 4, (character / 32) * 8, 4, 8),
                shader: Platform.Graphics.Shader.LEM1802,
                Palette0: (word & 0x0F00) >> 8,
                Palette1: (word & 0xF000) >> 12);
        }
    }
}
