using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items
{
    class Industrial : AItem
    {
        public override string DefaultName { get { return "Industrial"; } }
        public override int Price { get { return 100; } }
    }
}
