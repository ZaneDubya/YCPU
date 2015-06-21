using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaInput
{
    public class MouseHelper
    {
        public enum Button
        {
            Left = 0,
            Right,
            Middle,
            XButton1,
            XButton2
        }

        MouseState _previousState, _state;
        public MouseState PreviousState
        {
            get { return _previousState; }
        }
        public MouseState State
        {
            get { return _state; }
        }

        public void Update()
        {
            _previousState = State;
            _state = Mouse.GetState();
        }

        public float X()
        {
            return (float)State.X;
        }

        public float Y()
        {
            return (float)State.Y;
        }

        public float ScrollWheelValue()
        {
            return (float)State.ScrollWheelValue;
        }

        public float DeltaX()
        {
            return (float)(PreviousState.X - State.X);
        }

        public float DeltaY()
        {
            return (float)(PreviousState.Y - State.Y);
        }

        public float DeltaScrollWheelValue()
        {
            return (float)(PreviousState.ScrollWheelValue - State.ScrollWheelValue);
        }

        /// <summary>
        /// Creates an InputMethod that returns 1.0f if the mouse button was just clicked, and 0.0f otherwise.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public InputMethod JustPressed(Button button)
        {
            switch (button)
            {
                case Button.Left:
                    return delegate
                    {
                        return (PreviousState.LeftButton == ButtonState.Released &&
                            State.LeftButton == ButtonState.Pressed) ?
                            1.0f :
                            0.0f;
                    };
                case Button.Middle:
                    return delegate
                    {
                        return (PreviousState.MiddleButton == ButtonState.Released &&
                            State.MiddleButton == ButtonState.Pressed) ?
                            1.0f :
                            0.0f;
                    };
                case Button.Right:
                    return delegate
                    {
                        return (PreviousState.RightButton == ButtonState.Released &&
                            State.RightButton == ButtonState.Pressed) ?
                            1.0f :
                            0.0f;
                    };
                case Button.XButton1:
                    return delegate
                    {
                        return (PreviousState.XButton1 == ButtonState.Released &&
                            State.XButton1 == ButtonState.Pressed) ?
                            1.0f :
                            0.0f;
                    };
                case Button.XButton2:
                    return delegate
                    {
                        return (PreviousState.XButton2 == ButtonState.Released &&
                            State.XButton2 == ButtonState.Pressed) ?
                            1.0f :
                            0.0f;
                    };
            }
            throw new Exception("No delegate could be found that matched your input");
        }

        /// <summary>
        /// Creates an InputMethod that returns 1.0f if the mouse button was just released, and 0.0f otherwise.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public InputMethod JustReleased(Button button)
        {
            switch (button)
            {
                case Button.Left:
                    return delegate
                    {
                        return (PreviousState.LeftButton == ButtonState.Pressed &&
                            State.LeftButton == ButtonState.Released) ?
                            1.0f :
                            0.0f;
                    };
                case Button.Middle:
                    return delegate
                    {
                        return (PreviousState.MiddleButton == ButtonState.Pressed &&
                            State.MiddleButton == ButtonState.Released) ?
                            1.0f :
                            0.0f;
                    };
                case Button.Right:
                    return delegate
                    {
                        return (PreviousState.RightButton == ButtonState.Pressed &&
                            State.RightButton == ButtonState.Released) ?
                            1.0f :
                            0.0f;
                    };
                case Button.XButton1:
                    return delegate
                    {
                        return (PreviousState.XButton1 == ButtonState.Pressed &&
                            State.XButton1 == ButtonState.Released) ?
                            1.0f :
                            0.0f;
                    };
                case Button.XButton2:
                    return delegate
                    {
                        return (PreviousState.XButton2 == ButtonState.Pressed &&
                            State.XButton2 == ButtonState.Released) ?
                            1.0f :
                            0.0f;
                    };
            }
            throw new Exception("No delegate could be found that matched your input");
        }

        /// <summary>
        /// Creates an InputMethod that returns 1.0f if the mouse button is down, and 0.0f otherwise.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public InputMethod Down(Button button)
        {
            switch (button)
            {
                case Button.Left:
                    return delegate
                    {
                        return (State.LeftButton == ButtonState.Pressed) ?
                            1.0f :
                            0.0f;
                    };
                case Button.Middle:
                    return delegate
                    {
                        return (State.MiddleButton == ButtonState.Pressed) ?
                            1.0f :
                            0.0f;
                    };
                case Button.Right:
                    return delegate
                    {
                        return (State.RightButton == ButtonState.Pressed) ?
                            1.0f :
                            0.0f;
                    };
                case Button.XButton1:
                    return delegate
                    {
                        return (State.XButton1 == ButtonState.Pressed) ?
                            1.0f :
                            0.0f;
                    };
                case Button.XButton2:
                    return delegate
                    {
                        return (State.XButton2 == ButtonState.Pressed) ?
                            1.0f :
                            0.0f;
                    };
            }
            throw new Exception("No delegate could be found that matched your input");
        }

        /// <summary>
        /// Creates an InputMethod that returns 1.0f if the mouse button is up, and 0.0f otherwise.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public InputMethod Up(Button button)
        {
            switch (button)
            {
                case Button.Left:
                    return delegate
                    {
                        return (State.LeftButton == ButtonState.Released) ?
                            1.0f :
                            0.0f;
                    };
                case Button.Middle:
                    return delegate
                    {
                        return (State.MiddleButton == ButtonState.Released) ?
                            1.0f :
                            0.0f;
                    };
                case Button.Right:
                    return delegate
                    {
                        return (State.RightButton == ButtonState.Released) ?
                            1.0f :
                            0.0f;
                    };
                case Button.XButton1:
                    return delegate
                    {
                        return (State.XButton1 == ButtonState.Released) ?
                            1.0f :
                            0.0f;
                    };
                case Button.XButton2:
                    return delegate
                    {
                        return (State.XButton2 == ButtonState.Released) ?
                            1.0f :
                            0.0f;
                    };
            }
            throw new Exception("No delegate could be found that matched your input");
        }
    }
}
