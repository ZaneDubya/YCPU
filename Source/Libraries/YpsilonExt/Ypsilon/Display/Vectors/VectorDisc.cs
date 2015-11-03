// RoundLine.cs
// By Michael D. Anderson
// Version 4.00, Feb 8 2011
// Microsoft Public License (Ms-PL)

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Ypsilon.Display.Vectors
{
    /// <summary>
    /// A "degenerate" RoundLine where both endpoints are equal
    /// </summary>
    public class VectorDisc : VectorRoundLine
    {
        public VectorDisc(Vector2 p) : base(p, p) { }
        public VectorDisc(float x, float y) : base(x, y, x, y) { }
        public Vector2 Pos
        {
            get
            {
                return P0;
            }
            set
            {
                P0 = value;
                P1 = value;
            }
        }
    };
}
