using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Modes.Space.Entities.ShipActions
{
    class LandedAction : AAction
    {
        public Spob LandedAt = null;

        public LandedAction(Ship parent, Spob landedAt)
            : base(parent)
        {
            LandedAt = landedAt;
        }
    }
}
