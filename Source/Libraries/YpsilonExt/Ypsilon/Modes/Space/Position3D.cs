using System;
using Microsoft.Xna.Framework;
using Ypsilon.Core;

namespace Ypsilon.Modes.Space
{
    struct Position3D
    {
        public Double X, Y, Z;

        public Position3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Move(Vector3 vector, float speed)
        {
            vector.Normalize();
            vector *= speed;
            X += vector.X;
            Y += vector.Y;
            Z += vector.Z;
        }

        public bool Intersects(RectangleF rect, float radius)
        {
            Vector2 circleDistance;
            circleDistance.X = (float)Math.Abs(X - (rect.X + rect.Width / 2f));
            circleDistance.Y = (float)Math.Abs(Y - (rect.Y + rect.Height / 2f));

            if (circleDistance.X > (rect.Width / 2f + radius)) { return false; }
            if (circleDistance.Y > (rect.Height / 2f + radius)) { return false; }

            if (circleDistance.X <= (rect.Width / 2f)) { return true; }
            if (circleDistance.Y <= (rect.Height / 2f)) { return true; }

            float cornerDistX = (circleDistance.X - rect.Width / 2f);
            float cornerDistY = (circleDistance.Y - rect.Height / 2f);
            float cornerDistance_sq = cornerDistX * cornerDistX + cornerDistY * cornerDistY;

            return (cornerDistance_sq <= (radius * radius));
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float)X, (float)Y, (float)Z);
        }

        public static Position3D operator + (Position3D a, Position3D b)
        {
            return new Position3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Position3D operator +(Position3D a, Vector3 b)
        {
            return new Position3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Position3D operator - (Position3D a, Position3D b)
        {
            return new Position3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Position3D Zero
        {
            get
            {
                return new Position3D(0, 0, 0);

            }
        }

        internal static float Distance(Position3D a, Position3D b)
        {
            float distance = Vector3.Distance(a.ToVector3(), b.ToVector3());
            return distance;
        }
    }
}
