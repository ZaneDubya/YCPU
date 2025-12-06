namespace Ypsilon.Core.Patterns.MVC {
    /// <summary>
    /// Abstract Model. Maintains the state, core data, and update logic of a model.
    /// </summary>
    public abstract class AModel {
        protected AController Controller;
        protected AView View;
        protected abstract AController CreateController();

        protected abstract AView CreateView();
        public abstract void Dispose();

        public AController GetController() {
            if (Controller == null)
                Controller = CreateController();
            return Controller;
        }

        public AView GetView() {
            if (View == null)
                View = CreateView();
            return View;
        }

        public abstract void Initialize();

        public abstract void Update(float totalSeconds, float frameSeconds);
    }
}