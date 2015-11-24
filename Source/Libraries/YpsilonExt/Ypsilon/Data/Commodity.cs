using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Data
{
    class Commodity
    {
        public string Name;
        public int Cost;

        public Commodity(string name, int cost)
        {
            Name = name;
            Cost = cost;
        }
    }
}
