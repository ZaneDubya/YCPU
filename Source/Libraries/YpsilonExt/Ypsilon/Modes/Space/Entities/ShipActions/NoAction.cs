using Ypsilon.Entities;

namespace Ypsilon.Modes.Space.Entities.ShipActions
{
    /// <summary>
    /// Just flying along.
    /// </summary>
    class NoAction : AAction
    {
        public NoAction(Ship parent)
            : base(parent)
        {

        }
    }
}
