using System;
using System.Collections.Generic;
using Ypsilon.Entities;

namespace Ypsilon
{
    public class World
    {
        public float PlayerCredits = 10005;
        public Serial PlayerSerial = Serial.Null;

        public EntityList Entities
        {
            get;
            private set;
        }

        public EntityList Projectiles
        {
            get;
            private set;
        }

        public void Reset(bool clearPlayerEntity = true)
        {
            Entities.Reset(true);
            Projectiles.Reset(true);
        }

        public void Update(float totalSeconds, float frameSeconds)
        {
            Entities.Update(frameSeconds);
            Projectiles.Update(frameSeconds);
        }

        public World()
        {
            Entities = new EntityList(this);
            Projectiles = new EntityList(this);
        }

        /// <summary>
        /// Returns a list of Entities that are currently colliding with Projectiles.
        /// </summary>
        /// <param name="collisions"></param>
        public void GetProjectileCollisions(List<Tuple<AEntity, AEntity>> collisions)
        {
            foreach(AEntity projectile in Projectiles.All.Values)
            {
                foreach (AEntity entity in Entities.All.Values)
                {
                    if (!entity.CollidesWithProjectiles)
                        continue;

                    float collisionSize = entity.Size * 0.9f; // tweak this per entity?
                    float distance = (projectile.Position - entity.Position).ToVector3().Length();
                    if (distance < collisionSize)
                    {
                        collisions.Add(new Tuple<AEntity, AEntity>(entity, projectile));
                        break; // only one collision per projectile.
                    }
                }
            }
        }
    }
}
