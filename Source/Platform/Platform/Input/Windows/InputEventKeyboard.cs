using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Platform.Input.Windows
{
    public class InputEventKeyboard : InputEvent
    {
        public KeyboardEventType EventType
        {
            get;
            private set;
        }

        public char KeyChar
        {
            get;
            private set;
        }

        public WinKeys KeyCode
        {
            get;
            private set;
        }

        private int m_keyDataExtra;

        public override string ToString()
        {
            return EventType.ToString() + " " + KeyChar;
        }

        /// <summary>
        /// The repeat count for the current message. The value is the number of times
        /// the keystroke is autorepeated as a result of the user holding down the key.
        /// If the keystroke is held long enough, multiple messages are sent. However,
        /// the repeat count is not cumulative.
        /// The repeat count is always 1 for a WM_KEYUP message.
        /// </summary>
        public int Data_RepeatCount
        {
            get { return (m_keyDataExtra & 0x0000FFFF); }
        }

        /// <summary>
        /// Indicates whether the key is an extended key, such as the right-hand
        /// ALT and CTRL keys that appear on an enhanced 101- or 102-key keyboard.
        /// The value is 1 if it is an extended key; otherwise, it is 0.
        /// </summary>
        public int Data_IsExtendedKey
        {
            get { return ((m_keyDataExtra >> 24) & 0x00000001); }
        }

        /// <summary>
        /// The context code.
        /// The value is always 0 for a WM_KEYDOWN or a WM_KEYUP message.
        /// The value is 1 if the ALT key is down while the key is pressed;
        /// it is 0 if the WM_SYSKEYDOWN or WM_SYSKEYUP message is posted to
        /// the active window because no window has the keyboard focus.
        /// </summary>
        public int Data_ContextCode
        {
            get { return ((m_keyDataExtra >> 29) & 0x00000001); }
        }

        /// <summary>
        /// The previous key state. The value is 1 if the key is down before the
        /// message is sent, or it is zero if the key is up.
        /// The value is always 1 for a WM_(SYS)KEYUP message.
        /// </summary>
        public int Data_PreviousState
        {
            get { return ((m_keyDataExtra >> 30) & 0x00000001); }
        }

        /// <summary>
        /// The transition state. The value is always 0 for a WM_(SYS)KEYDOWN message.
        /// The value is always 1 for a WM_(SYS)KEYUP message.
        /// </summary>
        public int Data_TransitionState
        {
            get { return ((m_keyDataExtra >> 31) & 0x00000001); }
        }



        public InputEventKeyboard(KeyboardEventType eventType, int wParam_VirtKeyCode, int lParam_KeyData, WinKeys modifiers)
            : base(modifiers)
        {
            EventType = eventType;
            KeyCode = (WinKeys)wParam_VirtKeyCode;
            KeyChar = (char)wParam_VirtKeyCode;
            m_keyDataExtra = lParam_KeyData;
        }

        public InputEventKeyboard(KeyboardEventType eventType, InputEventKeyboard parent)
            : base(parent)
        {
            EventType = eventType;
            KeyCode = parent.KeyCode;
            m_keyDataExtra = parent.m_keyDataExtra;
        }

        public void WM_CHAR(InputEventKeyboard e)
        {
            KeyChar = (char)e.KeyCode;
        }
    }

    public enum KeyboardEventType
    {
        Down,
        Up,
        Press
    }
}
