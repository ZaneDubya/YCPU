using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Data.Materials;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items.MaterialItems
{
    abstract class AMaterialItem : AItem
    {
        public abstract MaterialInfo MaterialInfo { get; }
    }
}
