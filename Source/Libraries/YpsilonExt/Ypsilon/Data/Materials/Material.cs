using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Materials
{
    class Material
    {
        public MaterialIndex Index;

        public Material(MaterialIndex index, params MaterialQuality[] qualities)
        {
            Index = index;
        }

        /*public int ColdResistance;
        public int Conductivity
        public int DecayResistance
        public int EntangleResistance
        public int Flavor
        public int HeatResistance
        public int Malleability
        public int OverallQuality
        public int PotentialEnergy
        public int ShockResistance
        public int UnitToughness*/
    }
}
