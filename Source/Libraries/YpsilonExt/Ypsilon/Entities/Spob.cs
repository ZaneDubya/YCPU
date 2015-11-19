using Ypsilon.Data;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A space object: planet or asteroid.
    /// </summary>
    class Spob : AEntity
    {
        public ASpobDefinition Definition = new ASpobDefinition();

        

        public Spob(EntityManager manager, Serial serial)
            : base(manager, serial)
        {
            
        }

        public float ExtractOre(float frameSeconds)
        {
            float amount = (ResourceOre > frameSeconds) ? frameSeconds : ResourceOre;
            ResourceOre -= amount;
            return amount;
        }

        public float ResourceOre = 100f;
    }
}
