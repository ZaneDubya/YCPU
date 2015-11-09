using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ypsilon.Graphics;

namespace Ypsilon.Entities.Particles
{
    class Trail
    {
        Vector3 m_Offset;
        List<Vector3> m_Vertices = new List<Vector3>();
        private float m_SecondsSinceLastVector = 0;
        private const float c_SecondsBetweenVectors = 0.2f;
        bool m_CreateNewVector = false;

        public Trail(Vector3 offset)
        {
            m_Offset = offset;
        }

        public void Update(double frameSeconds, Vector3 offset)
        {
            for (int i = 0; i < m_Vertices.Count - 1; i++)
            {
                m_Vertices[i] -= offset;
            }

            m_SecondsSinceLastVector += (float)frameSeconds;
            if (m_SecondsSinceLastVector >= c_SecondsBetweenVectors)
            {
                m_SecondsSinceLastVector -= c_SecondsBetweenVectors;
                m_CreateNewVector = true;
            }
        }

        public void Draw(VectorRenderer renderer, Matrix worldMatrix)
        {
            if (m_Vertices.Count < 10 || m_CreateNewVector)
            {
                m_CreateNewVector = false;
                while (m_Vertices.Count >= 10)
                    m_Vertices.RemoveAt(0);
                Vector3 v = Vector3.Transform(m_Offset, worldMatrix);
                m_Vertices.Add(v);
            }
            renderer.DrawPolygon(m_Vertices.ToArray(), false, Color.Gray, false);
        }
    }
}
