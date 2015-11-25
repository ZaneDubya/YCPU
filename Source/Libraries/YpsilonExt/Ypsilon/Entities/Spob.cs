using Ypsilon.Data;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A space object: planet or asteroid.
    /// </summary>
    class Spob : AEntity
    {
        public ASpobDefinition Definition = new ASpobDefinition();

        public override string Name
        {
            get
            {
                return Definition.Name;
            }
        }

        public override string Description
        {
            get
            {
                return Definition.Description;
            }

        }

        public Spob()
        {
            
        }

        public int ExtractOre(int extractedUnits)
        {
            int amount = (ResourceOre < extractedUnits) ? ResourceOre : extractedUnits;
            ResourceOre -= extractedUnits;
            return amount;
        }

        public int ResourceOre = 10;
        public bool CanLandHere = false;
    }
}
