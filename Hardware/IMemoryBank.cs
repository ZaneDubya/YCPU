using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU
{
    /// <summary>
    /// Standard interface to a 4 kiloword memory bank.
    /// </summary>
    public interface IMemoryBank
    {
        /// <summary>
        /// If ReadOnly is true, writes to this bank should fail silently. This does not effect the MMU's 'write protection' bit.
        /// </summary>
        bool ReadOnly
        {
            get; set;
        }

        /// <summary>
        /// Returns the word of memory at this address.
        /// </summary>
        /// <returns></returns>
        ushort this[int address]
        {
            get; set;
        }
    }
}
