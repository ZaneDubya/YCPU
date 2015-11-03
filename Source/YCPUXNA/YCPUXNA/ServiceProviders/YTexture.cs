using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon;
using Microsoft.Xna.Framework.Graphics;

namespace YCPUXNA.ServiceProviders
{
    public class YTexture : ITexture
    {
        public int Width
        {
            get
            {
                if (Texture == null)
                    return 0;
                return Texture.Width;
            }
        }

        public int Height
        {
            get
            {
                if (Texture == null)
                    return 0;
                return Texture.Height;
            }
        }

        public int DeviceBusIndex
        {
            get;
            set;
        }

        public Texture2D Texture
        {
            get;
            private set;
        }

        public YTexture(Texture2D texture)
        {
            DeviceBusIndex = -1;
            Texture = texture;
        }
    }
}
