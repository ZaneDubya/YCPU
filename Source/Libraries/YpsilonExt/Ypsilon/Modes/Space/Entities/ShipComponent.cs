using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Data;
using Ypsilon.Entities;
using Ypsilon.Entities.Weapons;
using Ypsilon.Modes.Space.Entities.ShipActions;
using Ypsilon.Modes.Space.Input;
using Ypsilon.Modes.Space.Resources;

namespace Ypsilon.Modes.Space.Entities
{
    class ShipComponent : ASpaceComponent
    {
        public AAction Action = null;

        
        private Vector3[] m_ModelVertices;
        private Particles.Trail m_Trail1, m_Trail2;

        public ShipComponent()
        {
            
        }

        protected override void OnInitialize(World world, AEntity entity)
        {
            Ship ship = entity as Ship;

            DrawSize = ship.Size;
            ship.Throttle = (world.PlayerSerial == entity.Serial) ? 0.0f : 0.2f;
            m_ModelVertices = Vertices.SimpleArrow;

            m_Trail1 = new Particles.Trail(new Vector3(-0.7f, -0.7f, 0), DrawSize);
            m_Trail2 = new Particles.Trail(new Vector3(0.7f, -0.7f, 0), DrawSize);
        }

        public override void Update(World world, AEntity entity, float frameSeconds)
        {
            Ship ship = entity as Ship;

            // move forward
            Vector3 offset = ship.Velocity * frameSeconds;
            ship.Position += offset;

            m_Trail1.Update(frameSeconds, offset);
            m_Trail2.Update(frameSeconds, offset);

            if (Action != null)
                Action.Update(frameSeconds);

            if (IsFiring)
            {
                for (int i = 0; i < ship.Modules.Count; i++)
                {
                    AWeapon weapon = ship.Modules[i] as AWeapon;
                    if (weapon != null)
                    {
                        if (weapon.Fire())
                        {
                            // generate a projectile!
                            AProjectile proj = (AProjectile)world.Projectiles.AddEntity(typeof(AProjectile), weapon);
                            proj.SetComponent(new ProjectileComponent());
                        }
                    }
                }
            }
        }

        public override void Draw(AEntity entity, VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            Ship ship = entity as Ship;

            Vector3 translation = (ship.Position - worldSpaceCenter).ToVector3();

            DrawMatrix = Matrix.CreateScale(DrawSize) * CreateWorldMatrix(ship, translation);
            DrawVertices = m_ModelVertices;
            DrawColor = ship.Color;

            base.Draw(entity, renderer, worldSpaceCenter, mouseOverList);

            Matrix childRotation = Matrix.CreateRotationZ(ship.Rotator.Rotation);
            m_Trail1.Draw(renderer, translation, childRotation);
            m_Trail2.Draw(renderer, translation, childRotation);
        }

        private Matrix CreateWorldMatrix(Ship ship, Vector3 translation)
        {
            Matrix rotMatrix = ship.Rotator.RotationMatrix;

            Vector3 forward = rotMatrix.Forward;
            forward.Normalize();

            Vector3 up = rotMatrix.Up;
            up.Normalize();

            return Matrix.CreateWorld(translation, forward, up);
        }

        // ======================================================================
        // Input 
        // ======================================================================

        public bool IsFiring { get; set; }
    }
}
