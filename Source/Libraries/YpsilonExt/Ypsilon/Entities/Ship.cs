using Ypsilon.Data;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A starship.
    /// </summary>
    class Ship : AEntity
    {
        public AShipDefinition Definition = new AShipDefinition();

        public Ship()
        {
            
        }
    }
}
