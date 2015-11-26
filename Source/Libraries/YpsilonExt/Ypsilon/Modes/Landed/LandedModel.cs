using System;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Entities;
using Ypsilon.Scripts.Vendors;

namespace Ypsilon.Modes.Landed
{
    class LandedModel : AModel
    {
        public Spob LandedOn
        {
            get;
            private set;
        }

        public BuyInfo BuyInfo
        {
            get;
            set;
        }

        public LandedModel(Spob landedOn)
        {
            LandedOn = landedOn;

            Ship player = (Ship)World.Entities.GetPlayerEntity();
            BuyInfo = LandedOn.Exchange.GetBuyInfoLimitedToSellerInventory(player.Inventory);
        }

        public override void Dispose()
        {

        }

        public override void Initialize()
        {

        }

        public override void Update(float totalSeconds, float frameSeconds)
        {

        }

        protected override AController CreateController()
        {
            return new LandedController(this);
        }

        protected override AView CreateView()
        {
            return new LandedView(this);
        }
    }
}
