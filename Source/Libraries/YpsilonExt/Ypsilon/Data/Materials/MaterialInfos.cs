using Ypsilon.Scripts.Items.MaterialItems;

namespace Ypsilon.Data.Materials
{
    class MaterialInfos
    {
        public static MaterialInfo Copper, Iron;
        public static MaterialInfo Carbonate, Silicate; 

        static MaterialInfos()
        {
            Copper = new MaterialInfo(MaterialIndex.Copper,  typeof(CopperItem), MaterialQuality.IsMineral, MaterialQuality.IsMetal);
            Iron = new MaterialInfo(MaterialIndex.Copper, typeof(IronItem), MaterialQuality.IsMineral, MaterialQuality.IsMetal);

            Carbonate = new MaterialInfo(MaterialIndex.Copper, typeof(CarbonateOreItem), MaterialQuality.IsMineral, MaterialQuality.IsOre);
            Silicate = new MaterialInfo(MaterialIndex.Copper, typeof(SilicateOreItem), MaterialQuality.IsMineral, MaterialQuality.IsOre);
        }
    }
}
