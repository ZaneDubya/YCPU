using Ypsilon.Core.Input;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Core.Windows;
using Ypsilon.Modes.Space;
using Ypsilon.Entities;

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
                else if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Escape, false, false, false))
                {
                    ModeManager modes = ServiceRegistry.GetService<ModeManager>();
                    modes.QueuedModel = new SpaceModel();
                }
            }
            else if (view.State == LandedState.Exchange)
            {
                Ship player = (Ship)World.Entities.GetPlayerEntity();
                

                if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Escape, false, false, false))
                {
                    view.State = LandedState.Overview;
                }
                else if (input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.Tab, false, false, false))
                {
                    view.SelectSecond = !view.SelectSecond;
                }
                else if (input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.W, false, false, false))
                {
                    view.SelectIndex--;
                    if (view.SelectIndex < 0)
                    {
                        view.SelectIndex = 0;
                        view.SelectScrollOffset--;
                        if (view.SelectScrollOffset < 0)
                            view.SelectScrollOffset = 0;
                    }
                }
                else if (input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.S, false, false, false))
                {
                    view.SelectIndex++;
                    if (view.SelectIndex > 7)
                    {
                        view.SelectIndex = 7;
                        view.SelectScrollOffset++;
                        int maxScrollOffset = Data.CommodityList.List.Count - 8;
                        if (view.SelectScrollOffset > maxScrollOffset)
                            view.SelectScrollOffset = maxScrollOffset;
                    }
                }
            }
        }
    }
}
