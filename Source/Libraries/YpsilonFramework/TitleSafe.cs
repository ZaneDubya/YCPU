using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ypsilon.Core.Graphics;

namespace Ypsilon
{
    public static class TitleSafe
    {
        public static void DrawTitleSafeAreas(GraphicsDevice graphics, VectorRenderer vectors,
            Color notActionSafeColor, Color notTitleSafeColor)
        {
            int width = graphics.PresentationParameters.BackBufferWidth;
            int height = graphics.PresentationParameters.BackBufferHeight;
            int dx = (int)(width * 0.05);
            int dy = (int)(height * 0.05);
            // const int z = 0;

            /*(vectors.DrawLines(new[] {
                new Vector3(dx, dy, z),
                new Vector3(dx + width - 2 * dx, dy, z),
                new Vector3(dx + width - 2 * dx, dy + height - 2 * dy, z),
                new Vector3(dx, dy + height - 2 * dy, z) }, Matrix.Identity, notActionSafeColor, true);

            vectors.DrawLines(new[] {
                new Vector3(dx * 2, dy * 2, z),
                new Vector3(dx * 2 + width - 4 * dx, dy * 2, z),
                new Vector3(dx * 2 + width - 4 * dx, dy * 2 + height - 4 * dy, z),
                new Vector3(dx * 2, dy* 2 + height - 4 * dy, z) }, Matrix.Identity, notTitleSafeColor, true);*/
        }
    }
}
