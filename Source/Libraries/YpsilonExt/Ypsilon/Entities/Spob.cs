using Microsoft.Xna.Framework;
using Ypsilon.Entities.Movement;
using Ypsilon.Scripts.Vendors;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A space object: planet or asteroid.
    /// </summary>
    class Spob : AEntity
    {
        public VendorInfo Exchange;

        public float Size = 40f;
        public float RotationPeriod = 30f;
        public Color Color = Color.Magenta;
        public float VertexRandomizationFactor = 0f;

        public PlanetRotator Rotator
        {
            get;
            private set;
        }

        public Spob()
        {
            Exchange = new VendorInfo();
            Rotator = new PlanetRotator();
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
