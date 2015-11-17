using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ypsilon.World.Crafting
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
