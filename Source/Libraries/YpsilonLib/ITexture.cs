using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon
{
    public interface ITexture
    {
        int Width { get; }
        int Height { get; }
        int DeviceBusIndex { get; set;  }
    }
}
