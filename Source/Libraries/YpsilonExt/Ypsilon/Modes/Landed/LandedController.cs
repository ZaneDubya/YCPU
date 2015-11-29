using System;
using Ypsilon.Core.Input;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Core.Windows;
using Ypsilon.Entities;
using Ypsilon.Modes.Space;
using Ypsilon.Scripts.Vendors;

namespace Ypsilon.Modes.Landed
{
    class LandedController : AController
    {
        protected new LandedModel Model
        {
            get { return (LandedModel)base.Model; }
        }

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
                    modes.QueuedModel = new SpaceModel(Model.World);
                }
            }
            else if (view.State == LandedState.Exchange)
            {
                Ship player = (Ship)Model.World.Entities.GetPlayerEntity();
                VendorInfo vendor = Model.LandedOn.Exchange;

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
                    int maxSelection = view.SelectSecond ? vendor.Selling.Count : Model.BuyInfo.Types.Length;
                    view.SelectIndex++;
                    if (view.SelectIndex >= maxSelection)
                        view.SelectIndex = maxSelection - 1;
                    if (view.SelectIndex > 7)
                    {
                        view.SelectIndex = 7;
                        view.SelectScrollOffset++;
                        int maxScrollOffset = maxSelection - 8;
                        if (maxScrollOffset < 0)
                            maxScrollOffset = 0;
                        if (view.SelectScrollOffset > maxScrollOffset)
                            view.SelectScrollOffset = maxScrollOffset;
                    }
                }
                else if (input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.Enter, false, false, false))
                {
                    if (view.SelectSecond) // buying
                    {
                        // do we have enough credits? Buy and reduce credits.
                        SellInfo info = vendor.Selling[view.SelectIndex + view.SelectScrollOffset];
                        int price = info.Price;
                        int amount = 1;

                        if (Model.World.PlayerCredits >= price)
                        {
                            if (player.Inventory.TryAddItem(info.Type, amount))
                            {
                                Model.World.PlayerCredits -= price;
                                AItem item;
                                if (!player.Inventory.TryGetItem(info.Type, out item))
                                {
                                    throw new Exception("LandedController.Update() - could not retrieve newly purchased item.");
                                }
                                if (item.Amount == amount)
                                {
                                    // newly purchased item - need to refresh the sell list.
                                    Model.BuyInfo = vendor.GetBuyInfoLimitedToSellerInventory(player.Inventory);
                                }
                            }
                            else
                            {
                                // error message: not enough hold space.
                            }
                        }
                        else
                        {
                            // error message: not enough credits.
                        }
                    }
                    else // selling - add credits!
                    {
                        // get the item from the player - we will be decrementing this amount.
                        Type itemType = Model.BuyInfo.Types[view.SelectIndex + view.SelectScrollOffset];
                        AItem item;
                        if (!player.Inventory.TryGetItem(itemType, out item)) // this will always succeed.
                        {
                            throw new Exception("LandedController.Update() - could not retrieve item for sale.");
                        }

                        // if we were tracking amounts, we would want to increment the amount in the sell info...
                        // SellInfo info = vendor.GetSellInfoByItemType(itemType);
                        int price = Model.BuyInfo.GetPurchasePrice(item);
                        Model.World.PlayerCredits += price;
                        item.Amount -= 1;
                        if (item.Amount == 0)
                        {
                            // item was disposed, need a new sell list.
                            Model.BuyInfo = vendor.GetBuyInfoLimitedToSellerInventory(player.Inventory);
                        }
                    }
                }

                if ((Model.BuyInfo.Types.Length == 0) && (vendor.Selling.Count == 0))
                {
                    // !!! show an error message - the exchange is closed.
                    view.State = LandedState.Overview;
                }
                if (Model.BuyInfo.Types.Length == 0)
                {
                    view.SelectSecond = true;
                }
                else if (vendor.Selling.Count == 0)
                {
                    view.SelectSecond = false;
                }
            }
        }
    }
}
