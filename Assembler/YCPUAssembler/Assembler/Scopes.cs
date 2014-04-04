using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Assembler
{
    public class Scopes
    {
        private List<Scope> m_Scopes;
        private Scope m_Global;

        public Scopes()
        {
            m_Scopes = new List<Scope>();
            m_Global = new Scope(0);
        }

        public void ScopeOpen(int address)
        {
            m_Scopes.Add(new Scope(address));
        }

        public bool ScopeClose(int address)
        {
            Scope scope = LastOpenScope();
            if (scope == m_Global || scope == null)
                return false;
            scope.End = address - 1;
            return true;
        }

        public bool AddLabel(string label, int address, bool local)
        {
            if (local)
            {
                Scope scope = LastOpenScope();
                if (scope == m_Global)
                {
                    return AddLabel(label, address, false);
                }
                if (!scope.AddLabel(label, address))
                {
                    throw new Exception(string.Format("Label '{0}' already exists within the {1} scope.", label, (scope == m_Global) ? "global" : "current local"));
                }
                return true;
            }
            else
            {
                if (!m_Global.AddLabel(label, address))
                {
                    throw new Exception(string.Format("Label '{0}' already exists within the global scope.", label));
                }
                return true;
            }
        }

        public bool ContainsLabel(string label, int from_address)
        {
            return (LabelAddress(label, from_address) != -1);
        }

        public int LabelAddress(string label, int from_address)
        {
            Scope scope_match = m_Global;

            for (int i = 0; i < m_Scopes.Count; i++)
            {
                if (m_Scopes[i].ContainsAddress(from_address) && m_Scopes[i].ContainsLabel(label))
                    if ((scope_match == null) || (m_Scopes[i].Begin >= scope_match.Begin))
                        scope_match = m_Scopes[i];
            }

            if (scope_match != null)
                return scope_match.LabelAddress(label);
            return -1;
        }

        /*private Scope ParentScope(Scope child)
        {
            if (child == m_Global)
                return null;

            int child_index = m_Scopes.IndexOf(child);
            // if this child does not exist in the collection, error out.
            if (child_index == -1)
                return m_Global;

            for (int i = child_index - 2; i >= 0; i--)
            {
                if ((m_Scopes[i].Begin <= child.Begin) && ((m_Scopes[i].IsOpen) || (m_Scopes[i].End >= child.End)))
                    return m_Scopes[i];
            }

            return m_Global;
        }*/

        private Scope LastOpenScope()
        {
            for (int i = m_Scopes.Count - 1; i >= 0; i--)
            {
                if (m_Scopes[i].IsOpen)
                    return m_Scopes[i];
            }
            return m_Global;
        }

        public bool OpenScopes()
        {
            Scope scope = LastOpenScope();
            if (scope == m_Global)
                return false;
            return true;
        }


        private class Scope
        {
            public int Begin = -1;
            public int End = -1;

            private Dictionary<string, ushort> m_LabelAddressDictionary;

            public bool IsOpen
            {
                get { return End == -1; }
            }

            public Scope(int begin)
            {
                Begin = begin;
                m_LabelAddressDictionary = new Dictionary<string, ushort>();
            }

            public bool ContainsAddress(int address)
            {
                return ((Begin <= address) && (End >= address));
            }

            public bool AddLabel(string label, int address)
            {
                if (ContainsLabel(label))
                    return false;
                m_LabelAddressDictionary.Add(label, (ushort)address);
                return true;
            }

            public bool ContainsLabel(string label)
            {
                return m_LabelAddressDictionary.ContainsKey(label);
            }

            public int LabelAddress(string label)
            {
                if (!ContainsLabel(label))
                    return -1;
                return m_LabelAddressDictionary[label];
            }

            public override string ToString()
            {
                return string.Format("{0}-{1}, {2} labels", Begin, End, m_LabelAddressDictionary.Count);
            }
        }
    }
}
