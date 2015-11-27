using Microsoft.Xna.Framework;

namespace Ypsilon.Data
{
    class ASpobDefinition
    {
        public virtual float Size { get { return 60f; } }

        /// <summary>
        /// Rotation period, in seconds.
        /// </summary>
        public virtual float RotationPeriod { get { return 60f; } }

        public virtual Color Color { get { return Color.Gray; } }
        public virtual string Name { get { return "Spob"; } }
        public virtual string Description { get { return string.Empty; } }

        public virtual float VertexRandomizationFactor { get { return 0f; } }

        public bool DoRandomizeVertexes
        {
            get
            {
                return VertexRandomizationFactor != 0f;
            }
        }
    }
}
