using Microsoft.Xna.Framework;
using System;
using Ypsilon.Entities;

namespace Ypsilon.Data
{
    /// <summary>
    /// A description of a piece of ship equipment.
    /// </summary>
    public class AModuleDefinition
    {
        public virtual Point Size { get { return PointX.OneByOne; } }
        public virtual Type Type { get { return typeof(AModule); } }
    }
}
