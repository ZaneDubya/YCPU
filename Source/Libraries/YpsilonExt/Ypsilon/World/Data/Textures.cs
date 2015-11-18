using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.World.Data
{
    class Textures
    {
        public static Texture2D Pixel;

        public static void Initialize(GraphicsDevice graphics)
        {
            Pixel = new Texture2D(graphics, 1, 1);
            Pixel.SetData<Color>(new Color[1] { Color.White });
        }
    }
}
