using Microsoft.Xna.Framework;
using System;
using Ypsilon.World.Data;

namespace Ypsilon.World.Entities.Movement
{
    /// <summary>
    /// A rotation handler that rotates an object using yaw only, with roll as a graphical extra.
    /// </summary>
    class PlanetRotator
    {
        private ASpobDefinition m_Definition;
        private Vector3 m_AxialTilt;
        private float m_Rotation; // about the axis.

        /// <summary>
        /// Rotation about the axis, from 0.0 to 1.0f;
        /// </summary>
        public float Rotation
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                value %= 1.0f;
                if (value < 0.0f)
                    value += 1.0f;
                m_Rotation = value;
            }
        }

        public Matrix RotationMatrix
        {
            get
            {
                return Matrix.CreateRotationZ(m_Rotation * MathHelper.TwoPi);
            }
        }

        public PlanetRotator(ASpobDefinition definition)
        {
            m_Definition = definition;
        }
    }
}
