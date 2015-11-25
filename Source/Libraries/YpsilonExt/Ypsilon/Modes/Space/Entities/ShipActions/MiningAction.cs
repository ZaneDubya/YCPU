using Ypsilon.Entities;
using Ypsilon.Scripts.Items.MaterialItems;
using System;

namespace Ypsilon.Modes.Space.Entities.ShipActions
{
    class MiningAction : AAction
    {
        public Spob Target = null;

        private float m_MinedAmount = 0f;

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
            if (Target.ResourceOre == 0)
            {
                m_MinedAmount = 0;
                Messages.Add(MessageType.Error, "Target resources exhausted.");
                playerComponent.Action = new NoAction(Parent);
                return;
            }
            else
            {
                m_MinedAmount += frameSeconds; // multipled by the power of the mining device and the difficulty of mining the resource?
                if (m_MinedAmount >= 1.0f)
                {
                    int amount = (int)m_MinedAmount;
                    if (Parent.Inventory.TryAddItem(typeof(CarbonateOreItem), amount))
                    {
                        // success!
                        Target.ExtractOre(amount);
                        m_MinedAmount -= amount;
                    }
                    else
                    {
                        // can't add it. Failure!
                        m_MinedAmount = 0;
                        Messages.Add(MessageType.Error, "Could not store extracted ore in hold.");
                        playerComponent.Action = new NoAction(Parent);
                        return;
                    }
                }
            }

            base.Update(frameSeconds);
        }
    }
}
