using Microsoft.Xna.Framework;
using System;
using Ypsilon.Data;
using Ypsilon.Entities.Collections;
using Ypsilon.Entities.Movement;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A starship.
    /// </summary>
    public class Ship : AEntity
    {
        public AShipDefinition Definition = new AShipDefinition();
        public virtual Color Color { get; set; }
        public override float Size { get { return Definition.Size; } }

        public ItemList Inventory
        {
            get;
            private set;
        }

        public ModuleList Modules
        {
            get;
            private set;
        }

        public float Throttle
        {
            get
            {
                return m_Throttle;
            }
            set
            {
                if (value < 0.0f)
                    m_Throttle = 0f;
                else if (value >= 1.0f)
                    m_Throttle = 1f;
                else
                    m_Throttle = value;
            }
        }

        public ShipRotator2D Rotator
        {
            get;
            private set;
        }

        public Vector3 Velocity
        {
            get
            {
                return (Definition.DefaultSpeed / 10f) * Throttle * Rotator.Forward;
            }
        }

        private float m_Throttle;

        public Ship()
        {
            Inventory = new ItemList(this);
            Modules = new ModuleList(this);
            Rotator = new ShipRotator2D(Definition);
        }

        public override void RemoveEntity(AEntity entity)
        {
            if (entity is AItem)
            {
                Inventory.RemoveItem(entity as AItem);
            }
            else if (entity is AModule)
            {
                Modules.RemoveModule(entity as AModule);
            }
            else
            {
                throw new Exception(string.Format("Can't remove entity of type {0}", entity.GetType().ToString()));
            }
        }

        public override void Update(World world, float frameSeconds)
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                AModule module = Modules[i];
                module.Update(world, frameSeconds);
            }

            base.Update(world, frameSeconds);
        }

    }
}
