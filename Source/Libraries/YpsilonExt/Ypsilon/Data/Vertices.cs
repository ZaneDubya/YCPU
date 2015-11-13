using Microsoft.Xna.Framework;
using System;

namespace Ypsilon.Data
{
    class Vertices
    {
        public static Vector3[] SimpleArrow = new Vector3[4] {
            new Vector3(0, 1, 0),
            new Vector3(1, -1, 0f),
            new Vector3(0, -0.7f, 0.5f),
            new Vector3(-1, -1, 0f) };

        private static Vector3[] m_Planet;
        public static Vector3[] Planet
        {
            get
            {
                if (m_Planet == null)
                {
                    m_Planet = new Vector3[20];
                    for (int i = 0; i < m_Planet.Length; i++)
                    {
                        m_Planet[i] = new Vector3(
                            (float)Math.Cos(MathHelper.TwoPi * ((float)i / 20)),
                            (float)Math.Sin(MathHelper.TwoPi * ((float)i / 20)),
                            0f);
                    }
                }
                return m_Planet;
            }
        }
    }
}
