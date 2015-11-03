// RoundLine.cs
// By Michael D. Anderson
// Version 4.00, Feb 8 2011
// A class to efficiently draw thick lines with rounded ends.
// Microsoft Public License (Ms-PL)

#region Using Statements
using Microsoft.Xna.Framework;
using System;
#endregion

namespace Ypsilon.Display.Vectors
{
    /// <summary>
    /// Represents a single line segment.  Drawing is handled by the RoundLineManager class.
    /// </summary>
    public partial class VectorRoundLine
    {
        private Vector2 m_P0; // Begin point of the line
        private Vector2 m_P1; // End point of the line
        private float m_Length; // Length of the line
        private float m_Angle; // Angle of the line

        /// <summary>
        /// Start
        /// </summary>
        public Vector2 P0
        {
            get
            {
                return m_P0;
            }
            set
            {
                m_P0 = value;
                RecalcRhoTheta();
            }
        }

        /// <summary>
        /// End
        /// </summary>
        public Vector2 P1
        {
            get
            {
                return m_P1;
            }
            set
            {
                m_P1 = value;
                RecalcRhoTheta();
            }
        }
        public float Length { get { return m_Length; } }
        public float Angle { get { return m_Angle; } }

        public VectorRoundLine(Vector2 p0, Vector2 p1)
        {
            this.m_P0 = p0;
            this.m_P1 = p1;
            RecalcRhoTheta();
        }

        public VectorRoundLine(float x0, float y0, float x1, float y1)
        {
            this.m_P0 = new Vector2(x0, y0);
            this.m_P1 = new Vector2(x1, y1);
            RecalcRhoTheta();
        }

        /// <summary>
        /// Recalculate length and angle.
        /// </summary>
        protected void RecalcRhoTheta()
        {
            Vector2 delta = P1 - P0;
            m_Length = delta.Length();
            m_Angle = (float)Math.Atan2(delta.Y, delta.X);
        }
    };
}
