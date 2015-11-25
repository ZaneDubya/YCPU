using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Scripts.Items;
using Ypsilon.Scripts.Items.MaterialItems;

namespace Ypsilon.Scripts.Vendors
{
    class VendorInfo
    {
        /// <summary>
        /// A generic info for a vendor. Filled with generic values.
        /// </summary>
        public VendorInfo()
        {
            WillPurchase = new BuyInfo();
            WillPurchase.Add(typeof(Chocolate), 1f);
            WillPurchase.Add(typeof(Food), 1f);
            WillPurchase.Add(typeof(Industrial), 1f);
            WillPurchase.Add(typeof(LuxuryGoods), 1f);
            WillPurchase.Add(typeof(MedicalSupplies), 1f);
            WillPurchase.Add(typeof(SilicateOreItem), 1f);
            WillPurchase.Add(typeof(CarbonateOreItem), 1f);
            WillPurchase.Add(typeof(CopperItem), 1f);
            WillPurchase.Add(typeof(IronItem), 1f);

            WillSell = new List<SellInfo>();
            WillSell.Add(new SellInfo("Chocolate", 1f, 100, typeof(Chocolate), null));
            WillSell.Add(new SellInfo("Food", 1f, 100, typeof(Food), null));
            WillSell.Add(new SellInfo("Industrial", 1f, 100, typeof(Industrial), null));
            WillSell.Add(new SellInfo("Luxury Goods", 1f, 100, typeof(LuxuryGoods), null));
            WillSell.Add(new SellInfo("Medical Supplies", 1f, 100, typeof(MedicalSupplies), null));
            WillSell.Add(new SellInfo("Silicate Ore", 1f, 100, typeof(SilicateOreItem), null));
            WillSell.Add(new SellInfo("Carbonate Ore", 1f, 100, typeof(CarbonateOreItem), null));
            WillSell.Add(new SellInfo("Copper", 1f, 100, typeof(CopperItem), null));
            WillSell.Add(new SellInfo("Iron", 1f, 100, typeof(IronItem), null));
        }

        public BuyInfo WillPurchase { get; protected set; }
        public List<SellInfo> WillSell { get; protected set; }
    }
}
