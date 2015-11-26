using Microsoft.Xna.Framework;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A piece of ship equipment.
    /// </summary>
    public abstract class AModule : AEntity
    {
        public virtual Point Size { get { return PointX.OneByOne; } }
        public Point Position { get; set; }

        protected override void OnDispose()
        {
            if (Parent != null)
            {
                Parent.RemoveEntity(this);
            }
        }
    }
}
