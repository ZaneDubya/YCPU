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
            ShipComponent playerComponent = Parent.GetComponent<ShipComponent>();
            SpobComponent targetComponent;

            // make sure we have a target.
            if (Target == null)
            {
                playerComponent.Action = new NoAction(Parent);
                return;
            }
            else
            {
                targetComponent = Target.GetComponent<SpobComponent>();
                if (targetComponent == null)
                {
                    playerComponent.Action = new NoAction(Parent);
                    return;
                }
            }

            // error conditions.
            float maxDistance = playerComponent.DrawSize + targetComponent.DrawSize;

            if (Position3D.Distance(Parent.Position, Target.Position) > maxDistance)
            {
                Messages.Add(MessageType.Error, "Mining halted. Too far away to mine.");
            }
            else if (Parent.Throttle > 0)
            {
                Messages.Add(MessageType.Error, "Mining halted. Cannot mine while moving.");
                playerComponent.Action = new NoAction(Parent);
                return;
            }

            // stop mining if the target has run out of resources.
            if (Target.ResourceOre == 0)
            {
                m_MinedAmount = 0;
                Messages.Add(MessageType.Error, "Target resources exhausted.");
                playerComponent.Action = new NoAction(Parent);
                return;
            }

            // ok, mine!
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

            base.Update(frameSeconds);
        }
    }
}
