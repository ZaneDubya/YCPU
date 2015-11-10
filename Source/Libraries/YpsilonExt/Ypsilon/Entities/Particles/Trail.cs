using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Ypsilon.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Entities.Particles
{
    class Trail
    {
        Vector3 m_Offset;
        VertexPositionColorTexture[] m_Vertices;
        private float m_SecondsSinceLastVector = 0;
        private const float c_SecondsBetweenVectors = 0.2f;
        private bool m_FirstDraw = true; // used to set initial position of all trails.

        public Trail(Vector3 offset)
        {
            m_Offset = offset;
            m_Vertices = new VertexPositionColorTexture[10];
            for (int i = 0; i < m_Vertices.Length; i++)
            {
                float a = i < 8 ? MathHelper.Lerp(0f, 1f, (i / 8f)) : 1;
                m_Vertices[i].Position = m_Offset;
                m_Vertices[i].Color = new Color(.5f * a, .5f * a, 1f * a, a);
            }
        }

        public void Update(double frameSeconds, Vector3 offset)
        {
            for (int i = 0; i < m_Vertices.Length - 1; i++)
            {
                m_Vertices[i].Position -= offset;
            }

            m_SecondsSinceLastVector += (float)frameSeconds;
            if (m_SecondsSinceLastVector >= c_SecondsBetweenVectors)
            {
                m_SecondsSinceLastVector -= c_SecondsBetweenVectors;
                for (int i = 1; i < m_Vertices.Length; i++)
                    m_Vertices[i - 1].Position = m_Vertices[i].Position;
            }
        }

        public void Draw(VectorRenderer renderer, Matrix world)
        {
            if (m_FirstDraw)
            {
                for (int i = 0; i < m_Vertices.Length; i++)
                    m_Vertices[i].Position = m_Offset; // Vector3.Transform(m_Offset, worldMatrix);
                m_FirstDraw = false;
            }
            else
            {
                m_Vertices[m_Vertices.Length - 1].Position = m_Offset;
            }

            Matrix world2 = new Matrix();
            world2 = Matrix.CreateTranslation(world.Translation);
            /*world2.Forward = -Vector3.UnitZ;
            world2.Up = Vector3.UnitY;
            world2.Right = Vector3.UnitX;
            world2.Translation = world.Translation;*/

            VertexPositionColorTexture[] verts = new VertexPositionColorTexture[m_Vertices.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = m_Vertices[i];
                verts[i].Position = Vector3.Transform(m_Vertices[i].Position, world2);
            }
            renderer.DrawPolygon(verts, false);
        }
    }
}
