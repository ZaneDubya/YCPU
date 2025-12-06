using Microsoft.Xna.Framework.Graphics;
using Ypsilon.Emulation;

namespace Ypsilon.Providers {
    public class YTexture : ITexture {
        public Texture2D Texture { get; }

        public YTexture(int busIndex, Texture2D texture) {
            DeviceBusIndex = busIndex;
            Texture = texture;
        }

        public int DeviceBusIndex { get; set; }

        public int Height {
            get {
                if (Texture == null)
                    return 0;
                return Texture.Height;
            }
        }

        public int Width {
            get {
                if (Texture == null)
                    return 0;
                return Texture.Width;
            }
        }
    }
}