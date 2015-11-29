namespace Ypsilon.Entities
{
    /// <summary>
    /// Components manage state-based update for an entity.
    /// They do not manage an entity's intrinsic variables.
    /// </summary>
    public class AComponent
    {
        public bool IsInitialized
        {
            get;
            private set;
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public AComponent()
        {

        }

        public void Initialize(World world, AEntity entity)
        {
            if (IsInitialized)
                return;

            OnInitialize(world, entity);

            IsInitialized = true;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            OnDipose();

            IsDisposed = true;
        }

        protected virtual void OnInitialize(World world, AEntity entity)
        {

        }

        protected virtual void OnDipose()
        {

        }

        public virtual void Update(World world, AEntity entity, float frameSeconds)
        {

        }
    }
}
