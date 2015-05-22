using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon
{
    public interface IRenderer
    {
        void RenderLEM(IMemoryBank bank, uint[] chr, uint[] pal);
    }
}
