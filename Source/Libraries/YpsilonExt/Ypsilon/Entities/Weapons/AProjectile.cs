using Microsoft.Xna.Framework;
using Ypsilon.Data;

namespace Ypsilon.Entities.Weapons
{
    class AProjectile : AEntity
    {
        public Vector3 Velocity { get; protected set; }

        public virtual Color Color { get { return Colors.Railscasts[11]; } }
        public virtual float ProjectileSpeed { get { return 120f; } }
        public virtual float ProjectileLength { get { return 6f; } }

        protected AWeapon FiredFrom { get; private set; }

        public AProjectile(AWeapon firedFrom)
        {
            FiredFrom = firedFrom;
            Ship ship = (firedFrom.Parent as Ship);
            Vector3 forward = ship.Rotator.Forward;

            Position = ship.Position + forward * ship.Definition.DisplaySize;
            Velocity = ship.Rotator.Forward * ProjectileSpeed + ship.Velocity;
        }
    }
}
