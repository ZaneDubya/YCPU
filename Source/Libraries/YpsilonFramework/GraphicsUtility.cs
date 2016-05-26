using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon {
    public static class GraphicsUtility {
        public static Matrix ProjectionMatrixScreen => Matrix.CreateOrthographicOffCenter(0f, 1280f, 720f, 0f, -2000f, 2000f);
        public static Matrix ProjectionMatrixUI => Matrix.CreateOrthographicOffCenter(0f, 1280f, 720f, 0f, -1000f, 3000f);

        public static Matrix CreateProjectionMatrixScreenCentered(GraphicsDevice graphics) {
            return Matrix.CreateOrthographicOffCenter(graphics.Viewport.Width / -2, graphics.Viewport.Width / 2, graphics.Viewport.Height / -2, graphics.Viewport.Height / 2, short.MinValue, 0);//short.MaxValue);
        }

        public static Matrix CreateProjectionMatrixScreenOffset(GraphicsDevice graphics) {
            return Matrix.CreateOrthographicOffCenter(0, graphics.Viewport.Width, graphics.Viewport.Height, 0, short.MinValue, 0);//short.MaxValue);
        }
    }
}