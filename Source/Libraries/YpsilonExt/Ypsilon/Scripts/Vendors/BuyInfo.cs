using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Vendors
{
    /// <summary>
    /// A list of all items that this vendor will purchase from a seller.
    /// </summary>
    class BuyInfo
    {
        private Dictionary<Type, float> m_Table = new Dictionary<Type, float>();
        private Type[] m_Types;

        public BuyInfo()
        {
        }

        public void Add(Type type, float differential)
        {
            m_Table[type] = differential;
            m_Types = null;
        }

        /// <summary>
        /// Gets the value this vendor will give the seller if the proferred item is sold. Returns -1 if vendor does not buy this item.
        /// </summary>
        public int GetSellPriceFor(AItem item)
        {
            float diff = 0;
            if (m_Table.TryGetValue(item.GetType(), out diff))
            {
                return (int)(item.Price * diff);
            }
            return -1;
        }

        public Type[] Types
        {
            get
            {
                if (m_Types == null)
                {
                    m_Types = new Type[m_Table.Keys.Count];
                    m_Table.Keys.CopyTo(m_Types, 0);
                }

                return m_Types;
            }
        }

        public bool WillPurchase(AItem item)
        {
            if (item.Nontransferable)
                return false;

            return m_Table.ContainsKey(item.GetType());
        }
    }
}
