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
    public partial class RoundLine
    {
        private Vector2 p0; // Begin point of the line
        private Vector2 p1; // End point of the line
        private float rho; // Length of the line
        private float theta; // Angle of the line

        public Vector2 P0
        {
            get
            {
                return p0;
            }
            set
            {
                p0 = value;
                RecalcRhoTheta();
            }
        }
        public Vector2 P1
        {
            get
            {
                return p1;
            }
            set
            {
                p1 = value;
                RecalcRhoTheta();
            }
        }
        public float Rho { get { return rho; } }
        public float Theta { get { return theta; } }

        public RoundLine(Vector2 p0, Vector2 p1)
        {
            this.p0 = p0;
            this.p1 = p1;
            RecalcRhoTheta();
        }

        public RoundLine(float x0, float y0, float x1, float y1)
        {
            this.p0 = new Vector2(x0, y0);
            this.p1 = new Vector2(x1, y1);
            RecalcRhoTheta();
        }

        protected void RecalcRhoTheta()
        {
            Vector2 delta = P1 - P0;
            rho = delta.Length();
            theta = (float)Math.Atan2(delta.Y, delta.X);
        }
    };
}
