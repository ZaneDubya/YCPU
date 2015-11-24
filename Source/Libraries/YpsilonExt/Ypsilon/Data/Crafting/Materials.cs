using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Data.Crafting
{
    class Materials
    {
        public static Material Copper, Silver, Gold, Iron, Aluminium;

        static Materials()
        {

            Copper = new Material("Copper", MaterialIndex.Copper);
        }
    }
}
