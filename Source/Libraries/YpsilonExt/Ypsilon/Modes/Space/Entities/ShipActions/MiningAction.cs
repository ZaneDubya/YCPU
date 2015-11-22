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
            ShipSpaceComponent playerComponent = Parent.GetComponent<ShipSpaceComponent>();
            SpobSpaceComponent targetComponent;

            // make sure we have a target.

            if (Target == null)
            {
                playerComponent.Action = new NoAction(Parent);
                return;
            }
            else
            {
                targetComponent = Target.GetComponent<SpobSpaceComponent>();
                if (targetComponent == null)
                {
                    playerComponent.Action = new NoAction(Parent);
                    return;
                }
            }

            // error conditions.

            float maxDistance = playerComponent.ViewSize + targetComponent.ViewSize;

            if (Position3D.Distance(playerComponent.Position, targetComponent.Position) > maxDistance)
            {
                Messages.Add(MessageType.Error, "Mining halted. Too far away to mine.");
            }
            else if (playerComponent.Speed > Constants.MaxMiningSpeed)
            {
                Messages.Add(MessageType.Error, "Mining halted. Moving too fast to mine.");
                playerComponent.Action = new NoAction(Parent);
                return;
            }

            // mine, unless target has run out of resources...

            float amount = Target.ExtractOre(frameSeconds);
            if (amount == 0)
            {
                Messages.Add(MessageType.Error, "Target resources exhausted.");
                playerComponent.Action = new NoAction(Parent);
                return;
            }

            Parent.ResourceOre += amount;

            base.Update(frameSeconds);
        }
    }
}
