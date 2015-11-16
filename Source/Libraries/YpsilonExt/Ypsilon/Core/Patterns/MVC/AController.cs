using Ypsilon.Core.Input;

namespace Ypsilon.Core.Patterns.MVC
{
    /// <summary>
    /// Abstract Controller - receives input, interacts with state of model.
    /// </summary>
    public abstract class AController
    {
        protected AModel Model;

        public AController(AModel parent_model)
        {
            Model = parent_model;
        }

        public virtual void ReceiveInput(float frameSeconds, InputManager input)
        {

        }
    }
}
