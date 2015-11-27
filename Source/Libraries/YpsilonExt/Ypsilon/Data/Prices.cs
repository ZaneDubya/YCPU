using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Data
{
    static class Prices
    {
        private static Dictionary<Type, int> m_Prices = new Dictionary<Type, int>();

        public static void AddPrice(Type type, int price)
        {
            if (m_Prices.ContainsKey(type))
                m_Prices[type] = price;
            else
                m_Prices.Add(type, price);
        }

        public static int GetPrice(Type type)
        {
            int price;
            if (m_Prices.TryGetValue(type, out price))
                return price;
            else
            {
                // initialize the class, which should set the price variable.
                Activator.CreateInstance(type);
                if (m_Prices.TryGetValue(type, out price))
                    return price;
                else
                    throw new Exception(string.Format("Unknown price for item of type {0}", type.ToString()));
            }
        }
    }
}
