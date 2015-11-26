using Microsoft.Xna.Framework;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A piece of ship equipment.
    /// </summary>
    class AModule : AEntity
    {
        public virtual Point Size { get; }
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
