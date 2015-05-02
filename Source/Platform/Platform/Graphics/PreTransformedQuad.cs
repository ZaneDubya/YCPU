using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Platform.Graphics
{
    public class PreTransformedQuad
    {
        public VertexPositionTextureHueExtra[] Vertices;
        public static short[] Indices = { 2, 1, 0, 3, 1, 2 };

        public PreTransformedQuad(Vector3 position, Vector2 area, Color hue, Vector2 extra)
            :this(position, area, new Vector4(0,0,1,1), hue, extra)
        {

        }

        public PreTransformedQuad(Vector3 position, Vector2 area, Vector4 texture, Color hue, Vector2 extra)
        {
            Vector2 texelOffset = new Vector2();

            Vertices = new VertexPositionTextureHueExtra[]{
                new VertexPositionTextureHueExtra(position, new Vector2(texture.X, texture.Y) + texelOffset, hue, extra), // top left
                new VertexPositionTextureHueExtra(position + new Vector3(area.X, 0, 0), new Vector2(texture.Z, texture.Y) + texelOffset, hue, extra), // top right
                new VertexPositionTextureHueExtra(position + new Vector3(0, area.Y, 0), new Vector2(texture.X, texture.W) + texelOffset, hue, extra), // bottom left
                new VertexPositionTextureHueExtra(position + new Vector3(area, 0), new Vector2(texture.Z, texture.W) + texelOffset, hue, extra), // bottom right
            };
        }

        public PreTransformedQuad(Texture2D texture, Vector4 dest, Vector4 source, float z, Color hue, Vector2 extra)
        {
            Vector2 texelOffset = new Vector2();

            Vertices = new VertexPositionTextureHueExtra[]{
                new VertexPositionTextureHueExtra(new Vector3(dest.X, dest.Y, z), new Vector2(source.X, source.Y) + texelOffset, hue, extra), // top left
                new VertexPositionTextureHueExtra(new Vector3(dest.Z, dest.Y, z), new Vector2(source.Z, source.Y) + texelOffset, hue, extra), // top right
                new VertexPositionTextureHueExtra(new Vector3(dest.X, dest.W, z), new Vector2(source.X, source.W) + texelOffset, hue, extra), // bottom left
                new VertexPositionTextureHueExtra(new Vector3(dest.Z, dest.W, z), new Vector2(source.Z, source.W) + texelOffset, hue, extra), // bottom right
            };
        }
    }
}
