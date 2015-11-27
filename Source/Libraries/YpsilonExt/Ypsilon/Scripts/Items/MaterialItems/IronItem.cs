using Ypsilon.Data;
using Ypsilon.Data.Materials;

namespace Ypsilon.Scripts.Items.MaterialItems
{
    class IronItem : AMaterialItem
    {
        public override MaterialInfo MaterialInfo { get { return MaterialInfos.Iron; } }
        public override string DefaultName { get { return "Iron"; } }

        public IronItem()
        {

        }

        static IronItem()
        {
            Prices.AddPrice(typeof(IronItem), 150);
        }
    }
}
