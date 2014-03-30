using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU
{
    public interface IMemoryBank
    {
        bool ReadOnly
        {
            get; set;
        }

        ushort this[int i]
        {
            get; set;
        }
    }
}
