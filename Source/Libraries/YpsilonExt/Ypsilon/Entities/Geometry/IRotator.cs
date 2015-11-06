using Microsoft.Xna.Framework;

namespace Ypsilon.Entities.Geometry
{
    interface IRotator
    {
        Vector3 Forward { get; }
        Matrix RotationMatrix { get; }
        void Rotate(float updownRotation, float leftrightRotation, double gameSeconds);
    }
}
