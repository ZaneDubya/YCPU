using System;
using System.Collections.Generic;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        int ParseLabel(string line, bool local, ParserState state)
        {
            int index1 = line.IndexOf(' ');
            int index2 = line.IndexOf('\t');
            int index = index1 < index2 || index2 == -1 ? index1 : index2 < index1 || index1 != -1 ? index2 : -1;

            int colon_pos = line.IndexOf(':');
            string labelName;
            if (colon_pos == 0)
                labelName = index > 1 ? line.Substring(1, index - 1) : line.Substring(1, line.Length - 1);
            else
                labelName = line.Substring(0, colon_pos);

            if (!state.m_Scopes.AddLabel(labelName.Trim().ToLower(), state.machineCode.Count, local))
                throw new Exception(string.Format("Error adding label '{0}'.", labelName));
            return index;
        }

        void SetLabelAddressReferences(ParserState state)
        {
            foreach (ushort index in state.Labels.Keys)
            {
                string labelName = state.Labels[index].ToLower();

                if (!state.m_Scopes.ContainsLabel(labelName, index))
                {
                    throw new Exception(string.Format("Unknown label reference '{0}'", labelName));
                }

                ushort address = (ushort)state.m_Scopes.LabelAddress(labelName, index);
                state.machineCode[index] = (byte)(address & 0x00ff);
                state.machineCode[index + 1] = (byte)((address & 0xff00) >> 8);
            }
        }

        void SetDataFieldLabelAddressReferences(ParserState state)
        {
            foreach (ushort key in state.DataFields.Keys)
            {
                string labelName = state.DataFields[key].ToLower();

                if (state.m_Scopes.ContainsLabel(labelName, key) != true)
                {
                    throw new Exception(string.Format("Unknown label '{0}' referenced in data field", labelName));
                }

                ushort address = (ushort)state.m_Scopes.LabelAddress(labelName, key);
                state.machineCode[key] = (byte)(address & 0x00ff);
                state.machineCode[key + 1] = (byte)((address & 0xff00) >> 8);
            }
        }

        void SetBranchLabelAddressReferences(ParserState state)
        {
            foreach (ushort index in state.Branches.Keys)
            {
                string labelName = state.Branches[index];

                if (!state.m_Scopes.ContainsLabel(labelName, index))
                {
                    throw new Exception(string.Format("Unknown label reference '{0}'.", labelName));
                }

                ushort label_address = (ushort)state.m_Scopes.LabelAddress(labelName, index);
                int delta = label_address - index;
                if ((delta > sbyte.MaxValue) || (delta < sbyte.MinValue))
                    throw new Exception("Branch operation out of range.");
                state.machineCode[index + 1] = (byte)((sbyte)delta);
            }
        }
    }
}
