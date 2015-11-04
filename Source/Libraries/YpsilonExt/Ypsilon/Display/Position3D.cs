using System;

namespace Ypsilon.Display
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

        public static Position3D operator + (Position3D a, Position3D b)
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
