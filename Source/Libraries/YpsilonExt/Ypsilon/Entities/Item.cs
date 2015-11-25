using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Entities
{
    class Item : AEntity
    {
        private int m_Amount;

        public virtual string DefaultName { get { return null; } }

        public int Amount
        {
            get { return m_Amount; }
        }

        public Item()
        {

        }
    }
}
