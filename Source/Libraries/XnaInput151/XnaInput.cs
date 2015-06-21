using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaInput
{
    public delegate float InputMethod();

    public interface IInputService
    {
        //Mouse and Keyboard Helper objects
        MouseHelper Mouse { get; }
        KeyboardHelper Keyboard { get; }

        //Create a delegate for checking input
        InputMethod ControlGen(PlayerIndex player, Pad.Generator gen);
        InputMethod ControlGen(PlayerIndex player, Buttons button);
        InputMethod ControlGen(PlayerIndex player, Buttons high, Buttons low);
        InputMethod ControlGen(Keys key);
        InputMethod ControlGen(Keys high, Keys low);
        InputMethod ControlGen(PlayerIndex player, string source);
        InputMethod ControlGen(PlayerIndex player, string high, string low);

        //Return the InputMethod referenced by a particular string, for use in XML settings files
        InputMethod MapPadString(PlayerIndex player, string source);
        InputMethod MapKeyboardString(string source);
        InputMethod MapMouseString(string source);

        //Assign controls to a single player
        void AssignControl(PlayerIndex player, string name, InputMethod method);
        void AssignControl(PlayerIndex player, string name, Pad.Generator gen);
        void AssignControl(PlayerIndex player, string name, PlayerIndex padPlayer, Pad.Generator gen);
        void AssignControl(PlayerIndex player, string name, Buttons button);
        void AssignControl(PlayerIndex player, string name, Buttons high, Buttons low);
        void AssignControl(PlayerIndex player, string name, Keys key);
        void AssignControl(PlayerIndex player, string name, Keys high, Keys low);
        void AssignControl(PlayerIndex player, string name, string source);
        void AssignControl(PlayerIndex player, string name, string high, string low);

        //Assign controls to all players
        void AssignControl(string name, InputMethod method);
        void AssignControl(string name, Pad.Generator gen);
        void AssignControl(string name, PlayerIndex padPlayer, Pad.Generator gen);
        void AssignControl(string name, Buttons button);
        void AssignControl(string name, Buttons high, Buttons low);
        void AssignControl(string name, Keys key);
        void AssignControl(string name, Keys high, Keys low);
        void AssignControl(string name, string source);
        void AssignControl(string name, string high, string low);

        //Get the value of a control
        float ControlState(PlayerIndex player, string name);
    }

    public class InputManager : GameComponent, IInputService
    {
        private Dictionary<PlayerIndex, Dictionary<string, InputMethod>> PlayerControls;
        private MouseHelper _mouse;
        private KeyboardHelper _keyboard;
        public Dictionary<PlayerIndex, Pad> GamePads;
        
        public MouseHelper Mouse
        {
            get { return _mouse; }
        }

        public KeyboardHelper Keyboard
        {
            get { return _keyboard; }
        }
        
        #region Methods that just return a delegate
        /// <summary>
        /// Returns a delegate for an analog part of the GamePad with the given player index.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="gen"></param>
        public InputMethod ControlGen(PlayerIndex player, Pad.Generator gen)
        {
            return gen(player);
        }

        /// <summary>
        /// Creates an Input method that checks a GamePad button. The delegate will return 1.0f if the button is pressed, 0.0f otherwise.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="button"></param>
        public InputMethod ControlGen(PlayerIndex player, Buttons button)
        {
            return delegate
            {
                return (GamePads[player].IsConnected && GamePads[player].Down(button)) ? 1.0f : 0.0f;
            };
        }

        /// <summary>
        /// Creates an InputMethod that checks two GamePad buttons. The delegate returns 1.0f if the first is pressed,
        /// -1.0f if the second is pressed, and 0.0f if both or none are pressed.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        public InputMethod ControlGen(PlayerIndex player, Buttons high, Buttons low)
        {
            return delegate
            {
                return ControlGen(player, high)() - ControlGen(player, low)();
            };
        }

        /// <summary>
        /// Creates an InputMethod that checks a keyboard key. The delegate returns 1.0f if the key is pressed, 0.0f otherwise.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="key"></param>
        public InputMethod ControlGen(Keys key)
        {
            return delegate { return (Keyboard.State.IsKeyDown(key)) ? 1.0f : 0.0f; };
        }

        /// <summary>
        /// Returns an InputMethod. The delegate returns 1.0f if the first is pressed,
        /// -1.0f if the second is pressed, and 0.0f if both or neither are pressed.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        public InputMethod ControlGen(Keys high, Keys low)
        {
            return delegate
                {
                    return ControlGen(high)() - ControlGen(low)();
                };
        }

        /// <summary>
        /// Returns an InputMethod referenced by a string.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        public InputMethod ControlGen(PlayerIndex player, string source)
        {
            string[] parts = source.Split('.');
            switch (parts[0])
            {
                case "Mouse":
                    return MapMouseString(String.Join(".", parts, 1, parts.Length - 1));
                case "Keyboard":
                    return MapKeyboardString(String.Join(".", parts, 1, parts.Length - 1));
                case "Pad":
                    return MapPadString(player, String.Join(".", parts, 1, parts.Length - 1));
            }
            throw new Exception("No delegate could be found that matched the string");
        }

        /// <summary>
        /// Returns an InputMethod referrenced by two strings. The first string specifies source for the low value (-1.0f),
        /// the second specifies the source for the high value (1.0f)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <returns></returns>
        public InputMethod ControlGen(PlayerIndex player, string high, string low)
        {
            string[] high_parts = high.Split('.');
            string[] low_parts = low.Split('.');
            InputMethod high_del;
            InputMethod low_del;
            if (high_parts[0] == "GamePad")
            {
                high_del = MapPadString(player, String.Join(".", high_parts, 1, high_parts.Length - 1));
            }
            else //Keyboard
            {
                high_del = MapKeyboardString(String.Join(".", high_parts, 1, high_parts.Length - 1));
            }
            if (low_parts[0] == "GamePad")
            {
                low_del = MapPadString(player, String.Join(".", low_parts, 1, low_parts.Length - 1));
            }
            else //Keyboard
            {
                low_del = MapKeyboardString(String.Join(".", low_parts, 1, low_parts.Length - 1));
            }
            return delegate
            {
                return high_del() - low_del();
            };
        }

        /// <summary>
        /// Returns the GamePad InputMethod referenced by the string.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public InputMethod MapPadString(PlayerIndex player, string source)
        {
            string[] parts = source.Split('.');
            Buttons pad_button;
            switch (parts[0])
            {
                case "ThumbSticks":
                    switch (parts[1])
                    {
                        case "Left":
                            return (parts[2] == "X") ? ControlGen(player, Pad.ThumbSticks.Left.X) : ControlGen(player, Pad.ThumbSticks.Left.Y);
                        case "Right":
                            return (parts[2] == "X") ? ControlGen(player, Pad.ThumbSticks.Right.X) : ControlGen(player, Pad.ThumbSticks.Right.Y);
                    }
                    break;
                case "Triggers":
                    switch (parts[1])
                    {
                        case "Left":
                            return ControlGen(player, Pad.Triggers.Left);
                        case "Right":
                            return ControlGen(player, Pad.Triggers.Right);
                    }
                    break;
                case "JustPressed":
                    pad_button = (Buttons)Enum.Parse(typeof(Buttons), parts[1]);
                    return delegate
                    {
                        return GamePads[player].JustPressed(pad_button) ? 1.0f : 0.0f;
                    };
                case "JustReleased":
                    pad_button = (Buttons)Enum.Parse(typeof(Buttons), parts[1]);
                    return delegate
                    {
                        return GamePads[player].JustReleased(pad_button) ? 1.0f : 0.0f;
                    };
                case "Up":
                    pad_button = (Buttons)Enum.Parse(typeof(Buttons), parts[1]);
                    return delegate
                    {
                        return GamePads[player].Up(pad_button) ? 1.0f : 0.0f;
                    };
                case "Down":
                    pad_button = (Buttons)Enum.Parse(typeof(Buttons), parts[1]);
                    return delegate
                    {
                        return GamePads[player].Down(pad_button) ? 1.0f : 0.0f;
                    };
            }
            throw new Exception("No delegate could be found that matched the string");
        }

        /// <summary>
        /// Returns the Keyboard InputMethod referenced by the string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public InputMethod MapKeyboardString(string source)
        {
            string[] parts = source.Split('.');
            Keys key = (Keys)Enum.Parse(typeof(Keys), parts[parts.Length - 1]);
            switch (parts[0])
            {
                case "JustPressed":
                    return delegate
                    {
                        return (Keyboard.JustPressed(key)) ? 1.0f : 0.0f;
                    };
                case "JustReleased":
                    return delegate
                    {
                        return (Keyboard.JustReleased(key)) ? 1.0f : 0.0f;
                    };
                case "Up":
                    return delegate
                    {
                        return (Keyboard.Up(key)) ? 1.0f : 0.0f;
                    };
                case "Down":
                    return delegate
                    {
                        return (Keyboard.Down(key)) ? 1.0f : 0.0f;
                    };
            }
            throw new Exception("No delegate could be found that matched the string");
        }

        /// <summary>
        /// Returns the Mouse InputMethod referenced by the string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public InputMethod MapMouseString(string source)
        {
            string[] parts = source.Split('.');
            if (parts.Length == 2)
            {
                MouseHelper.Button mouse_button = (MouseHelper.Button)Enum.Parse(typeof(MouseHelper.Button), parts[1]);
                switch (parts[0])
                {
                    case "JustPressed":
                        return Mouse.JustPressed(mouse_button);
                    case "JustReleased":
                        return Mouse.JustReleased(mouse_button);
                    case "Down":
                        return Mouse.Down(mouse_button);
                    case "Up":
                        return Mouse.Up(mouse_button);
                }
            }
            switch (parts[0])
            {
                case "X":
                    return Mouse.X;
                case "Y":
                    return Mouse.Y;
                case "ScrollWheelValue":
                    return Mouse.ScrollWheelValue;
                case "DeltaX":
                    return Mouse.DeltaX;
                case "DeltaY":
                    return Mouse.DeltaY;
                case "DeltaScrollWheelValue":
                    return Mouse.DeltaScrollWheelValue;
            }
            throw new Exception("No delegate could be found that matched the string");
        }
        #endregion

        #region Methods for assigning controls to a single player index

        /// <summary>
        /// Allows you to assign a custom-made InputMethod to a control.
        /// InputMethods take no arguments, and return a float.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="method"></param>
        public void AssignControl(PlayerIndex player, string name, InputMethod method)
        {
            PlayerControls[player][name] = method;
        }

        /// <summary>
        /// Allows you to assign an analog GamePad part to an input control, with the same gamepad as player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="gen"></param>
        public void AssignControl(PlayerIndex player, string name, Pad.Generator gen)
        {
            PlayerControls[player][name] = gen(player);
        }

        /// <summary>
        /// Assigns a gamepad part to a control, with a different gamepad playerindex than player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="padPlayer"></param>
        /// <param name="gen"></param>
        public void AssignControl(PlayerIndex player, string name, PlayerIndex padPlayer, Pad.Generator gen)
        {
            PlayerControls[player][name] = gen(padPlayer);
        }

        /// <summary>
        /// Assigns a GamePad button to a control. The control will return 1.0f if the button is pressed, 0.0f otherwise.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="button"></param>
        public void AssignControl(PlayerIndex player, string name, Buttons button)
        {
            PlayerControls[player][name] = ControlGen(player, button);
        }

        /// <summary>
        /// Assigns two Gamepad buttons to a control. The control returns 1.0f if the first is pressed,
        /// -1.0f if the second is pressed, and 0.0f if both or none are pressed.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        public void AssignControl(PlayerIndex player, string name, Buttons high, Buttons low)
        {
            PlayerControls[player][name] = ControlGen(player, high, low);
        }

        /// <summary>
        /// Assigns a keyboard key to a control. The control returns 1.0f if the key is pressed, 0.0f otherwise.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="key"></param>
        public void AssignControl(PlayerIndex player, string name, Keys key)
        {
            PlayerControls[player][name] = ControlGen(key);
        }

        /// <summary>
        /// Assigns two keys to a control. The control returns 1.0f if the first is pressed,
        /// -1.0f if the second is pressed, and 0.0f if both or neither are pressed.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        public void AssignControl(PlayerIndex player, string name, Keys high, Keys low)
        {
            PlayerControls[player][name] = ControlGen(high, low);
        }

        public void AssignControl(PlayerIndex player, string name, string source)
        {
            PlayerControls[player][name] = ControlGen(player, source);
        }

        public void AssignControl(PlayerIndex player, string name, string high, string low)
        {
            PlayerControls[player][name] = ControlGen(player, high, low);
        }
        #endregion

        #region Methods for assigning controls to all player indices

        /// <summary>
        /// Allows you to assign a custom-made InputMethod to a control for all players.
        /// InputMethods take no arguments, and return a float.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="method"></param>
        public void AssignControl(string name, InputMethod method)
        {
            AssignControl(PlayerIndex.One, name, method);
            AssignControl(PlayerIndex.Two, name, method);
            AssignControl(PlayerIndex.Three, name, method);
            AssignControl(PlayerIndex.Four, name, method);
        }

        /// <summary>
        /// Allows you to assign an analog GamePad part to an input control for all players, with the same gamepad as player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="gen"></param>
        public void AssignControl(string name, Pad.Generator gen)
        {
            AssignControl(PlayerIndex.One, name, gen);
            AssignControl(PlayerIndex.Two, name, gen);
            AssignControl(PlayerIndex.Three, name, gen);
            AssignControl(PlayerIndex.Four, name, gen);
        }

        /// <summary>
        /// Assigns a gamepad part to a control for all players, with a different gamepad playerindex than player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="padPlayer"></param>
        /// <param name="gen"></param>
        public void AssignControl(string name, PlayerIndex padPlayer, Pad.Generator gen)
        {
            AssignControl(PlayerIndex.One, name, padPlayer, gen);
            AssignControl(PlayerIndex.Two, name, padPlayer, gen);
            AssignControl(PlayerIndex.Three, name, padPlayer, gen);
            AssignControl(PlayerIndex.Four, name, padPlayer, gen);
        }

        /// <summary>
        /// Assigns a GamePad button to a control for all players. The control will return 1.0f if the button is pressed, 0.0f otherwise.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="button"></param>
        public void AssignControl(string name, Buttons button)
        {
            AssignControl(PlayerIndex.One, name, button);
            AssignControl(PlayerIndex.Two, name, button);
            AssignControl(PlayerIndex.Three, name, button);
            AssignControl(PlayerIndex.Four, name, button);
        }

        /// <summary>
        /// Assigns two Gamepad buttons to a control for all players. The control returns 1.0f if the first is pressed,
        /// -1.0f if the second is pressed, and 0.0f if both or none are pressed.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        public void AssignControl(string name, Buttons high, Buttons low)
        {
            AssignControl(PlayerIndex.One, name, high, low);
            AssignControl(PlayerIndex.Two, name, high, low);
            AssignControl(PlayerIndex.Three, name, high, low);
            AssignControl(PlayerIndex.Four, name, high, low);
        }

        /// <summary>
        /// Assigns a keyboard key to a control for all players. The control returns 1.0f if the key is pressed, 0.0f otherwise.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="key"></param>
        public void AssignControl(string name, Keys key)
        {
            AssignControl(PlayerIndex.One, name, key);
            AssignControl(PlayerIndex.Two, name, key);
            AssignControl(PlayerIndex.Three, name, key);
            AssignControl(PlayerIndex.Four, name, key);
        }

        /// <summary>
        /// Assigns two keys to a control. The control returns 1.0f if the first is pressed,
        /// -1.0f if the second is pressed, and 0.0f if both or neither are pressed.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        public void AssignControl(string name, Keys high, Keys low)
        {
            AssignControl(PlayerIndex.One, name, high, low);
            AssignControl(PlayerIndex.Two, name, high, low);
            AssignControl(PlayerIndex.Three, name, high, low);
            AssignControl(PlayerIndex.Four, name, high, low);
        }

        /// <summary>
        /// Assign a control to all players, referenced by a string.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="source"></param>
        public void AssignControl(string name, string source)
        {
            AssignControl(PlayerIndex.One, name, source);
            AssignControl(PlayerIndex.Two, name, source);
            AssignControl(PlayerIndex.Three, name, source);
            AssignControl(PlayerIndex.Four, name, source);
        }

        /// <summary>
        /// Assign a control to all players referenced by two strings, which specify the source for the high and low values.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        public void AssignControl(string name, string high, string low)
        {
            AssignControl(PlayerIndex.One, name, high, low);
            AssignControl(PlayerIndex.Two, name, high, low);
            AssignControl(PlayerIndex.Three, name, high, low);
            AssignControl(PlayerIndex.Four, name, high, low);
        }
        #endregion

        /// <summary>
        /// Get the current state of a control.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public float ControlState(PlayerIndex player, string name)
        {
            if (PlayerControls[player].ContainsKey(name))
            {
                return PlayerControls[player][name]();
            }
            else
            {
                return 0.0f;
            }
        }

        public InputManager(Game game)
            : base(game)
        {
            PlayerControls = new Dictionary<PlayerIndex, Dictionary<string, InputMethod>>();
            PlayerControls[PlayerIndex.One] = new Dictionary<string, InputMethod>();
            PlayerControls[PlayerIndex.Two] = new Dictionary<string, InputMethod>();
            PlayerControls[PlayerIndex.Three] = new Dictionary<string, InputMethod>();
            PlayerControls[PlayerIndex.Four] = new Dictionary<string, InputMethod>();

            GamePads = new Dictionary<PlayerIndex, Pad>();
            GamePads[PlayerIndex.One] = new Pad(PlayerIndex.One);
            GamePads[PlayerIndex.Two] = new Pad(PlayerIndex.Two);
            GamePads[PlayerIndex.Three] = new Pad(PlayerIndex.Three);
            GamePads[PlayerIndex.Four] = new Pad(PlayerIndex.Four);

            _mouse = new MouseHelper();
            _keyboard = new KeyboardHelper();

            game.Components.Add(this);
            game.Services.AddService(typeof(IInputService), this);
        }

        public override void Update(GameTime gameTime)
        {
            GamePads[PlayerIndex.One].Update();
            GamePads[PlayerIndex.Two].Update();
            GamePads[PlayerIndex.Three].Update();
            GamePads[PlayerIndex.Four].Update();

            Mouse.Update();
            Keyboard.Update();

            base.Update(gameTime);
        }
    }
}
