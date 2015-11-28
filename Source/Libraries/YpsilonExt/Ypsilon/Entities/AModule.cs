using Microsoft.Xna.Framework;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A piece of ship equipment.
    /// </summary>
    public abstract class AModule : AItem
    {
        public Point ModuleHardpoint { get; set; }
        public virtual Point ModuleSize { get { return PointX.OneByOne; } }

        /// <summary>
        /// Modules don't stack.
        /// </summary>
        public override bool CanStack { get { return false; } }

        public virtual int HoldSpace { get { return 0; } }

        public override string DefaultName { get { return "Unnamed module"; } }

        protected override void OnDispose()
        {
            if (Parent != null)
            {
                Parent.RemoveEntity(this);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }


    }
}
