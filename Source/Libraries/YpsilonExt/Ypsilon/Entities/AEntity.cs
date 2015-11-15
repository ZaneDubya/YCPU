using System;
using Ypsilon.Graphics;

namespace Ypsilon.Entities
{
    class AEntity : IDisposable
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

        private bool m_IsPlayerEntity = false;
        public bool IsPlayerEntity
        {
            get
            {
                return m_IsPlayerEntity;
            }
            set
            {
                // if we're not changing the value, do nothing.
                if (value == m_IsPlayerEntity)
                    return;
                // if we're setting this to false, get rid of it.
                if (value == false)
                {
                    m_IsPlayerEntity = false;
                    if (Manager.PlayerSerial == Serial)
                        Manager.PlayerSerial = Serial.Null;
                }
                // if we're setting this to true, make sure entity manager knows.
                if (value == true)
                {
                    m_IsPlayerEntity = true;
                    if (Manager.PlayerSerial != Serial)
                        Manager.PlayerSerial = Serial;
                }
            }
        }

        public AEntity(EntityManager manager, Serial serial)
        {
            Manager = manager;
            Serial = serial;

            Position = Position3D.Zero;
        }

        public virtual void Update(float frameSeconds)
        {

        }

        public virtual void Draw(VectorRenderer renderer, Position3D worldSpaceCenter)
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
