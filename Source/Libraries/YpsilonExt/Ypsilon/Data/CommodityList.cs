using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Entities;

namespace Ypsilon.Data
{
    class CommodityList
    {
        private List<Commodity> m_Commodities;

        public int Count
        {
            get
            {
                return m_Commodities == null ? 0 : m_Commodities.Count;
            }
        }

        public Commodity this[int index]
        {
            get
            {
                if (m_Commodities == null)
                    return null;
                if (index < 0 || index >= m_Commodities.Count)
                    return null;
                return m_Commodities[index];

            }
        }

        private CommodityList()
        {
            m_Commodities = new List<Commodity>();
        }

        /// <summary>
        /// Generates a list of all commodities available for purchase, and market rates for the same.
        /// </summary>
        /// <param name="spob"></param>
        public CommodityList(Spob spob)
            : this()
        {
            m_Commodities.Add(new Commodity("Food", 100, 75));
            m_Commodities.Add(new Commodity("Industrial", 100, 350));
            m_Commodities.Add(new Commodity("Medical Supplies", 100, 750));
            m_Commodities.Add(new Commodity("Luxury Goods", 100, 900));
            m_Commodities.Add(new Commodity("Metal", 100, 200));
            m_Commodities.Add(new Commodity("Equipment", 100, 300));
            m_Commodities.Add(new Commodity("Cookies", 100, 600));
            m_Commodities.Add(new Commodity("Lizard Pelts", 100, 300));
        }

        /// <summary>
        /// Generates a list of all commodities in a ship's holds, with prices equal to selling price at a spob.
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="landedAt"></param>
        public CommodityList(Ship ship, Spob spob)
        {

        }

        /// <summary>
        /// Generates a list of all commodities in a ship. Does not calculate prices.
        /// </summary>
        /// <param name="ship"></param>
        public CommodityList(Ship ship)
        {

        }
    }
}
