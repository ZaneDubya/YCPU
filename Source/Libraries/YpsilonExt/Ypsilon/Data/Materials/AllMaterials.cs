using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Materials
{
    class AllMaterials
    {
        public static Material Copper, Silver, Gold, Iron, Aluminium;

        static AllMaterials()
        {
            Copper = new Material(MaterialIndex.Copper, MaterialQuality.IsMineral, MaterialQuality.IsMetal);
        }
    }
}
