using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaInput
{
    public class Pad
    {
        public delegate InputMethod Generator(PlayerIndex player);

        public static class ThumbSticks
        {
            public static class Left
            {
                public static InputMethod X(PlayerIndex player)
                {
                    return delegate
                    {
                        GamePadState state = GamePad.GetState(player);
                        return (state.IsConnected) ?
                            state.ThumbSticks.Left.X :
                            0.0f;
                    };
                }
                public static InputMethod Y(PlayerIndex player)
                {
                    return delegate
                    {
                        GamePadState state = GamePad.GetState(player);
                        return (state.IsConnected) ?
                            state.ThumbSticks.Left.Y :
                            0.0f;
                    };
                }
            }
            public static class Right
            {
                public static InputMethod X(PlayerIndex player)
                {
                    return delegate
                    {
                        GamePadState state = GamePad.GetState(player);
                        return (state.IsConnected) ?
                            state.ThumbSticks.Right.X :
                            0.0f;
                    };
                }
                public static InputMethod Y(PlayerIndex player)
                {
                    return delegate
                    {
                        GamePadState state = GamePad.GetState(player);
                        return (state.IsConnected) ?
                            state.ThumbSticks.Right.Y :
                            0.0f;
                    };
                }
            }
        }

        public static class Triggers
        {
            public static InputMethod Left(PlayerIndex player)
            {
                return delegate
                {
                    GamePadState state = GamePad.GetState(player);
                    return (state.IsConnected) ?
                        state.Triggers.Left :
                        0.0f;
                };
            }
            public static InputMethod Right(PlayerIndex player)
            {
                return delegate
                {
                    GamePadState state = GamePad.GetState(player);
                    return (state.IsConnected) ?
                        state.Triggers.Right :
                        0.0f;
                };
            }
        }

        GamePadState _state, _previousState;
        public GamePadState State
        {
            get { return _state; }
        }
        public GamePadState PreviousState
        {
            get { return _previousState; }
        }
        PlayerIndex player;

        public bool IsConnected
        {
            get { return State.IsConnected; }
        }

        public Pad(PlayerIndex p)
        {
            player = p;
        }

        public void Update()
        {
            _previousState = State;
            _state = GamePad.GetState(player);
        }

        public bool Down(Buttons button)
        {
            return State.IsButtonDown(button);
        }

        public bool Up(Buttons button)
        {
            return State.IsButtonUp(button);
        }

        public bool JustPressed(Buttons button)
        {
            return State.IsButtonDown(button) & PreviousState.IsButtonUp(button);
        }

        public bool JustReleased(Buttons button)
        {
            return State.IsButtonUp(button) & PreviousState.IsButtonDown(button);
        }
    }
}
