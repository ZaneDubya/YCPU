using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Emulation
{
    public interface ITexture
    {
        int Width { get; }
        int Height { get; }
        int DeviceBusIndex { get; set;  }
    }
}
