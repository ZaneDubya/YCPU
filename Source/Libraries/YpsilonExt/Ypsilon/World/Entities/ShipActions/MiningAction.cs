using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.World.Entities.ShipActions
{
    class MiningAction : AAction
    {
        public Spob Target = null;

        public MiningAction(Ship parent, Spob target)
            : base(parent)
        {
            Target = target;
        }

        public override void Update(float frameSeconds)
        {
            if (Target == null )
            {
                Parent.Action = new NoAction(Parent);
                return;
            }

            float amount = Target.ExtractOre(frameSeconds);
            if (amount == 0)
            {
                Parent.Action = new NoAction(Parent);
                return;
            }

            Parent.ResourceOre += amount;

            base.Update(frameSeconds);
        }
    }
}
