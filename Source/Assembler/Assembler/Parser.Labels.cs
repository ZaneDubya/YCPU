using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Assembler
{
    partial class Parser
    {
        protected void SetLabelAddressReferences()
        {
            foreach (ushort index in m_LabelReferences.Keys)
            {
                string labelName = this.m_LabelReferences[index];

                if (!m_Scopes.ContainsLabel(labelName, index))
                {
                    throw new Exception(string.Format("Unknown label reference '{0}'", labelName));
                }

                ushort address = (ushort)m_Scopes.LabelAddress(labelName, index);
                m_MachineCodeOutput[index] = (byte)(address & 0x00ff);
                m_MachineCodeOutput[index + 1] = (byte)((address & 0xff00) >> 8);
            }
        }

        protected int ParseLabel(string line, bool local)
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

            if (!m_Scopes.AddLabel(labelName.Trim(), m_MachineCodeOutput.Count, local))
                throw new Exception(string.Format("Error adding label '{0}'.", labelName));
            return index;
        }
    }
}
