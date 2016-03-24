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
    internal class ParserState
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

        private void SetLabelAddressReferences()
        {
            foreach (ushort index in Labels.Keys)
            {
                string labelName = Labels[index].ToLowerInvariant();
                if (Scopes.ContainsLabel(labelName, index))
                {
                    ushort address = (ushort)Scopes.LabelAddress(labelName, index);
                    Code[index] = (byte)(address & 0x00ff);
                    Code[index + 1] = (byte)((address & 0xff00) >> 8);
                    return;
                }
            }
        }

        private void SetDataFieldLabelAddressReferences()
        {
            foreach (ushort key in DataFields.Keys)
            {
                string labelName = DataFields[key].ToLowerInvariant();

                if (Scopes.ContainsLabel(labelName, key) != true)
                {
                    throw new Exception($"Unknown label '{labelName}' referenced in data field");
                }

                ushort address = (ushort)Scopes.LabelAddress(labelName, key);
                Code[key] = (byte)(address & 0x00ff);
                Code[key + 1] = (byte)((address & 0xff00) >> 8);
            }
        }

        private void SetBranchLabelAddressReferences()
        {
            foreach (ushort index in Branches.Keys)
            {
                string labelName = Branches[index];

                if (!Scopes.ContainsLabel(labelName, index))
                {
                    throw new Exception($"Unknown label reference '{labelName}'.");
                }

                ushort label_address = (ushort)Scopes.LabelAddress(labelName, index);
                int delta = label_address - index;
                if ((delta & 0x00000001) != 00)
                    throw new Exception($"Branch to label '{labelName}' is not word aligned.");
                delta /= 2;
                if ((delta > sbyte.MaxValue) || (delta < sbyte.MinValue))
                    throw new Exception("Branch operation out of range.");
                Code[index + 1] = (byte)((sbyte)delta);
            }
        }
    }
}
