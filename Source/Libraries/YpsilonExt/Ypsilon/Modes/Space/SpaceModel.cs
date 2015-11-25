using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Entities;

namespace Ypsilon.Modes.Space
{
    class SpaceModel : AModel
    {
        public Serial SelectedSerial
        {
            get;
            set;
        }

        public SpaceModel()
        {
            SelectedSerial = Serial.Null;
        }

        public override void Initialize()
        {

        }

        public override void Dispose()
        {

        }

        public override void Update(float totalSeconds, float frameSeconds)
        {
            
        }

        protected override AController CreateController()
        {
            return new SpaceController(this);
        }

        protected override AView CreateView()
        {
            return new SpaceView(this);
        }
    }
}
