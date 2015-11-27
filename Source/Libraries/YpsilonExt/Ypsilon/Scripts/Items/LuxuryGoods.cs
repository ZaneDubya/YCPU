using Ypsilon.Data;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items
{
    class LuxuryGoods : AItem
    {
        public override string DefaultName { get { return "Luxury Goods"; } }

        static LuxuryGoods()
        {
            Prices.AddPrice(typeof(LuxuryGoods), 150);
        }
    }
}
