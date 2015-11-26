using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A collection of items!
    /// </summary>
    public class ModuleList
    {
        private AEntity m_Parent;
        private List<AModule> m_Modules;

        public int Count
        {
            get
            {
                return m_Modules == null ? 0 : m_Modules.Count;
            }
        }

        public AModule this[int index]
        {
            get
            {
                if (m_Modules == null)
                    return null;
                if (index < 0 || index >= m_Modules.Count)
                    return null;
                return m_Modules[index];
            }
        }

        public ModuleList(AEntity parent)
        {
            m_Parent = parent;
            m_Modules = new List<AModule>();
        }

        /// <summary>
        /// Attempts to add an item to this list. Returns false if the item would not fit inside the list.
        /// </summary>
        public bool TryAddModule(Type type, Point position)
        {
            bool canHold = true;
            if (canHold)
            {
                AModule module = (AModule)AEntity.CreateEntity(type);
                module.Parent = m_Parent;
                module.Position = position;
                m_Modules.Add(module);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if module of type is in collection.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetModule(Type type, out AModule item)
        {
            foreach (AModule m in m_Modules)
            {
                if (m.GetType() == type)
                {
                    item = m;
                    return true;
                }
            }

            item = null;
            return false;
        }

        public void RemoveModule(AModule module)
        {
            for (int i = 0; i < m_Modules.Count; i++)
            {
                if (m_Modules[i] == module)
                {
                    m_Modules.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
