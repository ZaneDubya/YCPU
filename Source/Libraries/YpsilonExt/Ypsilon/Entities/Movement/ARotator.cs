using Microsoft.Xna.Framework;
using Ypsilon.Data;

namespace Ypsilon.Entities.Movement
{
    abstract class ARotator
    {
        public abstract Vector3 Forward { get; }
        public abstract Matrix RotationMatrix { get; }

        protected readonly ShipDefinition Definition;

        public ARotator(ShipDefinition definition)
        {
            Definition = definition;
        }

        /// <summary>
        /// Rotate the ship using the specified percentage of the rotation rate.
        /// </summary>
        /// <param name="updownRotation">Rotation around the x-axis of the ship, from -1 to 1.</param>
        /// <param name="leftrightRotation">Rotation around the Y-axis of the ship, from -1 to 1.</param>
        /// <param name="gameSeconds">Time between last frame and this frame.</param>
        public abstract void Rotate(float updownRotation, float leftrightRotation, double frameSeconds);
    }
}
