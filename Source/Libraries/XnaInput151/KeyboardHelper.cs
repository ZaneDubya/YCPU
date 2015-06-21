using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaInput
{
    public class KeyboardHelper
    {
        public KeyboardState State, PreviousState;

        public void Update()
        {
            PreviousState = State;
            State = Keyboard.GetState();
        }

        public bool JustPressed(Keys key)
        {
            return State.IsKeyDown(key) && PreviousState.IsKeyUp(key);
        }

        public bool JustReleased(Keys key)
        {
            return State.IsKeyUp(key) && PreviousState.IsKeyDown(key);
        }

        public bool Up(Keys key)
        {
            return State.IsKeyUp(key);
        }

        public bool Down(Keys key)
        {
            return State.IsKeyDown(key);
        }
    }
}
