using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Emulation.Hardware
{
    partial class YCPU
    {
        private enum Segments
        {
            CS = 0,
            DS = 1,
            ES = 2,
            SS = 3,
            IS = 4,
            USER = 8
        }
    }
}
