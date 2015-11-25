using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Data.Materials;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items.MaterialItems
{
    class SilicateOreItem : AMaterialItem
    {
        public override MaterialInfo MaterialInfo { get { return MaterialInfos.Silicate; } }
        public override string DefaultName { get { return "Silicate Ore"; } }
        public override int Price { get { return 100; } }

        public SilicateOreItem()
        {

        }
    }
}
