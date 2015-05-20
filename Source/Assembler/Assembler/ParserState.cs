using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Assembler
{
    class ParserState
    {
        public Dictionary<ushort, string> Branches;
        public Dictionary<ushort, string> Labels;
        public Dictionary<ushort, string> DataFields;
        public Scopes m_Scopes;

        public List<byte> machineCode = new List<byte>();

        public string m_Directory;

        public ParserState()
        {
            Branches = new Dictionary<ushort, string>();
            Labels = new Dictionary<ushort, string>();
            DataFields = new Dictionary<ushort, string>();
            m_Scopes = new Scopes();

        }
    }
}
