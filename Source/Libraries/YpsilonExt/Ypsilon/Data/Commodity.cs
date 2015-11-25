using System;
using System.Collections.Generic;
using System.Linq;
using Ypsilon.Entities;
using System.Text;

namespace Ypsilon.Data
{
    class Commodity
    {
        public Item Item;
        public string Name;
        public int Amount;
        public int Value;

        public Commodity(string name, int amount, int value)
        {
            Name = name;
            Amount = amount;
            Value = value;
        }
    }
}
