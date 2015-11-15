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
            new Vector3(-1, -1, 0f)
        };

        public static Vector3[] SelectionLeft = new Vector3[4] {
            new Vector3(-1f, 1.25f, 0f),
            new Vector3(-1.25f, 1.25f, 0f),
            new Vector3(-1.25f, -1.25f, 0f),
            new Vector3(-1f, -1.25f, 0f)
        };

        public static Vector3[] SelectionRight = new Vector3[4] {
            new Vector3(1f, 1.25f, 0f),
            new Vector3(1.25f, 1.25f, 0f),
            new Vector3(1.25f, -1.25f, 0f),
            new Vector3(1f, -1.25f, 0f)
        };

        private static Vector3[] m_Planet;
        public static Vector3[] Planet
        {
            get
            {
                int vectorCount = 16;
                if (m_Planet == null)
                {
                    m_Planet = new Vector3[vectorCount];
                    for (int i = 0; i < m_Planet.Length; i++)
                    {
                        m_Planet[i] = new Vector3(
                            (float)Math.Cos(MathHelper.TwoPi * ((float)i / vectorCount)),
                            (float)Math.Sin(MathHelper.TwoPi * ((float)i / vectorCount)),
                            0f);
                    }
                }
                return m_Planet;
            }
        }

        public static Vector3[] GenerateNewAsteroid(float vertexRandFactor)
        {
            Vector3[] asteroidVectors = new Vector3[Planet.Length];
            Array.Copy(Planet, asteroidVectors, Planet.Length);

            for (int i = 0; i < asteroidVectors.Length; i++)
            {
                asteroidVectors[i] *= (float)Utility.Random_GetNonpersistantDouble() * vertexRandFactor + (1.0f - (vertexRandFactor / 2.0f));
            }
            return asteroidVectors;
        }

        public static Vector3[] GetSpobVertices(ASpobDefinition definition)
        {
            if (definition.DoRandomizeVertexes)
            {
                return GenerateNewAsteroid(definition.VertexRandomizationFactor);
            }
            else
            {
                return Vertices.Planet;
            }
        }
    }
}
