using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;

namespace Ypsilon.Modes.Space.Particles
{
    class Trail
    {
        private Vector3 m_Offset; // from owning ship object.
        private float m_ShipSize;

        private VertexPositionNormalTextureData[] m_Vertices;
        private float m_SecondsSinceLastVector = 0;
        private const float c_SecondsBetweenVectors = 0.2f;
        private bool m_FirstDraw = true; // used to set initial position of all trails.

        public Trail(Vector3 offset, float shipSize)
        {
            m_Offset = offset;
            m_ShipSize = shipSize;

            float fadeout = 5;
            m_Vertices = new VertexPositionNormalTextureData[6];
            for (int i = 0; i < m_Vertices.Length; i++)
            {
                float a = i < fadeout ? MathHelper.Lerp(0f, 1f, (i / fadeout)) : 1;
                m_Vertices[i].Position = m_Offset;
                m_Vertices[i].Data = Data.Colors.Railscasts[0x03].ToVector4();
                m_Vertices[i].Data *= a;
            }
        }

        public void Update(float frameSeconds, Vector3 offset)
        {
            for (int i = 0; i < m_Vertices.Length - 1; i++)
            {
                m_Vertices[i].Position -= offset;
            }

            m_SecondsSinceLastVector += frameSeconds;
            if (m_SecondsSinceLastVector >= c_SecondsBetweenVectors)
            {
                m_SecondsSinceLastVector -= c_SecondsBetweenVectors;
                for (int i = 1; i < m_Vertices.Length; i++)
                    m_Vertices[i - 1].Position = m_Vertices[i].Position;
            }
        }

        public void Draw(VectorRenderer renderer, Vector3 translation, Matrix rotation)
        {
            if (m_FirstDraw)
            {
                for (int i = 0; i < m_Vertices.Length; i++)
                    m_Vertices[i].Position = m_Offset * m_ShipSize;
                m_FirstDraw = false;
            }
            else
            {
                m_Vertices[m_Vertices.Length - 1].Position = Vector3.Transform(m_Offset * m_ShipSize, rotation);
            }

            renderer.DrawPolygon(m_Vertices, translation, false);
        }
    }
}