using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Ypsilon.Platform.Input
{
    public class KeyboardManager : GameComponent
    {
        //public variables
        public string Text { get; set; }
        /// <summary>
        /// This represents the amount of milliseconds between key presses that must pass
        /// </summary>
        public int MustPass = 125;

        //private and internal variables
        KeyboardState mainState;
        KeyboardState prevState;

        public KeyboardManager(Game game) :
            base(game)
        {

        }

        DateTime prevUpdate = DateTime.Now;
        public override void Update(GameTime gameTime)
        {
            mainState = Keyboard.GetState();
            string input = Convert(mainState.GetPressedKeys());
            string prevInput = Convert(prevState.GetPressedKeys());
            DateTime now = DateTime.Now;
            //make sure 100ms (with a few measurements) has passed
            int time = MustPass;
            if (input == "\b")
                time -= 25;
            if (/*now.Subtract(prevUpdate).Milliseconds >= time*/true)
            {
                foreach (char x in input)
                {
                    //process backspace
                    if (x == '\b')
                    {
                        if (Text.Length >= 1)
                        {
                            Text = Text.Remove(Text.Length - 1, 1);
                        }
                    }
                    else
                        Text += x;
                }
                if (!string.IsNullOrEmpty(input))
                    prevUpdate = now;
            }
            prevState = mainState;
            base.Update(gameTime);
        }

        public string Convert(Keys[] keys)
        {
            string output = "";
            bool usesShift = (keys.Contains(Keys.LeftShift) || keys.Contains(Keys.RightShift));

            foreach (Keys key in keys)
            {
                //thanks SixOfEleven @ DIC
                if (prevState.IsKeyUp(key))
                    continue;

                if (key >= Keys.A && key <= Keys.Z)
                    output += key.ToString();
                else if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                    output += ((int)(key - Keys.NumPad0)).ToString();
                else if (key >= Keys.D0 && key <= Keys.D9)
                {
                    string num = ((int)(key - Keys.D0)).ToString();
                    #region special num chars
                    if (usesShift)
                    {
                        switch (num)
                        {
                            case "1":
                                {
                                    num = "!";
                                }
                                break;
                            case "2":
                                {
                                    num = "@";
                                }
                                break;
                            case "3":
                                {
                                    num = "#";
                                }
                                break;
                            case "4":
                                {
                                    num = "$";
                                }
                                break;
                            case "5":
                                {
                                    num = "%";
                                }
                                break;
                            case "6":
                                {
                                    num = "^";
                                }
                                break;
                            case "7":
                                {
                                    num = "&";
                                }
                                break;
                            case "8":
                                {
                                    num = "*";
                                }
                                break;
                            case "9":
                                {
                                    num = "(";
                                }
                                break;
                            case "0":
                                {
                                    num = ")";
                                }
                                break;
                            default:
                                //wtf?
                                break;
                        }
                    }
                    #endregion
                    output += num;
                }
                else if (key == Keys.OemPeriod)
                    output += ".";
                else if (key == Keys.OemTilde)
                    output += "'";
                else if (key == Keys.Space)
                    output += " ";
                else if (key == Keys.OemMinus)
                    output += "-";
                else if (key == Keys.OemPlus)
                    output += "+";
                else if (key == Keys.OemQuestion && usesShift)
                    output += "?";
                else if (key == Keys.Back) //backspace
                    output += "\b";

                if (!usesShift) //shouldn't need to upper because it's automagically in upper case
                    output = output.ToLower();
            }
            return output;
        }
    }
}
