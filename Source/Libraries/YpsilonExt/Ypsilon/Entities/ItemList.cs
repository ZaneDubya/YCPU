using System;
using System.Collections.Generic;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A collection of items!
    /// </summary>
    class ItemList
    {
        private AEntity m_Parent;
        private List<AItem> m_Items;

        public int ItemCount
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

        /// <summary>
        /// Attempts to add an item to this collection. Returns false if the item would not fit inside the collection.
        /// </summary>
        public bool TryAddItem(Type itemType, int amount)
        {
            bool canHold = true;
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

        public ItemList(AEntity parent)
        {
            m_Parent = parent;
            m_Items = new List<AItem>();
        }
    }
}
