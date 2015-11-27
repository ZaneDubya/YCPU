using Ypsilon.Data;
using Ypsilon.Data.Materials;

namespace Ypsilon.Scripts.Items.MaterialItems
{
    class CopperItem : AMaterialItem
    {
        public override MaterialInfo MaterialInfo { get { return MaterialInfos.Copper; } }
        public override string DefaultName { get { return "Copper"; } }

        public CopperItem()
        {

        }

        static CopperItem()
        {
            Prices.AddPrice(typeof(CopperItem), 150);
        }
    }
}
