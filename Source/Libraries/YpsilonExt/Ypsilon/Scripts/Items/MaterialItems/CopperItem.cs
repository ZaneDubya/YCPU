using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Data.Materials;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items.MaterialItems
{
    class CopperItem : AMaterialItem
    {
        public override MaterialInfo MaterialInfo { get { return MaterialInfos.Copper; } }
        public override string DefaultName { get { return "Copper"; } }
        public override int Price { get { return 100; } }

        public CopperItem()
        {

        }
    }
}
