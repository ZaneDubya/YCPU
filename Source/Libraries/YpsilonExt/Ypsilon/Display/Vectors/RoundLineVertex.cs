#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ypsilon.Display.Vectors
{
    // A vertex type for drawing RoundLines, including an instance index
    struct RoundLineVertex : IVertexType
    {
        public Vector3 pos;
        public Vector2 rhoTheta;
        public Vector2 scaleTrans;
        public float index;

        public RoundLineVertex(Vector3 pos, Vector2 norm, Vector2 tex, float index)
        {
            this.pos = pos;
            this.rhoTheta = norm;
            this.scaleTrans = tex;
            this.index = index;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.Normal, 0),
                new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(28, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
            );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
