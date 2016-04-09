/***************************************************************************
 *   InputEventKeyboard.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Ypsilon.Core.Windows;

namespace Ypsilon.Core.Input
{
    public class InputEventKeyboard : InputEvent
    {
        protected readonly KeyboardEvent m_eventType;
        public KeyboardEvent EventType => m_eventType;

        private WinKeys m_KeyChar = WinKeys.None;
        public char KeyChar
        {
            get
            {
                if (m_KeyChar != WinKeys.None)
                {
                    char ch = CultureHandler.TranslateChar((char)m_KeyChar);
                    return ch;
                }
                return '\0';
            }
        }

        public void OverrideKeyChar(WinKeys newChar)
        {
            m_KeyChar = newChar;
        }

        public bool IsChar => KeyChar != '\0';

        public override string ToString()
        {
            return EventType + " " + KeyChar;
        }


        private WinKeys m_KeyCode;
        private int m_KeyDataExtra;

        public WinKeys KeyCode => m_KeyCode;

        public int KeyCodeInt => (int)m_KeyCode;

        /// <summary>
        /// The repeat count for the current message. The value is the number of times
        /// the keystroke is autorepeated as a result of the user holding down the key.
        /// If the keystroke is held long enough, multiple messages are sent. However,
        /// the repeat count is not cumulative.
        /// The repeat count is always 1 for a WM_KEYUP message.
        /// </summary>
        public int DataRepeatCount => (m_KeyDataExtra & 0x0000FFFF);

        /// <summary>
        /// Indicates whether the key is an extended key, such as the right-hand
        /// ALT and CTRL keys that appear on an enhanced 101- or 102-key keyboard.
        /// The value is 1 if it is an extended key; otherwise, it is 0.
        /// </summary>
        public int DataIsExtendedKey => ((m_KeyDataExtra >> 24) & 0x00000001);

        /// <summary>
        /// The context code.
        /// The value is always 0 for a WM_KEYDOWN or a WM_KEYUP message.
        /// The value is 1 if the ALT key is down while the key is pressed;
        /// it is 0 if the WM_SYSKEYDOWN or WM_SYSKEYUP message is posted to
        /// the active window because no window has the keyboard focus.
        /// </summary>
        public int DataContextCode => ((m_KeyDataExtra >> 29) & 0x00000001);

        /// <summary>
        /// The previous key state. The value is 1 if the key is down before the
        /// message is sent, or it is zero if the key is up.
        /// The value is always 1 for a WM_(SYS)KEYUP message.
        /// </summary>
        public int DataPreviousState => ((m_KeyDataExtra >> 30) & 0x00000001);

        /// <summary>
        /// The transition state. The value is always 0 for a WM_(SYS)KEYDOWN message.
        /// The value is always 1 for a WM_(SYS)KEYUP message.
        /// </summary>
        public int DataTransitionState => ((m_KeyDataExtra >> 31) & 0x00000001);

        public InputEventKeyboard(KeyboardEvent eventType, WinKeys wParamVirtKeyCode, int lParamKeyData, WinKeys modifiers)
            : base(modifiers)
        {
            m_eventType = eventType;
            m_KeyCode = wParamVirtKeyCode;
            m_KeyDataExtra = lParamKeyData;
        }

        public InputEventKeyboard(KeyboardEvent eventType, InputEventKeyboard parent)
            : base(parent)
        {
            m_eventType = eventType;
            m_KeyCode = parent.m_KeyCode;
            m_KeyDataExtra = parent.m_KeyDataExtra;
        }
    }
}
