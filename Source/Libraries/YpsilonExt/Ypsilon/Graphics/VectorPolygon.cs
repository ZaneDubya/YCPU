using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Graphics
{
    /// <summary>
    /// A series of points that may be drawn together to form a line.
    /// </summary>
    public class VectorPolygon
    {
        /// <summary>
        /// The raw set of points, in "model space".
        /// </summary>
        public readonly Vector3[] Points;

        public bool IsClosed;

        /// <summary>
        /// Constructs a new VectorPolygon object from the given points.
        /// </summary>
        /// <param name="points">The raw set of points.</param>
        public VectorPolygon(Vector3[] points, bool isClosed)
        {
            Points = points;
            IsClosed = isClosed;
        }
    }
}
