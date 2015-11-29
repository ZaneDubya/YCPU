using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Ypsilon.Entities.Collections
{
    /// <summary>
    /// A collection of modules, for a ship.
    /// </summary>
    public class ModuleList
    {
        private Ship m_Parent;
        private List<AModule> m_Modules;
        private Dictionary<Point, AModule> m_Hardpoints;

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
                // add up all hold space from holdspace providing modules.
                foreach (AModule m in m_Modules)
                    holdspace += m.HoldSpace;
                // any remaining open hardpoints provide some space each.
                foreach (Point p in m_Hardpoints.Keys)
                    if (m_Hardpoints[p] == null)
                        holdspace += 10;

                return holdspace;
            }
        }

        public ModuleList(Ship parent)
        {
            m_Parent = parent;
            m_Modules = new List<AModule>();
            m_Hardpoints = new Dictionary<Point, AModule>();
            RecalculateHardpoints();
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
                RecalculateHardpoints();
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
                    Point p = new Point(position.X + x, position.Y + y);
                    if (!m_Hardpoints.ContainsKey(p) || m_Hardpoints[p] != null)
                        return false;
                }
            }
            return true;
        }

        public bool TryGetModule(Point position, out AModule module)
        {
            module = null;
            if (!m_Hardpoints.ContainsKey(position) || m_Hardpoints[position] != null)
                return false;

            module = m_Hardpoints[position];
            return  true;
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
            RecalculateHardpoints();
        }

        private void RecalculateHardpoints()
        {
            m_Hardpoints.Clear();
            for (int i = 0; i < m_Parent.Definition.Hardpoints.Count; i++)
                m_Hardpoints.Add(m_Parent.Definition.Hardpoints[i], null);

            for (int i = 0; i < m_Modules.Count; i++)
            {
                for (int y = 0; y < m_Modules[i].ModuleSize.Y; y++)
                {
                    for (int x = 0; x < m_Modules[i].ModuleSize.X; x++)
                    {
                        Point p = new Point(m_Modules[i].ModuleHardpoint.X + x, m_Modules[i].ModuleHardpoint.Y);
                        if (m_Hardpoints[p] != null)
                        {
                            throw new Exception("Double-filled hardpoint???");
                        }
                        else
                        {
                            m_Hardpoints[p] = m_Modules[i];
                        }
                    }
                }
            }
        }
    }
}
