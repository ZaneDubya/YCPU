using Ypsilon.Data;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A starship.
    /// </summary>
    class Ship : AEntity
    {
        public AShipDefinition Definition = new AShipDefinition();

        public float ResourceOre = 0f;

        public Ship(EntityManager manager, Serial serial)
            : base(manager, serial)
        {
            
        }
    }
}
