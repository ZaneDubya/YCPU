using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ypsilon.Data;
using Ypsilon.Entities.Movement;
using Ypsilon.Graphics;

namespace Ypsilon.Entities
{
    class StellarObject
    {
        public Position3D Position = new Position3D(10, 10, 0);
        public PlanetDefinition Definition = new PlanetDefinition();
        private Vector3[] m_ModelVertices;

        public StellarObject()
        {
            m_ModelVertices = Vertices.Planet;
        }

        public void Update(float frameSeconds)
        {

        }

        public void Draw(VectorRenderer renderer, Position3D worldSpaceCenter)
        {
            Vector3 translation = (Position - worldSpaceCenter).ToVector3();
            Matrix world = CreateWorldMatrix(translation);

            Vector3[] verts = new Vector3[m_ModelVertices.Length];
            for (int i = 0; i < verts.Length; i++)
                verts[i] = Vector3.Transform(m_ModelVertices[i] * Definition.Size, world);

            Color color = Color.Blue;
            renderer.DrawPolygon(verts, true, color, false);
        }

        private Matrix CreateWorldMatrix(Vector3 translation)
        {
            Matrix rotMatrix = Matrix.Identity; // m_Rotator.RotationMatrix;

            Vector3 forward = rotMatrix.Forward;
            forward.Normalize();

            Vector3 up = rotMatrix.Up;
            up.Normalize();

            return Matrix.CreateWorld(translation, forward, up);
        }
    }
}
