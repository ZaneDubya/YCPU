using System;
using Ypsilon.Core.Patterns.MVC;

namespace Ypsilon.Modes.Landed
{
    class LandedModel : AModel
    {
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
