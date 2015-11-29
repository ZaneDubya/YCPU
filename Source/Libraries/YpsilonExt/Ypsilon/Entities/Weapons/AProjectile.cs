using Microsoft.Xna.Framework;

namespace Ypsilon.Entities.Weapons
{
    class AProjectile : AEntity
    {
        public Vector3 Velocity { get; protected set; }

        public AWeapon FiredFrom { get; private set; }

        public override float Size { get { return FiredFrom.ProjectileSize; } }

        public override bool CollidesWithProjectiles { get { return false; } } // not essential, as these are tracked separately...

        public AProjectile(AWeapon firedFrom)
        {
            FiredFrom = firedFrom;
            Ship ship = (firedFrom.Parent as Ship);
            Vector3 forward = ship.Rotator.Forward;

            Position = ship.Position + forward * ship.Size;
            Velocity = ship.Rotator.Forward * FiredFrom.ProjectileSpeed + ship.Velocity;
        }
    }
}
