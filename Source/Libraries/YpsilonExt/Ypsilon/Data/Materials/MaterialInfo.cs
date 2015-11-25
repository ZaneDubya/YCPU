using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Data.Materials
{
    class MaterialInfo
    {
        public MaterialIndex Index;
        public Type ItemType;

        private List<MaterialQuality> m_Qualities;

        public bool HasQuality(MaterialQuality quality)
        {
            if (m_Qualities == null)
                return false;
            if (m_Qualities.Contains(quality))
                return true;
            return false;
        }

        public MaterialInfo(MaterialIndex index, Type itemType, params MaterialQuality[] qualities)
        {
            Index = index;
            ItemType = itemType;
            if (qualities == null || qualities.Length == 0)
                m_Qualities = null;
            else
            {
                m_Qualities = new List<MaterialQuality>();
                for (int i = 0; i < qualities.Length; i++)
                    m_Qualities.Add(qualities[i]);
            }
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
