using Ypsilon.Data;
using Ypsilon.Data.Materials;

namespace Ypsilon.Scripts.Items.MaterialItems
{
    class SilicateOreItem : AMaterialItem
    {
        public override MaterialInfo MaterialInfo { get { return MaterialInfos.Silicate; } }
        public override string DefaultName { get { return "Silicate Ore"; } }

        public SilicateOreItem()
        {

        }

        static SilicateOreItem()
        {
            Prices.AddPrice(typeof(SilicateOreItem), 150);
        }
    }
}
