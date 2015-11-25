using Microsoft.Xna.Framework;
using Ypsilon.Core.Input;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Core.Windows;
using Ypsilon.Entities;
using Ypsilon.Modes.Space.Entities;
using Ypsilon.Modes.Space.Entities.ShipActions;
using Ypsilon.Modes.Space.Input;
using System.Collections.Generic;
using Ypsilon.Modes.Landed;

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
            Ship player = (Ship)World.Entities.GetPlayerEntity();
            ShipSpaceComponent playerComponent = player.GetComponent<ShipSpaceComponent>();

            // Left-down to select.
            if (input.HandleMouseEvent(MouseEvent.Down, MouseButton.Left))
            {
                if (MouseOverList.HasEntities)
                {
                    Model.SelectedSerial = MouseOverList.GetNextSelectableEntity(Model.SelectedSerial).Serial;
                }
            }

            // T is for TARGET
            if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.T, false, false, false))
            {
                AEntity target = GetClosestEntity(playerComponent.Position);
                if (target != null)
                {
                    Model.SelectedSerial = target.Serial;
                }
            }

            // L is for LAND
            if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.L, false, false, false))
            {
                AEntity selected = World.Entities.GetEntity<AEntity>(Model.SelectedSerial, false);
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
                    modes.QueuedModel = new LandedModel(selected as Spob);
                }
            }

                // M is for MINING
            if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.M, false, false, false))
            {
                if (playerComponent.Action is MiningAction)
                {
                    // stop mining
                    playerComponent.Action = new NoAction(player);
                    Messages.Add(MessageType.Error, "Mining halted.");
                }
                else
                {
                    // start mining
                    AEntity selected = World.Entities.GetEntity<AEntity>(Model.SelectedSerial, false);
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

        private AEntity GetClosestEntity(Position3D position)
        {
            Vector3 aPos = position.ToVector3();

            AEntity nearest = null;
            float distance = float.MaxValue;

            Dictionary<int, AEntity> entites = World.Entities.AllEntities;

            foreach (AEntity e in entites.Values)
            {
                if (e.IsDisposed || e.IsPlayerEntity)
                    continue;
                Position3D ePos = e.GetComponent<AEntitySpaceComponent>().Position;
                float eDist = Vector3.Distance(ePos.ToVector3(), aPos);
                if (eDist < distance)
                {
                    distance = eDist;
                    nearest = e;
                }
            }

            return nearest;
        }
    }
}
