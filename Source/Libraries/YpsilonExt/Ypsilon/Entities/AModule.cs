using Microsoft.Xna.Framework;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A piece of ship equipment.
    /// </summary>
    public abstract class AModule : AEntity
    {
        public abstract Point Size { get; }
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
