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
    class ParserState
    {
        public Parser Parser;
        public Dictionary<ushort, string> Branches { get; private set; }
        public Dictionary<ushort, string> Labels { get; private set; }
        public Dictionary<ushort, string> DataFields { get; private set; }
        public Scopes Scopes { get; private set; }
        public List<byte> Code { get; private set; }

        public string WorkingDirectory;

        public ParserState(Parser parser)
        {
            Parser = parser;
            Branches = new Dictionary<ushort, string>();
            Labels = new Dictionary<ushort, string>();
            DataFields = new Dictionary<ushort, string>();
            Scopes = new Scopes();
            Code = new List<byte>();
        }

        public void UpdateLabelReferences()
        {
            SetLabelAddressReferences();
            SetBranchLabelAddressReferences();
            SetDataFieldLabelAddressReferences();
        }

        void SetLabelAddressReferences()
        {
            foreach (ushort index in this.Labels.Keys)
            {
                string labelName = this.Labels[index].ToLowerInvariant();

                if (!this.Scopes.ContainsLabel(labelName, index))
                {
                    throw new Exception(string.Format("Unknown label reference '{0}'", labelName));
                }

                ushort address = (ushort)this.Scopes.LabelAddress(labelName, index);
                this.Code[index] = (byte)(address & 0x00ff);
                this.Code[index + 1] = (byte)((address & 0xff00) >> 8);
            }
        }

        void SetDataFieldLabelAddressReferences()
        {
            foreach (ushort key in this.DataFields.Keys)
            {
                string labelName = this.DataFields[key].ToLowerInvariant();

                if (this.Scopes.ContainsLabel(labelName, key) != true)
                {
                    throw new Exception(string.Format("Unknown label '{0}' referenced in data field", labelName));
                }

                ushort address = (ushort)this.Scopes.LabelAddress(labelName, key);
                this.Code[key] = (byte)(address & 0x00ff);
                this.Code[key + 1] = (byte)((address & 0xff00) >> 8);
            }
        }

        void SetBranchLabelAddressReferences()
        {
            foreach (ushort index in this.Branches.Keys)
            {
                string labelName = this.Branches[index];

                if (!this.Scopes.ContainsLabel(labelName, index))
                {
                    throw new Exception(string.Format("Unknown label reference '{0}'.", labelName));
                }

                ushort label_address = (ushort)this.Scopes.LabelAddress(labelName, index);
                int delta = label_address - index;
                if ((delta & 0x00000001) != 00)
                    throw new Exception(string.Format("Branch to label '{0}' is not word aligned.", labelName));
                delta /= 2;
                if ((delta > sbyte.MaxValue) || (delta < sbyte.MinValue))
                    throw new Exception("Branch operation out of range.");
                this.Code[index + 1] = (byte)((sbyte)delta);
            }
        }
    }
}
