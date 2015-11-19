using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Data;
using Ypsilon.Modes.Space.Entities.Movement;
using Ypsilon.Modes.Space.Entities.ShipActions;
using Ypsilon.Modes.Space.Input;
using Ypsilon.Modes.Space.Resources;
using Ypsilon.Entities;

namespace Ypsilon.Modes.Space.Entities
{
    class ShipSpaceComponent : AEntitySpaceComponent
    {
        public new Ship Entity
        {
            get
            {
                return (Ship)base.Entity;
            }
        }

        public AAction Action = null;

        private ShipRotator2D m_Rotator;
        private Vector3[] m_ModelVertices;
        private Particles.Trail m_Trail1, m_Trail2;

        public float Speed
        {
            get
            {
                return (Entity.Definition.DefaultSpeed / 10f) * Throttle;
            }
        }

        private float m_Throttle = 0.2f;
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
                return Speed * m_Rotator.Forward;
            }
        }

        public ShipSpaceComponent(Ship ship)
            : base(ship)
        {
            ViewSize = Entity.Definition.DisplaySize;
        }

        protected override void OnInitialize()
        {
            m_ModelVertices = Vertices.SimpleArrow;
            m_Rotator = new ShipRotator2D(Entity.Definition);

            m_Trail1 = new Particles.Trail(new Vector3(-0.7f, -0.7f, 0), ViewSize);
            m_Trail2 = new Particles.Trail(new Vector3(0.7f, -0.7f, 0), ViewSize);
        }

        public override void Update(float frameSeconds)
        {
            // move forward
            Vector3 offset = Velocity * frameSeconds;
            Position += offset;

            m_Trail1.Update(frameSeconds, offset);
            m_Trail2.Update(frameSeconds, offset);

            if (Action != null)
                Action.Update(frameSeconds);
        }

        public override void Draw(VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            Vector3 translation = (Position - worldSpaceCenter).ToVector3();

            DrawMatrix = CreateWorldMatrix(translation);
            DrawVertices = m_ModelVertices;
            DrawColor = Entity.IsPlayerEntity ? Color.White : Color.OrangeRed;

            base.Draw(renderer, worldSpaceCenter, mouseOverList);

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
