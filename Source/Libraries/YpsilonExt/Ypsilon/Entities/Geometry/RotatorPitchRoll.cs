using Microsoft.Xna.Framework;

namespace Ypsilon.Entities.Geometry
{
    /// <summary>
    /// A rotation handler that rotates an object using pitch and roll
    /// </summary>
    class RotatorPitchRoll : IRotator
    {
        private Quaternion m_RotationQ = Quaternion.Identity;

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

        public void Rotate(float pitch, float roll, double gameSeconds)
        {
            Quaternion zaxisRotation = Quaternion.CreateFromAxisAngle(new Vector3(-1, 0, 0), pitch);
            Quaternion xaxisRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, -1, 0), roll);
            m_RotationQ *= zaxisRotation * xaxisRotation;
        }
    }
}
