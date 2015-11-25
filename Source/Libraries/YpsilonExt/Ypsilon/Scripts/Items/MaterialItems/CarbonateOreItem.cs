using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Data.Materials;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items.MaterialItems
{
    class CarbonateOreItem : AMaterialItem
    {
        public override MaterialInfo MaterialInfo { get { return MaterialInfos.Carbonate; } }
        public override string DefaultName { get { return "Carbonate Ore"; } }
        public override int Price { get { return 100; } }

        public CarbonateOreItem()
        {

        }
    }
}
