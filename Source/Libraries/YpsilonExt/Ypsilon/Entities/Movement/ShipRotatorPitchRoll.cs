using Microsoft.Xna.Framework;
using Ypsilon.Data;

namespace Ypsilon.Entities.Movement
{
    /// <summary>
    /// A rotation handler that rotates an object using pitch and roll
    /// </summary>
    class ShipRotatorPitchRoll
    {
        private Quaternion m_RotationQ = Quaternion.Identity;
        private AShipDefinition m_Definition;

        public Vector3 Forward
        {
            get
            {
                Vector3 rotatedVector = Vector3.Transform(new Vector3(0, 1, 0), m_RotationQ);
                return rotatedVector;
            }
        }

        public Matrix RotationMatrix
        {
            get
            {
                return Matrix.CreateFromQuaternion(m_RotationQ);
            }
        }

        public ShipRotatorPitchRoll(AShipDefinition definition)
        {
            m_Definition = definition;
        }

        public void Rotate(float pitch, float roll, double frameSeconds)
        {
            pitch = MathHelper.Clamp(pitch, -1, 1);
            roll = MathHelper.Clamp(roll, -1, 1);
            float pitchThisFrame = pitch * (float)((m_Definition.DefaultRotation / 360f) * MathHelper.TwoPi * frameSeconds);
            float rollThisFrame = roll * (float)((m_Definition.DefaultRotation / 360f) * MathHelper.TwoPi * frameSeconds);

            Quaternion zaxisRotation = Quaternion.CreateFromAxisAngle(new Vector3(-1, 0, 0), pitchThisFrame);
            Quaternion xaxisRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, -1, 0), rollThisFrame);
            m_RotationQ *= zaxisRotation * xaxisRotation;
        }
    }
}
