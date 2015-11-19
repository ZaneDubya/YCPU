using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Entities;
using Ypsilon.Modes.Space.Entities.Movement;
using Ypsilon.Modes.Space.Input;
using Ypsilon.Modes.Space.Resources;

namespace Ypsilon.Modes.Space.Entities
{
    class SpobSpceComponent : AEntitySpaceComponent
    {
        public new Spob Entity
        {
            get
            {
                return (Spob)base.Entity;
            }
        }

        private PlanetRotator m_Rotator;
        private Vector3[] m_ModelVertices;

        public SpobSpceComponent(Spob spob)
            : base(spob)
        {
            ViewSize = Entity.Definition.Size;
        }

        protected override void OnInitialize()
        {
            m_ModelVertices = Vertices.GetSpobVertices(Entity.Definition);
            m_Rotator = new PlanetRotator(Entity.Definition);
        }

        public override void Update(float frameSeconds)
        {
            m_Rotator.Rotation = m_Rotator.Rotation + frameSeconds / Entity.Definition.RotationPeriod;
        }

        public override void Draw(VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            Vector3 translation = (Position - worldSpaceCenter).ToVector3();
            DrawMatrix = CreateWorldMatrix(translation);
            DrawVertices = m_ModelVertices;
            DrawColor = Entity.Definition.Color;

            base.Draw(renderer, worldSpaceCenter, mouseOverList);
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
