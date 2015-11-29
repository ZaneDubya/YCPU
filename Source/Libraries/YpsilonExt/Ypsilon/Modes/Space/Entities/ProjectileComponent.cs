using Microsoft.Xna.Framework;
using System;
using Ypsilon.Core.Graphics;
using Ypsilon.Entities;
using Ypsilon.Entities.Weapons;
using Ypsilon.Modes.Space.Input;

namespace Ypsilon.Modes.Space.Entities
{
    class ProjectileComponent : ASpaceComponent
    {
        private Vector3[] m_ModelVertices;
        private float m_TravelDistance = 0f;

        public ProjectileComponent()
        {

        }

        protected override void OnInitialize(World world, AEntity entity)
        {
            m_ModelVertices = new Vector3[2];
            m_ModelVertices[0] = Vector3.Zero;
            m_ModelVertices[1] = Vector3.UnitY * entity.Size;
        }

        public override void Update(World world, AEntity entity, float frameSeconds)
        {
            if (m_TravelDistance >= (entity as AProjectile).FiredFrom.ProjectileRange)
            {
                entity.Dispose();
            }
            else
            {
                Vector3 travel = (entity as AProjectile).Velocity * frameSeconds;
                entity.Position += travel;
                m_TravelDistance += travel.Length();
            }
        }

        public override void Draw(AEntity entity, VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            AProjectile projectile = entity as AProjectile;
            Vector3 translation = (projectile.Position - worldSpaceCenter).ToVector3();

            DrawMatrix = Matrix.CreateScale(DrawSize) * CreateWorldMatrix(projectile, translation);
            DrawVertices = m_ModelVertices;
            DrawColor = projectile.FiredFrom.Color;

            base.Draw(entity, renderer, worldSpaceCenter, mouseOverList);
        }

        private Matrix CreateWorldMatrix(AProjectile projectile, Vector3 translation)
        {
            float dir = (float)(Math.Atan2(projectile.Velocity.Y, projectile.Velocity.X));
            Matrix rotation = Matrix.CreateRotationZ(dir + MathHelper.PiOver2);
            return Matrix.CreateWorld(translation, rotation.Forward, rotation.Up);
        }
    }
}
