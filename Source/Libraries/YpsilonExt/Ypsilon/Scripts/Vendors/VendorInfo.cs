using System.Collections.Generic;
using Ypsilon.Entities;
using Ypsilon.Scripts.Items;
using Ypsilon.Scripts.Items.MaterialItems;

namespace Ypsilon.Scripts.Vendors
{
    class VendorInfo
    {
        public BuyInfo Purchasing { get; protected set; }
        public List<SellInfo> Selling { get; protected set; }

        /// <summary>
        /// A generic info for a vendor. Filled with generic values.
        /// </summary>
        public VendorInfo()
        {
            Purchasing = new BuyInfo();
            Purchasing.Add(typeof(Chocolate), 1f);
            Purchasing.Add(typeof(Food), 1f);
            Purchasing.Add(typeof(Industrial), 1f);
            Purchasing.Add(typeof(LuxuryGoods), 1f);
            Purchasing.Add(typeof(MedicalSupplies), 1f);
            Purchasing.Add(typeof(SilicateOreItem), 1f);
            Purchasing.Add(typeof(CarbonateOreItem), 1f);
            Purchasing.Add(typeof(CopperItem), 1f);
            Purchasing.Add(typeof(IronItem), 1f);

            Selling = new List<SellInfo>();
            Selling.Add(new SellInfo("Chocolate", 1f, 100, typeof(Chocolate), null));
            Selling.Add(new SellInfo("Food", 1f, 100, typeof(Food), null));
            Selling.Add(new SellInfo("Industrial", 1f, 100, typeof(Industrial), null));
            Selling.Add(new SellInfo("Luxury Goods", 1f, 100, typeof(LuxuryGoods), null));
            Selling.Add(new SellInfo("Medical Supplies", 1f, 100, typeof(MedicalSupplies), null));
            Selling.Add(new SellInfo("Silicate Ore", 1f, 100, typeof(SilicateOreItem), null));
            Selling.Add(new SellInfo("Carbonate Ore", 1f, 100, typeof(CarbonateOreItem), null));
            Selling.Add(new SellInfo("Copper", 1f, 100, typeof(CopperItem), null));
            Selling.Add(new SellInfo("Iron", 1f, 100, typeof(IronItem), null));
        }

        public BuyInfo GetBuyInfoLimitedToSellerInventory(ItemList inventory)
        {
            BuyInfo limited = new BuyInfo();
            for (int i = 0; i < inventory.ItemCount; i++)
            {
                if (Purchasing.WillPurchase(inventory[i]))
                {
                    float diff;
                    if (Purchasing.GetPurchaseInfo(inventory[i], out diff))
                    {
                        limited.Add(inventory[i].GetType(), diff);
                    }
                }
            }
            return limited;
        }
    }
}
