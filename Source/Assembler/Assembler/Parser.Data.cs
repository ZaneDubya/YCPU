using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Assembler
{
    partial class Parser
    {
        protected void SetDataFieldLabelAddressReferences()
        {
            foreach (ushort key in m_LabelDataFieldReferences.Keys)
            {
                string labelName = m_LabelDataFieldReferences[key];

                if (m_Scopes.ContainsLabel(labelName, key) != true)
                {
                    throw new Exception(string.Format("Unknown label '{0}' referenced in data field", labelName));
                }

                ushort address = (ushort)m_Scopes.LabelAddress(labelName, key);
                m_MachineCodeOutput[key] = (byte)(address & 0x00ff);
                m_MachineCodeOutput[key + 1] = (byte)((address & 0xff00) >> 8);
            }
        }
    }
}
