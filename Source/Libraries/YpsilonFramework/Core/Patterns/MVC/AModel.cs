namespace Ypsilon.Core.Patterns.MVC
{
    /// <summary>
    /// Abstract Model. Maintains the state, core data, and update logic of a model.
    /// </summary>
    public abstract class AModel
    {
        protected AView View;
        public AView GetView()
        {
            if (View == null)
                    View = CreateView();
            return View;
        }

        protected AController Controller;
        public AController GetController()
        {
            if (Controller == null)
                Controller = CreateController();
            return Controller;
        }

        public abstract void Initialize();
        public abstract void Dispose();

        public abstract void Update(float totalSeconds, float frameSeconds);

        protected abstract AView CreateView();
        protected abstract AController CreateController();
    }
}
