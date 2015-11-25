using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Entities
{
    public class AEntityComponent
    {
        public AEntity Entity
        {
            get;
            private set;
        }

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

        public AEntityComponent(AEntity entity)
        {
            Entity = entity;
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            OnInitialize();

            IsInitialized = true;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            OnDipose();

            IsDisposed = true;
        }

        protected virtual void OnInitialize()
        {

        }

        protected virtual void OnDipose()
        {

        }

        public virtual void Update(float frameSeconds)
        {

        }
    }
}
