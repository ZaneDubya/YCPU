using Microsoft.Xna.Framework;
using Ypsilon.Platform;
using Ypsilon.Platform.Graphics;

namespace Ypsilon
{
    public interface IRenderer
    {
        YTexture Palette_NES { set; get; }
        YTexture Palette_LEM { set; get; }

        void GUIDrawSprite(YTexture texture, Rectangle destinationRectangle,
            Rectangle? sourceRectangle = null, Color? color = null, YSpriteEffect effects = YSpriteEffect.None,
            Shader shader = Shader.Standard, int Palette0 = 0, int Palette1 = 0);
    }
}
