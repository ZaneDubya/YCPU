using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon
{
    public interface IServiceRegistry
    {
        T Register<T>(T service);
        void Unregister<T>();

        bool ServiceExists<T>();
        T GetService<T>();
    }
}
