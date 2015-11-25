using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Entities
{
    abstract class AItem : AEntity
    {
        public abstract int Price { get; }

        private int m_Amount;

        public int Amount
        {
            get { return m_Amount; }
        }

        public bool Nontransferable { get; set; }
    }
}
