using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Data.Materials;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items.MaterialItems
{
    class IronItem : AMaterialItem
    {
        public override MaterialInfo MaterialInfo { get { return MaterialInfos.Iron; } }
        public override string DefaultName { get { return "Iron"; } }
        public override int Price { get { return 100; } }

        public IronItem()
        {

        }
    }
}
