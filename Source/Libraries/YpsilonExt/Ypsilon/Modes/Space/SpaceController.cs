using Microsoft.Xna.Framework;
using Ypsilon.Core.Input;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Core.Windows;
using Ypsilon.Entities;
using Ypsilon.Modes.Space.Entities;
using Ypsilon.Modes.Space.Entities.ShipActions;
using Ypsilon.Modes.Space.Input;

namespace Ypsilon.Modes.Space
{
    class SpaceController : AController
    {
        protected new SpaceModel Model
        {
            get { return (SpaceModel)base.Model; }
        }

        public MouseOverList MouseOverList
        {
            get;
            private set;
        }

        public SpaceController(SpaceModel model)
            : base(model)
        {
            MouseOverList = new MouseOverList();
        }

        public override void Update(float frameSeconds, InputManager input)
        {
            // get the player object.
            Ship player = (Ship)Model.Entities.GetPlayerEntity();
            ShipSpaceComponent playerComponent = player.GetComponent<ShipSpaceComponent>();

            // Left-down to select.
            if (input.HandleMouseEvent(MouseEvent.Down, MouseButton.Left))
            {
                if (MouseOverList.HasEntities)
                {
                    Model.SelectedSerial = MouseOverList.GetNextSelectableEntity(Model.SelectedSerial).Serial;
                }
            }

            // L is for LAND
            if (input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.L, false, false, false))
            {
                AEntity selected = Model.Entities.GetEntity<AEntity>(Model.SelectedSerial, false);
                if (selected == null)
                {
                    // do nothing? error noise?
                }
                else if (!(selected is Spob))
                {
                    // do nothing? error noise?
                }
                else if (!(selected as Spob).CanLandHere)
                {
                    Messages.Add(MessageType.Error, "Cannot land on target.");
                }
                else if (playerComponent.Speed > Constants.MaxLandingSpeed)
                {
                    Messages.Add(MessageType.Error, "Landing cancelled. Moving too quickly to land.");
                }
                else
                {
                    ModeManager modes = ServiceRegistry.GetService<ModeManager>();
                    modes.QueuedModel = new Landed.LandedModel();
                }
            }

                // M is for MINING
            if (input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.M, false, false, false))
            {
                if (playerComponent.Action is MiningAction)
                {
                    // stop mining
                    playerComponent.Action = new NoAction(player);
                }
                else
                {
                    // start mining
                    AEntity selected = Model.Entities.GetEntity<AEntity>(Model.SelectedSerial, false);
                    if (selected == null)
                    {
                        // do nothing? error noise?
                    }
                    else if (!(selected is Spob))
                    {
                        Messages.Add(MessageType.Error, "Cannot mine this object.");
                    }
                    else
                    {
                        AEntitySpaceComponent selectedComponent = selected.GetComponent<AEntitySpaceComponent>();

                        float maxDistance = playerComponent.ViewSize + selectedComponent.ViewSize;

                        if (Position3D.Distance(playerComponent.Position, selectedComponent.Position) > maxDistance)
                        {
                            Messages.Add(MessageType.Error, "Too far away to mine. Close distance.");
                        }
                        else if (playerComponent.Speed > Constants.MaxMiningSpeed)
                        {
                            Messages.Add(MessageType.Error, "Moving too quickly to mine. Reduce velocity.");
                        }
                        else
                        {
                            playerComponent.Action = new MiningAction(player, selected as Spob);
                            Messages.Add(MessageType.Error, "Mining commenced.");
                        }
                    }
                }
            }

            // Move the player object with WASD.
            float acceleration = 0.0f;
            float leftrightRotation = 0.0f;
            if (input.IsKeyDown(WinKeys.Up) || input.IsKeyDown(WinKeys.W))
                acceleration += 1f;
            if (input.IsKeyDown(WinKeys.Down) || input.IsKeyDown(WinKeys.S))
                acceleration = -1f;
            if (input.IsKeyDown(WinKeys.Right) || input.IsKeyDown(WinKeys.D))
                leftrightRotation = -1f;
            if (input.IsKeyDown(WinKeys.Left) || input.IsKeyDown(WinKeys.A))
                leftrightRotation = 1f;
            playerComponent.Rotator.Rotate(0f, leftrightRotation, frameSeconds);
            playerComponent.Throttle = playerComponent.Throttle + acceleration * frameSeconds;

            // update the mouse over list's mouse position.
            MouseOverList.ScreenMousePosition = new Vector3(input.MousePosition.X, input.MousePosition.Y, 0);
        }
    }
}
