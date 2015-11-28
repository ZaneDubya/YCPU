using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Entities;
using Ypsilon.Modes.Space.Entities.Movement;
using Ypsilon.Modes.Space.Input;
using Ypsilon.Modes.Space.Resources;

namespace Ypsilon.Modes.Space.Entities
{
    class SpobComponent : ASpaceComponent
    {
        private PlanetRotator m_Rotator;
        private Vector3[] m_ModelVertices;

        public SpobComponent()
        {

        }

        protected override void OnInitialize(AEntity entity)
        {
            Spob spob = entity as Spob;

            ViewSize = spob.Size;
            m_ModelVertices = Vertices.GetSpobVertices(spob.VertexRandomizationFactor);
            m_Rotator = new PlanetRotator();
        }

        public override void Update(AEntity entity, float frameSeconds)
        {
            Spob spob = entity as Spob;
            m_Rotator.Rotation = m_Rotator.Rotation + frameSeconds / spob.RotationPeriod;
        }

        public override void Draw(AEntity entity, VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            Spob spob = entity as Spob;

            Vector3 translation = (Position - worldSpaceCenter).ToVector3();
            DrawMatrix = Matrix.CreateScale(ViewSize) * CreateWorldMatrix(translation);
            DrawVertices = m_ModelVertices;
            DrawColor = spob.Color;

            base.Draw(entity, renderer, worldSpaceCenter, mouseOverList);
        }

        private Matrix CreateWorldMatrix(Vector3 translation)
        {
            Matrix rotMatrix = m_Rotator.RotationMatrix;

            Vector3 forward = rotMatrix.Forward;
            forward.Normalize();

            Vector3 up = rotMatrix.Up;
            up.Normalize();

            return Matrix.CreateWorld(translation, forward, up);
        }
    }
}
