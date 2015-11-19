using Microsoft.Xna.Framework;
using System;
using Ypsilon.Core.Graphics;
using Ypsilon.World.Data;
using Ypsilon.World.Input;
using Ypsilon.PlayerState;

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

        public PropertyList Properties = new PropertyList();

        public bool IsInitialized
        {
            get;
            private set;
        }

        public bool IsPlayerEntity
        {
            get
            {
                return Serial == WorldModel.PlayerSerial;
            }
        }

        public bool IsSelected
        {
            get
            {
                return Serial == WorldModel.SelectedSerial;
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

        protected Vector3[] DrawVertices = null;
        protected Color DrawColor = Color.White;
        protected Matrix DrawMatrix = Matrix.Identity;

        public AEntity(EntityManager manager, Serial serial)
        {
            Manager = manager;
            Serial = serial;

            Position = Position3D.Zero;
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            OnInitialize();

            IsInitialized = true;
        }

        public virtual void Update(float frameSeconds)
        {

        }

        public virtual void Draw(VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            Vector3[] v = new Vector3[DrawVertices.Length];
            for (int i = 0; i < v.Length; i++)
                v[i] = Vector3.Transform(DrawVertices[i] * ViewSize, DrawMatrix);
            renderer.DrawPolygon(v, true, DrawColor, false);

            mouseOverList.AddEntityIfMouseIsOver(this, DrawMatrix.Translation);

            if (IsSelected)
            {
                Matrix drawMatrixNoRotation = Matrix.Identity;
                drawMatrixNoRotation.Translation = DrawMatrix.Translation;
                Vector3[] selection = new Vector3[Vertices.SelectionLeft.Length];
                for (int i = 0; i < selection.Length; i++)
                    selection[i] = Vector3.Transform(Vertices.SelectionLeft[i] * ViewSize, drawMatrixNoRotation);
                renderer.DrawPolygon(selection, false, Color.Yellow, false);
                for (int i = 0; i < selection.Length; i++)
                    selection[i] = Vector3.Transform(Vertices.SelectionRight[i] * ViewSize, drawMatrixNoRotation);
                renderer.DrawPolygon(selection, false, Color.Yellow, false);
            }
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
