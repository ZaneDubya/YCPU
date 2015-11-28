using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Entities;
using Ypsilon.Modes.Space.Entities.Movement;
using Ypsilon.Modes.Space.Entities.ShipActions;
using Ypsilon.Modes.Space.Input;
using Ypsilon.Modes.Space.Resources;
using Ypsilon.Data;

namespace Ypsilon.Modes.Space.Entities
{
    class ShipComponent : ASpaceComponent
    {
        public AAction Action = null;

        private ShipRotator2D m_Rotator;
        private Vector3[] m_ModelVertices;
        private Particles.Trail m_Trail1, m_Trail2;

        private float m_Speed;

        private float m_Throttle;
        public float Throttle
        {
            get
            {
                return m_Throttle;
            }
            set
            {
                if (value < 0.0f)
                    m_Throttle = 0f;
                else if (value >= 1.0f)
                    m_Throttle = 1f;
                else
                    m_Throttle = value;
            }
        }

        public ShipRotator2D Rotator
        {
            get
            {
                return m_Rotator;
            }
        }

        public Vector3 Velocity
        {
            get
            {
                return m_Speed * m_Rotator.Forward;
            }
        }

        public ShipComponent()
        {
            
        }

        protected override void OnInitialize(AEntity entity)
        {
            Ship ship = entity as Ship;

            ViewSize = ship.Definition.DisplaySize;
            m_Rotator = new ShipRotator2D(ship.Definition);
            m_Throttle = ship.IsPlayerEntity ? 0.0f : 0.2f;
            m_ModelVertices = Vertices.SimpleArrow;
            m_Rotator = new ShipRotator2D(ship.Definition);

            m_Trail1 = new Particles.Trail(new Vector3(-0.7f, -0.7f, 0), ViewSize);
            m_Trail2 = new Particles.Trail(new Vector3(0.7f, -0.7f, 0), ViewSize);
        }

        public override void Update(AEntity entity, float frameSeconds)
        {
            Ship ship = entity as Ship;

            // set speed.
            m_Speed = (ship.Definition.DefaultSpeed / 10f) * Throttle;

            // move forward
            Vector3 offset = Velocity * frameSeconds;
            Position += offset;

            m_Trail1.Update(frameSeconds, offset);
            m_Trail2.Update(frameSeconds, offset);

            if (Action != null)
                Action.Update(frameSeconds);
        }

        public override void Draw(AEntity entity, VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            Vector3 translation = (Position - worldSpaceCenter).ToVector3();

            DrawMatrix = Matrix.CreateScale(ViewSize) * CreateWorldMatrix(translation);
            DrawVertices = m_ModelVertices;
            DrawColor = entity.IsPlayerEntity ? Colors.Railscasts[0x0C] : Colors.Railscasts[0x08];

            base.Draw(entity, renderer, worldSpaceCenter, mouseOverList);

            Matrix childRotation = Matrix.CreateRotationZ((m_Rotator as ShipRotator2D).Rotation);
            m_Trail1.Draw(renderer, translation, childRotation);
            m_Trail2.Draw(renderer, translation, childRotation);
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
