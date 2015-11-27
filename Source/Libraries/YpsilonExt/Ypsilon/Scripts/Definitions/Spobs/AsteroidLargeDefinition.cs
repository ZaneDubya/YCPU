using Microsoft.Xna.Framework;
using Ypsilon.Data;

namespace Ypsilon.Scripts.Definitions.Spobs
{
    class AsteroidLargeDefinition : ASpobDefinition
    {
        public override string Name { get { return "Class D Asteroid"; } }
        // public override string Description { get { return base.Name; } }
        public override Color Color { get { return Color.Gray; } }
        public override float Size { get { return 28f; } }
        public override float RotationPeriod { get { return 14f; } }
        public override float VertexRandomizationFactor { get { return 0.4f; } }
    }
}
