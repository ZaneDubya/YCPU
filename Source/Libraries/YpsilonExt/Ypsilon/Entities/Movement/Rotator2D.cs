using Microsoft.Xna.Framework;
using System;
using Ypsilon.Data;

namespace Ypsilon.Entities.Movement
{
    /// <summary>
    /// A rotation handler that rotates an object using yaw only, with roll as a graphical extra.
    /// </summary>
    class Rotator2D : ARotator
    {
        private float m_Roll, m_Yaw;

        public override Vector3 Forward
        {
            get
            {
                return new Vector3(-(float)Math.Sin(m_Yaw), (float)Math.Cos(m_Yaw), 0);
            }
        }

        public float Rotation
        {
            get
            {
                return m_Yaw;
            }
        }

        public override Matrix RotationMatrix
        {
            get
            {
                return Matrix.CreateRotationY(m_Roll) * Matrix.CreateRotationZ(m_Yaw);
            }
        }

        public Rotator2D(ShipDefinition definition)
            : base(definition)
        {

        }

        public override void Rotate(float updownRotation, float leftrightRotation, double frameSeconds)
        {
            // note - updownRotation is ignored.
            leftrightRotation = MathHelper.Clamp(leftrightRotation, -1, 1);

            float yawThisFrame = (float)((Definition.DefaultRotation / 360f) * MathHelper.TwoPi * frameSeconds);
            float rollThisFrame = yawThisFrame / 2;
            float rollMax = MathHelper.PiOver2 * (Definition.DefaultRotation / 360f);

            m_Yaw += leftrightRotation * yawThisFrame;

            if (leftrightRotation > 0)
            {
                // yaw to right, roll to right
                m_Roll -= leftrightRotation * yawThisFrame;
            }
            else if (leftrightRotation < 0)
            {
                m_Roll += Math.Abs(leftrightRotation * yawThisFrame);
            }
            else
            {
                if (Math.Abs(m_Roll) < rollThisFrame)
                    m_Roll = 0f;
                else if (m_Roll > 0)
                    m_Roll -= rollThisFrame;
                else if (m_Roll < 0)
                    m_Roll += rollThisFrame;
            }

            if (m_Roll < -rollMax)
                m_Roll = -rollMax;
            if (m_Roll > rollMax)
                m_Roll = rollMax;

            if (m_Yaw < 0)
                m_Yaw += MathHelper.TwoPi;
            if (m_Yaw >= MathHelper.TwoPi)
                m_Yaw %= MathHelper.TwoPi;
        }
    }
}
