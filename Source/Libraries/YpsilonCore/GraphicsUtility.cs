using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Ypsilon
{
    public static class GraphicsUtility
    {
        public static Matrix ProjectionMatrixUI
        {
            get { return Matrix.CreateOrthographicOffCenter(0f, 1280f, 720f, 0f, -1000f, 3000f); }
        }

        public static Matrix ProjectionMatrixScreen
        {
            get { return Matrix.CreateOrthographicOffCenter(0f, 1280f, 720f, 0f, -2000f, 2000f); }
        }

        public static Matrix CreateProjectionMatrixScreenOffset(GraphicsDevice graphics)
        {
            return Matrix.CreateOrthographicOffCenter(0, graphics.Viewport.Width, graphics.Viewport.Height, 0, Int16.MinValue, Int16.MaxValue);
        }

        public static Matrix CreateProjectionMatrixScreenCentered(GraphicsDevice graphics)
        {
            return Matrix.CreateOrthographicOffCenter(graphics.Viewport.Width / -2, graphics.Viewport.Width / 2, graphics.Viewport.Height / -2, graphics.Viewport.Height / 2, Int16.MinValue, Int16.MaxValue);
        } 
    }
}
