using Ypsilon.Entities;

namespace Ypsilon.Modes.Space.Entities.ShipActions
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
            ShipSpaceComponent component = Parent.GetComponent<ShipSpaceComponent>();
            if (component == null)
                return;

            if (Target == null )
            {
                component.Action = new NoAction(Parent);
                return;
            }

            float amount = Target.ExtractOre(frameSeconds);
            if (amount == 0)
            {
                component.Action = new NoAction(Parent);
                return;
            }

            Parent.ResourceOre += amount;

            base.Update(frameSeconds);
        }
    }
}
