using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ypsilon.Entities.Geometry;

namespace Ypsilon.Entities
{
    class Entity
    {
        private IRotator m_Rotator;

        public Position3D Position = Position3D.Zero;

        public Vector3[] ModelVertices;

        public Vector3[] WorldVertices(Position3D worldSpaceCenter)
        {
            Matrix world = CreateWorldMatrix(worldSpaceCenter);
            Vector3[] verts = new Vector3[ModelVertices.Length];
            for (int i = 0; i < verts.Length;  i++)
            {
                verts[i] = Vector3.Transform(ModelVertices[i], world);
            }
            return verts;
        }

        public Entity()
        {
            SetUpVertices();
            m_Rotator = new Rotator2D();
        }

        public void Update(double gameSeconds)
        {
            float updownRotation = 0.0f;
            float leftrightRotation = 0.0f;

            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W))
                updownRotation = 0.025f;
            if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S))
                updownRotation = -0.025f;
            if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.D))
                leftrightRotation = -0.025f;
            if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.A))
                leftrightRotation = 0.025f;

            m_Rotator.Rotate(updownRotation, leftrightRotation, gameSeconds);

            // move forward
            float moveSpeed = 0.1f;
            Position += moveSpeed * m_Rotator.Forward;
        }

        private void SetUpVertices()
        {
            ModelVertices = new Vector3[4] {
                new Vector3(0, 1, 0),
                new Vector3(1, -1, 0),
                new Vector3(0, -0.5f, 0.33f),
                new Vector3(-1, -1, 0) };
        }

        private Matrix CreateWorldMatrix(Position3D worldSpaceCenter)
        {
            Matrix rotMatrix = m_Rotator.RotationMatrix; // Matrix.CreateFromQuaternion(RotationQ);

            Vector3 forward = rotMatrix.Forward;
            forward.Normalize();

            Vector3 up = rotMatrix.Up;
            up.Normalize();

            return Matrix.CreateWorld((Position - worldSpaceCenter).ToVector3(), forward, up);
        }
    }
}
