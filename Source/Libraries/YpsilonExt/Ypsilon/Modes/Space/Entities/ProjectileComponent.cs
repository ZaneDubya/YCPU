using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Entities;
using Ypsilon.Entities.Weapons;
using Ypsilon.Modes.Space.Input;
using System;

namespace Ypsilon.Modes.Space.Entities
{
    class ProjectileComponent : ASpaceComponent
    {
        private Vector3[] m_ModelVertices;

        public ProjectileComponent()
        {

        }

        protected override void OnInitialize(AEntity entity)
        {
            m_ModelVertices = new Vector3[2];
            m_ModelVertices[0] = Vector3.Zero;
            m_ModelVertices[1] = Vector3.UnitY * (entity as AProjectile).Velocity.Length();
        }

        public override void Update(AEntity entity, float frameSeconds)
        {
            entity.Position += (entity as AProjectile).Velocity;
        }

        public override void Draw(AEntity entity, VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            AProjectile projectile = entity as AProjectile;
            Vector3 translation = (projectile.Position - worldSpaceCenter).ToVector3();

            DrawMatrix = Matrix.CreateScale(DrawSize) * CreateWorldMatrix(projectile, translation);
            DrawVertices = m_ModelVertices;
            DrawColor = projectile.Color;

            base.Draw(entity, renderer, worldSpaceCenter, mouseOverList);
        }

        private Matrix CreateWorldMatrix(AProjectile projectile, Vector3 translation)
        {
            Vector3 forward = -Vector3.UnitZ;

            float dir = (float)(Math.Atan2(forward.Y, forward.X) / (2 * Math.PI));
            Matrix rot = Matrix.CreateRotationZ(dir);
            Vector3 up = rot.Up;

            return Matrix.CreateWorld(translation, forward, up);
        }
    }
}
