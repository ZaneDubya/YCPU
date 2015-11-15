using Microsoft.Xna.Framework;
using Ypsilon.Data;
using Ypsilon.Graphics;
using Ypsilon.Entities.Movement;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A space object.
    /// </summary>
    class Spob : AEntity
    {
        public ASpobDefinition Definition = new ASpobDefinition();

        private AShipRotator m_Rotator;
        private Vector3[] m_ModelVertices;

        public Spob(EntityManager manager, Serial serial)
            : base(manager, serial)
        {
            
        }

        protected override void OnInitialize()
        {
            m_ModelVertices = Vertices.GetSpobVertices(Definition);
            m_Rotator = new ShipRotator2D(
        }

        public override void Update(float frameSeconds)
        {

        }

        public override void Draw(VectorRenderer renderer, Position3D worldSpaceCenter)
        {
            Vector3 translation = (Position - worldSpaceCenter).ToVector3();
            Matrix world = CreateWorldMatrix(translation);

            Vector3[] verts = new Vector3[m_ModelVertices.Length];
            for (int i = 0; i < verts.Length; i++)
                verts[i] = Vector3.Transform(m_ModelVertices[i] * Definition.Size, world);
            
            renderer.DrawPolygon(verts, true, Definition.Color, false);
        }

        private Matrix CreateWorldMatrix(Vector3 translation)
        {
            Matrix rotMatrix = Matrix.Identity;

            Vector3 forward = rotMatrix.Forward;
            forward.Normalize();

            Vector3 up = rotMatrix.Up;
            up.Normalize();

            return Matrix.CreateWorld(translation, forward, up);
        }
    }
}
