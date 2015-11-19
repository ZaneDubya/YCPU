using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Entities
{
    class AEntityComponent
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

        protected virtual void OnInitialize()
        {

        }

        public virtual void Update(float frameSeconds)
        {

        }
    }
}
