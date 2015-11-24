using Ypsilon.Core.Input;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Core.Windows;

namespace Ypsilon.Modes.Landed
{
    class LandedController : AController
    {
        public LandedController(LandedModel model)
            : base(model)
        {

        }

        public override void Update(float frameSeconds, InputManager input)
        {
            LandedView view = Model.GetView() as LandedView;

            if (view.State == LandedState.Overview)
            {
                if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.D1, false, false, false))
                {

                }
                else if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.D2, false, false, false))
                {
                    (Model.GetView() as LandedView).State = LandedState.Exchange;
                }
                else if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.D3, false, false, false))
                {

                }
                else if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.D4, false, false, false))
                {

                }
                else if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.D5, false, false, false))
                {
                    ModeManager modes = ServiceRegistry.GetService<ModeManager>();
                    modes.QueuedModel = Persistence.Savegame.Load();
                }
            }
            else if (view.State == LandedState.Exchange)
            {
                if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Escape, false, false, false))
                {
                    view.State = LandedState.Overview;
                }
            }
        }
    }
}
