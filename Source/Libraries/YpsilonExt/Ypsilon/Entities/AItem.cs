using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Entities
{
    public abstract class AItem : AEntity
    {
        public abstract int Price { get; }
        public virtual bool CanStack { get { return true; } }

        private int m_Amount = 1;

        public int Amount
        {
            get { return m_Amount; }
            set
            {
                if (value <= 0)
                {
                    m_Amount = 0;
                    Dispose();
                }
                else
                {
                    m_Amount = value;
                }
            }
        }

        public bool Nontransferable { get; set; }

        protected override void OnDispose()
        {
            if (Parent != null)
            {
                Parent.RemoveEntity(this);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", Amount, Name);
        }
    }
}
