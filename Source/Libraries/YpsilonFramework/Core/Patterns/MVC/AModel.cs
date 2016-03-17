namespace Ypsilon.Core.Patterns.MVC
{
    /// <summary>
    /// Abstract Model. Maintains the state, core data, and update logic of a model.
    /// </summary>
    public abstract class AModel
    {
        protected AView m_View;
        public AView GetView()
        {
            if (m_View == null)
                    m_View = CreateView();
            return m_View;
        }

        protected AController m_Controller;
        public AController GetController()
        {
            if (m_Controller == null)
                m_Controller = CreateController();
            return m_Controller;
        }

        public AModel()
        {

        }

        public abstract void Initialize();
        public abstract void Dispose();

        public abstract void Update(float totalSeconds, float frameSeconds);

        protected abstract AView CreateView();
        protected abstract AController CreateController();
    }
}
