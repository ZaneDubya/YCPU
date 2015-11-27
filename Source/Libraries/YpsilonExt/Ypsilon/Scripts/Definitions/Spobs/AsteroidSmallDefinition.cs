using Microsoft.Xna.Framework;
using Ypsilon.Data;

namespace Ypsilon.Scripts.Definitions.Spobs
{
    class AsteroidSmallDefinition : ASpobDefinition
    {
        public override string Name { get { return "Class B Asteroid"; } }
        public override string Description { get { return "The Alpha Asteroids are the remnants of a planet that  either never formed or was torn apart by the gravitational forces of Beta Caeli. Most of the lighter carbonaceous and stony asteroids have been ejected from the belt, leaving the heavier metallic asteroids behind. ~It is estimated from the combined mass of the metallic Alpha Asteroids that a planet two-thirds the size of Mercury could have once existed. Estimates put the combined mass of the Alpha Asteroids at 4.23x10(18) kilograms. ~The Alpha Asteroids are primarily found between 55 and 74 million kilometers from Beta Caeli."; } }
        public override Color Color { get { return Color.LightGray; } }
        public override float Size { get { return 10f; } }
        public override float RotationPeriod { get { return 10f; } }
        public override float VertexRandomizationFactor { get { return 0.6f; } }
    }
}
