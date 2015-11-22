using System;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Entities;

namespace Ypsilon.Modes.Landed
{
    class LandedModel : AModel
    {
        public Spob LandedOn
        {
            get;
            private set;
        }

        public LandedModel(Spob landedOn)
        {
            LandedOn = landedOn;
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
