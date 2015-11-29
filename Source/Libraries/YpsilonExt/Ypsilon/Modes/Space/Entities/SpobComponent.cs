using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Entities;
using Ypsilon.Modes.Space.Input;
using Ypsilon.Modes.Space.Resources;

namespace Ypsilon.Modes.Space.Entities
{
    class SpobComponent : ASpaceComponent
    {
        private Vector3[] m_ModelVertices;

        public SpobComponent()
        {

        }

        protected override void OnInitialize(AEntity entity)
        {
            Spob spob = entity as Spob;

            DrawSize = spob.Size;
            m_ModelVertices = Vertices.GetSpobVertices(spob.VertexRandomizationFactor);
        }

        public override void Update(AEntity entity, float frameSeconds)
        {
            Spob spob = entity as Spob;
            spob.Rotator.Rotation = spob.Rotator.Rotation + frameSeconds / spob.RotationPeriod;
        }

        public override void Draw(AEntity entity, VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            Spob spob = entity as Spob;

            Vector3 translation = (spob.Position - worldSpaceCenter).ToVector3();
            DrawMatrix = Matrix.CreateScale(DrawSize) * CreateWorldMatrix(spob, translation);
            DrawVertices = m_ModelVertices;
            DrawColor = spob.Color;

            base.Draw(entity, renderer, worldSpaceCenter, mouseOverList);
        }

        private Matrix CreateWorldMatrix(Spob spob, Vector3 translation)
        {
            Matrix rotMatrix = spob.Rotator.RotationMatrix;

            Vector3 forward = rotMatrix.Forward;
            forward.Normalize();

            Vector3 up = rotMatrix.Up;
            up.Normalize();

            return Matrix.CreateWorld(translation, forward, up);
        }
    }
}
