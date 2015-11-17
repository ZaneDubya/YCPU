using System;
using Ypsilon.Core.Graphics;
using Ypsilon.Core;
using Microsoft.Xna.Framework;

namespace Ypsilon.World.Entities
{
    abstract class AEntity : IDisposable
    {
        protected EntityManager Manager
        {
            get;
            private set;
        }

        public Serial Serial
        {
            get;
            private set;
        }

        public Position3D Position
        {
            get;
            set;
        }

        private bool m_IsInitialized = false;
        public bool IsInitialized
        {
            get
            {
                return m_IsInitialized;
            }
        }

        public bool IsPlayerEntity
        {
            get
            {
                return Serial == State.Vars.PlayerSerial;
            }
        }

        public bool IsSelected
        {
            get
            {
                return Serial == State.Vars.SelectedSerial;
            }
        }

        public bool IsVisible
        {
            get
            {
                return true;
            }
        }

        public abstract float ViewSize
        {
            get;
        }

        public AEntity(EntityManager manager, Serial serial)
        {
            Manager = manager;
            Serial = serial;

            Position = Position3D.Zero;
        }

        public void Initialize()
        {
            if (m_IsInitialized)
                return;

            OnInitialize();

            m_IsInitialized = true;
        }

        public virtual void Update(float frameSeconds)
        {

        }

        public virtual void Draw(VectorRenderer renderer, Position3D worldSpaceCenter)
        {

        }

        protected virtual void OnInitialize()
        {

        }

        #region IDisposable Support
        private bool m_IsDisposedValue = false; // To detect redundant calls

        public bool IsDisposed
        {
            get
            {
                return m_IsDisposedValue;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_IsDisposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                m_IsDisposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AEntity() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
