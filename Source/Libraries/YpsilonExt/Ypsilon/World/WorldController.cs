using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Core.Input;
using Ypsilon.Core.Patterns.MVC;
using Microsoft.Xna.Framework;
using Ypsilon.World.Entities;
using Ypsilon.Core.Windows;

namespace Ypsilon.World
{
    class WorldController : AController
    {
        protected new WorldModel Model
        {
            get { return (WorldModel)base.Model; }
        }

        public WorldController(WorldModel model)
            : base(model)
        {

        }

        public override void ReceiveInput(float frameSeconds, InputManager input)
        {
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
        }
    }
}
