using System;
using Ypsilon.Data;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A starship.
    /// </summary>
    class Ship : AEntity
    {
        public AShipDefinition Definition = new AShipDefinition();

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

        public Ship()
        {
            Inventory = new ItemList(this);
            Modules = new ModuleList(this);
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
    }
}
