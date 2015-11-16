using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ypsilon.World.Data;
using Ypsilon.World.Entities.Movement;
using Ypsilon.Core.Graphics;
using System;

namespace Ypsilon.World.Entities
{
    class Ship : AEntity
    {
        public AShipDefinition Definition = new AShipDefinition();

        private ShipRotator2D m_Rotator;
        private Vector3[] m_ModelVertices;
        private Particles.Trail m_Trail1, m_Trail2;

        public float Speed
        {
            get
            {
                return (Definition.DefaultSpeed / 10f) * Throttle;
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

        public override float ViewSize
        {
            get
            {
                return Definition.DisplaySize / 2f;
            }
        }

        public Ship(EntityManager manager, Serial serial)
            : base(manager, serial)
        {
            
        }

        protected override void OnInitialize()
        {
            m_ModelVertices = Vertices.SimpleArrow;
            m_Rotator = new ShipRotator2D(Definition);

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
        }

        public override void Draw(VectorRenderer renderer, Position3D worldSpaceCenter)
        {
            if (IsVisible)
            {
                Vector3 translation = (Position - worldSpaceCenter).ToVector3();
                Matrix rotation = Matrix.CreateRotationZ((m_Rotator as ShipRotator2D).Rotation);
                Matrix world = CreateWorldMatrix(translation);

                m_Trail1.Draw(renderer, translation, rotation);
                m_Trail2.Draw(renderer, translation, rotation);

                Vector3[] verts = new Vector3[m_ModelVertices.Length];
                for (int i = 0; i < verts.Length; i++)
                    verts[i] = Vector3.Transform(m_ModelVertices[i] * ViewSize, world);

                Color color = IsPlayerEntity ? Color.White : Color.OrangeRed;
                renderer.DrawPolygon(verts, true, color, false);

                if (IsSelected)
                {
                    Vector3[] selection = new Vector3[Vertices.SelectionLeft.Length];
                    for (int i = 0; i < selection.Length; i++)
                        selection[i] = Vector3.Transform(Vertices.SelectionLeft[i] * ViewSize, world);
                    renderer.DrawPolygon(selection, false, Color.Yellow, false);
                    for (int i = 0; i < selection.Length; i++)
                        selection[i] = Vector3.Transform(Vertices.SelectionRight[i] * ViewSize, world);
                    renderer.DrawPolygon(selection, false, Color.Yellow, false);
                }
            }
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
