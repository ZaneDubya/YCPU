using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ypsilon.Data;
using Ypsilon.Entities.Movement;
using Ypsilon.Graphics;

namespace Ypsilon.Entities
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
                return (Definition.DefaultSpeed / 100f) * Throttle;
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

        public Vector3 Velocity
        {
            get
            {
                return Speed * m_Rotator.Forward;
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

            m_Trail1 = new Particles.Trail(new Vector3(-0.7f, -0.7f, 0));
            m_Trail2 = new Particles.Trail(new Vector3(0.7f, -0.7f, 0));
        }

        public override void Update(float frameSeconds)
        {
            if (IsPlayerEntity)
            {
                float acceleration = 0.0f;
                float leftrightRotation = 0.0f;

                KeyboardState keys = Keyboard.GetState();
                if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W))
                    acceleration += 1f;
                if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S))
                    acceleration = -1f;
                if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.D))
                    leftrightRotation = -1f;
                if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.A))
                    leftrightRotation = 1f;

                m_Rotator.Rotate(0f, leftrightRotation, frameSeconds);
                Throttle = Throttle + acceleration * frameSeconds;
            }

            // move forward
            Vector3 offset = Velocity * frameSeconds;
            Position += offset;

            m_Trail1.Update(frameSeconds, offset);
            m_Trail2.Update(frameSeconds, offset);
        }

        public override void Draw(VectorRenderer renderer, Position3D worldSpaceCenter)
        {
            Vector3 translation = (Position - worldSpaceCenter).ToVector3();
            Matrix rotation = Matrix.CreateRotationZ((m_Rotator as ShipRotator2D).Rotation);
            Matrix world = CreateWorldMatrix(translation);

            m_Trail1.Draw(renderer, translation, rotation);
            m_Trail2.Draw(renderer, translation, rotation);

            Vector3[] verts = new Vector3[m_ModelVertices.Length];
            for (int i = 0; i < verts.Length; i++)
                verts[i] = Vector3.Transform(m_ModelVertices[i], world);

            Color color = IsPlayerEntity ? Color.White : Color.OrangeRed;
            renderer.DrawPolygon(verts, true, color, false);
            
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
