using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Core.Graphics {
    public struct VertexPositionTextureDataColor : IVertexType {
        public Vector3 Position;
        public Vector2 UV;
        public Vector4 Data;
        public Color Hue;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), // position
            new VertexElement(sizeof (float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0), // tex coord
            new VertexElement(sizeof (float) * 5, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1), // data
            new VertexElement(sizeof (float) * 9, VertexElementFormat.Color, VertexElementUsage.Color, 0) // color
            );

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public static int SizeInBytes => sizeof (float) * 10;

        public VertexPositionTextureDataColor(Vector3 position, Vector2 uv, Vector4 data, Color hue) {
            Position = position;
            UV = uv;
            Data = data;
            Hue = hue;
        }

        public static readonly VertexPositionTextureDataColor[] PolyBuffer = {
            new VertexPositionTextureDataColor(new Vector3(), new Vector2(0, 0), Vector4.Zero, Color.White),
            new VertexPositionTextureDataColor(new Vector3(), new Vector2(1, 0), Vector4.Zero, Color.White),
            new VertexPositionTextureDataColor(new Vector3(), new Vector2(0, 1), Vector4.Zero, Color.White),
            new VertexPositionTextureDataColor(new Vector3(), new Vector2(1, 1), Vector4.Zero, Color.White)
        };

        public static readonly VertexPositionTextureDataColor[] PolyBufferFlipped = {
            new VertexPositionTextureDataColor(new Vector3(), new Vector2(0, 0), Vector4.Zero, Color.White),
            new VertexPositionTextureDataColor(new Vector3(), new Vector2(0, 1), Vector4.Zero, Color.White),
            new VertexPositionTextureDataColor(new Vector3(), new Vector2(1, 0), Vector4.Zero, Color.White),
            new VertexPositionTextureDataColor(new Vector3(), new Vector2(1, 1), Vector4.Zero, Color.White)
        };

        public override string ToString() {
            return $"VPTDC: <{Position}> <{UV}>";
        }
    }
}