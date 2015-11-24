using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Data
{
    class Commodities
    {
        public static List<Commodity> List;

        static Commodities()
        {
            List = new List<Commodity>();
            List.Add(new Commodity("Food", 75));
            List.Add(new Commodity("Industrial", 350));
            List.Add(new Commodity("Medical Supplies", 750));
            List.Add(new Commodity("Luxury Goods", 900));
            List.Add(new Commodity("Metal", 200));
            List.Add(new Commodity("Equipment", 300));
            List.Add(new Commodity("Cookies", 600));
            List.Add(new Commodity("Lizard Pelts", 300));
        }
    }
}
