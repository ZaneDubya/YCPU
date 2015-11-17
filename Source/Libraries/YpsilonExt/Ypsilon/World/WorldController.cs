using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Core.Input;
using Ypsilon.Core.Patterns.MVC;
using Microsoft.Xna.Framework;
using Ypsilon.World.Entities;
using Ypsilon.Core.Windows;
using Ypsilon.World.Input;
using Ypsilon.Core.Graphics;

namespace Ypsilon.World
{
    class WorldController : AController
    {
        protected new WorldModel Model
        {
            get { return (WorldModel)base.Model; }
        }

        public MouseOverList MouseOverList
        {
            get;
            private set;
        }

        public WorldController(WorldModel model)
            : base(model)
        {
            MouseOverList = new MouseOverList();
        }

        public override void Update(float frameSeconds, InputManager input)
        {
            if (input.HandleMouseEvent(MouseEvent.Down, MouseButton.Left))
            {
                if (MouseOverList.HasEntities)
                {
                    State.Vars.SelectedSerial = MouseOverList.GetNextSelectableEntity(State.Vars.SelectedSerial).Serial;
                }
            }

            Ship player = (Ship)Model.Entities.GetPlayerEntity();
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

            player.Rotator.Rotate(0f, leftrightRotation, frameSeconds);
            player.Throttle = player.Throttle + acceleration * frameSeconds;

            MouseOverList.ScreenMousePosition = new Vector3(input.MousePosition.X, input.MousePosition.Y, 0);
        }
    }
}
