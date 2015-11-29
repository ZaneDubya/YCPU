using Microsoft.Xna.Framework;

namespace Ypsilon.Entities.Weapons
{
    class AProjectile : AEntity
    {
        public Vector3 Velocity { get; protected set; }
        public Color Color { get { return Color.White; } }

        protected AWeapon FiredFrom { get; private set; }

        public AProjectile(AWeapon firedFrom)
        {
            FiredFrom = firedFrom;
            Position = firedFrom.Position;
            Velocity = (firedFrom.Parent as Ship).Rotator.Forward * 10f;
        }
    }
}
