using System;
using Microsoft.Xna.Framework;

namespace Ypsilon.Entities
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
    }
}
