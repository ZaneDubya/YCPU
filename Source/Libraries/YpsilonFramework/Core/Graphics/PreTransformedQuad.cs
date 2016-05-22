using Microsoft.Xna.Framework;

namespace Ypsilon.Core.Graphics {
    public class PreTransformedQuad {
        public static short[] Indices = {2, 1, 0, 3, 1, 2};
        public VertexPositionTextureDataColor[] Vertices;

        public PreTransformedQuad(Vector3 position, Vector2 area, Vector4 uv, Color hue, Vector4 data) {
            Resize(position, area, uv, data, hue);
        }

        private void Resize(Vector3 position, Vector2 area, Vector4 uv, Vector4 data, Color hue) {
            Vector2 texelPixelOffset = new Vector2(); // new Vector2(0.5f / device.Viewport.Width, 0.5f / device.Viewport.Height);
            Vertices = new[] {
                new VertexPositionTextureDataColor(position, new Vector2(uv.X, uv.Y) + texelPixelOffset, data, hue), // top left
                new VertexPositionTextureDataColor(position + new Vector3(area.X, 0, 0), new Vector2(uv.Z, uv.Y) + texelPixelOffset, data, hue), // top right
                new VertexPositionTextureDataColor(position + new Vector3(0, area.Y, 0), new Vector2(uv.X, uv.W) + texelPixelOffset, data, hue), // bottom left
                new VertexPositionTextureDataColor(position + new Vector3(area, 0), new Vector2(uv.Z, uv.W) + texelPixelOffset, data, hue) // bottom right
            };
        }
    }
}