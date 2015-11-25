using Ypsilon.Data;
using Ypsilon.Scripts.Vendors;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A space object: planet or asteroid.
    /// </summary>
    class Spob : AEntity
    {
        public ASpobDefinition Definition;
        public VendorInfo Exchange;

        public override string DefaultName { get { return Definition.Name; } }
        public override string Description { get { return Definition.Description; } }

        public Spob()
        {
            Exchange = new VendorInfo();
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
