using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.World.Entities.ShipActions
{
    class MiningAction : AAction
    {
        public AEntity Target = null;

        public MiningAction(Ship parent, AEntity target)
            : base(parent)
        {
            Target = target;
        }

        public override void Update(float frameSeconds)
        {
            if (Target == null || !(Target is Spob))
            {
                Parent.Action = new NoAction(Parent);
                return;
            }

            float amount = (Target as Spob).ExtractOre(frameSeconds);
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
