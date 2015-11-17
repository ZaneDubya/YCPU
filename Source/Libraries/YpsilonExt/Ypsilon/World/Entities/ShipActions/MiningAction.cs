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
            if (Target == null)
                Parent.Action = new NoAction(Parent);

            base.Update(frameSeconds);
        }
    }
}
