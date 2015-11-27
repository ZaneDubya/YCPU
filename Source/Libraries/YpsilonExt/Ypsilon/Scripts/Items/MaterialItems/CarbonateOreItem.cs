using Ypsilon.Data;
using Ypsilon.Data.Materials;

namespace Ypsilon.Scripts.Items.MaterialItems
{
    class CarbonateOreItem : AMaterialItem
    {
        public override MaterialInfo MaterialInfo { get { return MaterialInfos.Carbonate; } }
        public override string DefaultName { get { return "Carbonate Ore"; } }

        public CarbonateOreItem()
        {

        }

        static CarbonateOreItem()
        {
            Prices.AddPrice(typeof(CarbonateOreItem), 150);
        }
    }
}
