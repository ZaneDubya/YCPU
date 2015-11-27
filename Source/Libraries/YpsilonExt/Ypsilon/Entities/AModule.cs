using Microsoft.Xna.Framework;
using Ypsilon.Data;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A piece of ship equipment.
    /// </summary>
    public abstract class AModule : AEntity
    {
        public AModuleDefinition Definition { get; internal set; }
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
