using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ypsilon.Entities.Geometry
{
    /// <summary>
    /// A rotation handler that rotates an object using yaw only, with roll as a graphical extra.
    /// </summary>
    class Rotator2D : IRotator
    {
        private Vector2 m_Rotation; // x is roll, y is yaw.

        public Vector3 Forward
        {
            get
            {
                return new Vector3(-(float)Math.Sin(m_Rotation.Y), (float)Math.Cos(m_Rotation.Y), 0);
            }
        }

        public Matrix RotationMatrix
        {
            get
            {
                return Matrix.CreateRotationY(m_Rotation.X) * Matrix.CreateRotationZ(m_Rotation.Y);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roll"></param>
        /// <param name="yaw"></param>
        /// <param name="gameSeconds"></param>
        public void Rotate(float ignored, float yaw, double gameSeconds)
        {
            m_Rotation += new Vector2(roll % MathHelper.TwoPi, yaw % MathHelper.TwoPi);

            if (m_Rotation.X < 0)
                m_Rotation.X += MathHelper.TwoPi;
            if (m_Rotation.X >= MathHelper.TwoPi)
                m_Rotation.X %= MathHelper.TwoPi;

            if (m_Rotation.Y < 0)
                m_Rotation.Y += MathHelper.TwoPi;
            if (m_Rotation.Y >= MathHelper.TwoPi)
                m_Rotation.Y %= MathHelper.TwoPi;
        }
    }
}
