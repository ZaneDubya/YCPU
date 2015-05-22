using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon
{
    public interface IInputProvider
    {
        bool TryGetKeypress(out ushort keycode);
    }
}
