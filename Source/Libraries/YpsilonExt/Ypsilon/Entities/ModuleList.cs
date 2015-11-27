using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Ypsilon.Data;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A collection of modules, for a ship.
    /// </summary>
    public class ModuleList
    {
        private Ship m_Parent;
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

        public int HoldSpace
        {
            get
            {
                int holdspace = 0;
                foreach (AModule m in m_Modules)
                    holdspace += m.HoldSpace;
                return holdspace;
            }
        }

        public ModuleList(Ship parent)
        {
            m_Parent = parent;
            m_Modules = new List<AModule>();
        }

        /// <summary>
        /// Attempts to add an item to this list. Returns false if the item would not fit in the designated place.
        /// </summary>
        public bool TryAddModule(AModule module, Point position)
        {
            bool canHold = CanPlaceModule(module, position);
            if (canHold)
            {
                module.Parent = m_Parent;
                module.ModuleHardpoint = position;
                m_Modules.Add(module);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanPlaceModule(AModule module, Point position)
        {
            for (int y = 0; y < module.ModuleSize.Y; y++)
            {
                for (int x = 0; x < module.ModuleSize.X; x++)
                {
                    if (!m_Parent.Definition.Hardpoints.Contains(new Point(position.X + x, position.Y + y)))
                        return false;
                    AModule m;
                    if (TryGetModule(new Point(position.X + x, position.Y + y), out m))
                        return false;
                }
            }
            return true;
        }

        public bool TryGetModule(Point position, out AModule module)
        {
            foreach (AModule m in m_Modules)
            {
                if (position.X >= m.ModuleHardpoint.X && position.X < m.ModuleHardpoint.X + m.ModuleSize.X &&
                    position.Y >= m.ModuleHardpoint.Y && position.Y < m.ModuleHardpoint.Y + m.ModuleSize.Y)
                {
                    module = m;
                    return true;
                }
            }

            module = null;
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
