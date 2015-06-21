using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace YCPUXNA.ServiceProviders.Input
{
    public class InputEventKeyboard : InputEvent
    {
        public KeyboardEventType EventType
        {
            get;
            private set;
        }

        public Keys Key
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return EventType.ToString() + " " + Key.ToString();
        }

        public InputEventKeyboard(KeyboardEventType eventType, Keys key, bool shift, bool ctrl, bool alt)
            : base(shift, ctrl, alt)
        {
            EventType = eventType;
            Key = key;
        }
    }

    public enum KeyboardEventType
    {
        Down,
        Up,
        Press
    }
}
