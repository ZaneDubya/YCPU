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

        public bool TryAdd(AItem item)
        {
            bool canHold = true;
            if (canHold)
            {
                m_Items.Add(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        public ItemList(AEntity parent)
        {
            m_Parent = parent;
            m_Items = new List<AItem>();
        }
    }
}
