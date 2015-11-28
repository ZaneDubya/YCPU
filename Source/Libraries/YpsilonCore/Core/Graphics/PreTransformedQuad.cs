using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Core.Graphics
{
    public class PreTransformedQuad
    {
        public VertexPositionColorTexture[] Vertices;
        public static short[] Indices = { 2, 1, 0, 3, 1, 2 };

        public PreTransformedQuad(Vector3 position, Vector2 area, Color hue)
        {
            resize(position, area, new Vector4(0, 0, 1, 1), hue);
        }

        public PreTransformedQuad(Vector3 position, Vector2 area, Vector4 uv, Color hue)
        {
            resize(position, area, uv, hue);
        }

        void resize(Vector3 position, Vector2 area, Vector4 uv, Color hue)
        {
            Vector2 texelPixelOffset = new Vector2(); // new Vector2(0.5f / device.Viewport.Width, 0.5f / device.Viewport.Height);

            Vertices = new VertexPositionColorTexture[]{
                new VertexPositionColorTexture( position, hue, new Vector2(uv.X, uv.Y) + texelPixelOffset), // top left
                new VertexPositionColorTexture( position + new Vector3(area.X, 0, 0), hue, new Vector2(uv.Z, uv.Y) + texelPixelOffset), // top right
                new VertexPositionColorTexture( position + new Vector3(0, area.Y, 0), hue, new Vector2(uv.X, uv.W) + texelPixelOffset), // bottom left
                new VertexPositionColorTexture( position + new Vector3(area, 0), hue, new Vector2(uv.Z, uv.W) + texelPixelOffset), // bottom right
            };
        }
    }
}