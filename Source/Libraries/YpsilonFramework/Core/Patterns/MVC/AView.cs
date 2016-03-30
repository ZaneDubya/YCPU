using Microsoft.Xna.Framework;

namespace Ypsilon.Core.Patterns.MVC
{
    /// <summary>
    /// Abstract Model - polls the state of a model, and renders it for the player.
    /// </summary>
    public abstract class AView
    {
        protected readonly AModel Model;

        public AView(AModel parentModel)
        {
            Model = parentModel;
        }

        public virtual void Draw(float frameSeconds)
        {

        }
    }
}
