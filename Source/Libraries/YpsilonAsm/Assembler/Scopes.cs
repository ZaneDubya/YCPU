/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;
using System.Collections.Generic;

namespace Ypsilon.Assembler
{
    public class Scopes
    {
        private readonly List<Scope> m_Scopes = new List<Scope>();
        private readonly Scope m_Global = new Scope(0, 0);

        public bool IsScopeOpen => GetLastOpenScope() == m_Global;

        public void ScopeOpen(int address, int line)
        {
            m_Scopes.Add(new Scope(address, line));
        }

        public bool ScopeClose(int address)
        {
            Scope scope = GetLastOpenScope();
            if (scope == m_Global || scope == null)
                return false;
            scope.EndAddress = address - 1;
            return true;
        }

        public bool AddLabel(string label, int address)
        {
            Scope scope = GetLastOpenScope();
            if (scope == m_Global)
            {
                if (!m_Global.AddLabel(label, address))
                {
                    throw new Exception($"Label '{label}' already exists within the global scope");
                }
                return true;
            }
            if (!scope.AddLabel(label, address))
            {
                throw new Exception(
                    $"Label '{label}' already exists within the {((scope == m_Global) ? "global" : "current local")} scope");
            }
            return true;
        }

        public bool ContainsLabel(string label, int fromAddress)
        {
            return LabelAddress(label, fromAddress) != -1;
        }

        public bool ContainsAlias(string label, int fromAddress)
        {
            return AliasAddress(label, fromAddress) != -1;
        }

        public int LabelAddress(string label, int fromAddress)
        {
            Scope scopeMatch = m_Global;

            for (int i = 0; i < m_Scopes.Count; i++)
            {
                if (m_Scopes[i].ContainsAddress(fromAddress) && m_Scopes[i].ContainsLabel(label))
                    if ((scopeMatch == null) || (m_Scopes[i].StartAddress >= scopeMatch.StartAddress))
                        scopeMatch = m_Scopes[i];
            }

            if (scopeMatch != null)
                return scopeMatch.LabelAddress(label);
            return -1;
        }

        public int AliasAddress(string alias, int fromAddress)
        {
            Scope scopeMatch = m_Global;

            for (int i = 0; i < m_Scopes.Count; i++)
            {
                if (m_Scopes[i].ContainsAddress(fromAddress) && m_Scopes[i].ContainsAlias(alias))
                    if ((scopeMatch == null) || (m_Scopes[i].StartAddress >= scopeMatch.StartAddress))
                        scopeMatch = m_Scopes[i];
            }

            if (scopeMatch != null)
                return scopeMatch.AliasAddress(alias);
            return -1;
        }

        public Scope GetLastOpenScope()
        {
            for (int i = m_Scopes.Count - 1; i >= 0; i--)
            {
                if (m_Scopes[i].IsOpen)
                    return m_Scopes[i];
            }
            return m_Global;
        }

        public bool TryGetOpenScope(out Scope scope)
        {
            scope = GetLastOpenScope();
            if (scope == m_Global)
                return false;
            return true;
        }


        public class Scope
        {
            public int StartAddress;
            public int EndAddress = -1;
            public int StartLine;

            private readonly Dictionary<string, ushort> m_LabelAddressDictionary = new Dictionary<string, ushort>();
            private readonly Dictionary<string, ushort> m_AliasDirectory = new Dictionary<string, ushort>();

            public bool IsOpen => EndAddress == -1;

            public Scope(int beginAddress, int beginLine)
            {
                StartAddress = beginAddress;
                StartLine = beginLine;
            }

            public bool ContainsAddress(int address)
            {
                return ((StartAddress <= address) && (EndAddress >= address));
            }

            public override string ToString()
            {
                return $"{StartAddress}~{EndAddress}, {m_LabelAddressDictionary.Count} labels";
            }

            // ======================================================================
            // Labels
            // ======================================================================

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

            // ======================================================================
            // Aliases
            // ======================================================================

            public bool AddAlias(string alias, ushort address)
            {
                alias = alias.ToLower();
                if (ContainsLabel(alias))
                    return false;
                m_AliasDirectory.Add(alias, address);
                return true;
            }

            public bool ContainsAlias(string alias)
            {
                alias = alias.ToLower();
                return m_AliasDirectory.ContainsKey(alias);
            }

            public ushort AliasAddress(string alias)
            {
                alias = alias.ToLower();
                if (!ContainsAlias(alias))
                    return 0xffff;
                return m_AliasDirectory[alias];
            }
        }
    }
}
