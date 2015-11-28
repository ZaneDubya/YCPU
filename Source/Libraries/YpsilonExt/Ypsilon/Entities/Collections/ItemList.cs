using System;
using System.Collections.Generic;

namespace Ypsilon.Entities.Collections
{
    /// <summary>
    /// A collection of items!
    /// </summary>
    public class ItemList
    {
        private Ship m_Parent;
        private List<AItem> m_Items;

        public int Count
        {
            get
            {
                return m_Items == null ? 0 : m_Items.Count;
            }
        }

        public AItem this[int index]
        {
            get
            {
                if (m_Items == null)
                    return null;
                if (index < 0 || index >= m_Items.Count)
                    return null;
                return m_Items[index];
            }
        }

        public int Tons
        {
            get
            {
                int tons = 0;
                foreach (AItem i in m_Items)
                {
                    tons += i.Amount;
                }
                return tons;
            }
        }

        public ItemList(Ship parent)
        {
            m_Parent = parent;
            m_Items = new List<AItem>();
        }

        /// <summary>
        /// Attempts to add an item to this collection. Returns false if the item would not fit inside the collection.
        /// </summary>
        public bool TryAddItem(Type itemType, int amount)
        {
            bool canHold = TryToFitAnItemIn(itemType, amount);
            AItem item;
            if (canHold)
            {
                if (TryGetItem(itemType, out item))
                {
                    item.Amount += amount;
                }
                else
                {
                    item = (AItem)AEntity.CreateEntity(itemType);
                    item.Parent = m_Parent;
                    item.Amount = amount;
                    m_Items.Add(item);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if item of type is in collection.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetItem(Type itemType, out AItem item)
        {
            foreach (AItem i in m_Items)
            {
                if (i.GetType() == itemType)
                {
                    item = i;
                    return true;
                }
            }

            item = null;
            return false;
        }

        public void RemoveItem(AItem item)
        {
            for (int i = 0; i < m_Items.Count; i++)
            {
                if (m_Items[i] == item)
                {
                    m_Items.RemoveAt(i);
                    i--;
                }
            }
        }

        private bool TryToFitAnItemIn(Type itemType, int amount)
        {
            if (Tons < m_Parent.Modules.HoldSpace)
                return true;
            return false;
        }
    }
}
