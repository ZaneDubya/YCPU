using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ypsilon.Entities.Defines;
using Ypsilon.Entities.Movement;
using Ypsilon.Graphics;

namespace Ypsilon.Entities
{
    class Ship
    {
        public Position3D Position = Position3D.Zero;
        public float Speed
        {
            get
            {
                return (Definition.DefaultSpeed / 100f) * (IsPlayerEntity ? 1f : .2f);
            }
        }

        public ShipDefinition Definition = new ShipDefinition();

        private ARotator m_Rotator;
        private Vector3[] m_ModelVertices;
        private Particles.Trail m_Trail1, m_Trail2;

        public bool IsPlayerEntity = false;

        public Ship()
        {
            m_ModelVertices = ShipVertices.SimpleArrow;
            m_Rotator = new Rotator2D(Definition);
            m_Trail1 = new Particles.Trail(new Vector3(-0.7f, -0.7f, 0));
            m_Trail2 = new Particles.Trail(new Vector3(0.7f, -0.7f, 0));
        }

        public void Update(double frameSeconds)
        {
            if (IsPlayerEntity)
            {
                float updownRotation = 0.0f;
                float leftrightRotation = 0.0f;

                KeyboardState keys = Keyboard.GetState();
                if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W))
                    updownRotation = 1f;
                if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S))
                    updownRotation = -1f;
                if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.D))
                    leftrightRotation = -1f;
                if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.A))
                    leftrightRotation = 1f;

                m_Rotator.Rotate(updownRotation, leftrightRotation, frameSeconds);
            }

            // move forward
            Vector3 offset = Speed * m_Rotator.Forward * (float)frameSeconds;
            Position += offset;

            m_Trail1.Update(frameSeconds, offset);
            m_Trail2.Update(frameSeconds, offset);
        }

        public void Draw(double frameSeconds, VectorRenderer renderer, Position3D worldSpaceCenter)
        {
            Vector3 translation = (Position - worldSpaceCenter).ToVector3();
            Matrix rotation = Matrix.CreateRotationZ((m_Rotator as Rotator2D).Rotation);
            Matrix world = CreateWorldMatrix(translation);

            m_Trail1.Draw(renderer, translation, rotation);
            m_Trail2.Draw(renderer, translation, rotation);

            Vector3[] verts = new Vector3[m_ModelVertices.Length];
            for (int i = 0; i < verts.Length; i++)
                verts[i] = Vector3.Transform(m_ModelVertices[i], world);
            renderer.DrawPolygon(verts, true, Color.White, false);
            
        }

        private Matrix CreateWorldMatrix(Vector3 translation)
        {
            Matrix rotMatrix = m_Rotator.RotationMatrix; // Matrix.CreateFromQuaternion(RotationQ);

            Vector3 forward = rotMatrix.Forward;
            forward.Normalize();

            Vector3 up = rotMatrix.Up;
            up.Normalize();

            return Matrix.CreateWorld(translation, forward, up);
        }
    }
}
