using Microsoft.Xna.Framework;
using Ypsilon.Data;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A piece of ship equipment.
    /// </summary>
    public abstract class AModule : AItem
    {
        public Point ModuleHardpoint { get; set; }
        public virtual Point ModuleSize { get { return PointX.OneByOne; } }

        protected override void OnDispose()
        {
            if (Parent != null)
            {
                Parent.RemoveEntity(this);
            }
        }
    }
}
