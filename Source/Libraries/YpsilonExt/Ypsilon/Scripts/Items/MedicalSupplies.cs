using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items
{
    class MedicalSupplies : AItem
    {
        public override string DefaultName { get { return "Medical Supplies"; } }
        public override int Price { get { return 100; } }
    }
}
