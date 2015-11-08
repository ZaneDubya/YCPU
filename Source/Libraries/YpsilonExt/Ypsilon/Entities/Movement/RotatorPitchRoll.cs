using Microsoft.Xna.Framework;
using Ypsilon.Entities.Defines;

namespace Ypsilon.Entities.Movement
{
    /// <summary>
    /// A rotation handler that rotates an object using pitch and roll
    /// </summary>
    class RotatorPitchRoll : ARotator
    {
        private Quaternion m_RotationQ = Quaternion.Identity;

        public override Vector3 Forward
        {
            get
            {
                Vector3 rotatedVector = Vector3.Transform(new Vector3(0, 1, 0), m_RotationQ);
                return rotatedVector;
            }
        }

        public override Matrix RotationMatrix
        {
            get
            {
                return Matrix.CreateFromQuaternion(m_RotationQ);
            }
        }

        RotatorPitchRoll(ShipDefinition definition)
            : base(definition)
        {

        }

        public override void Rotate(float pitch, float roll, double frameSeconds)
        {
            Quaternion zaxisRotation = Quaternion.CreateFromAxisAngle(new Vector3(-1, 0, 0), pitch);
            Quaternion xaxisRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, -1, 0), roll);
            m_RotationQ *= zaxisRotation * xaxisRotation;
        }
    }
}
