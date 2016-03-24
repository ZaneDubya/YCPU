using System;
using System.Collections.Generic;
using Ypsilon.Core.Input;
using Ypsilon.Core.Windows;
using Ypsilon.Emulation;

namespace YCPUXNA.Providers
{
    /// <summary>
    /// This is a front end to Ypsilon.Core.Input.InputManager that provides messages to
    /// </summary>
    public class InputProvider : IInputProvider
    {
        // YPSILON STUFF

        public const ushort EventUp = 0x0100;
        public const ushort EventDown = 0x0200;
        public const ushort EventPress = 0x0300;

        public const ushort CtrlDown = 0x1000;
        public const ushort AltDown = 0x2000;
        public const ushort ShiftDown = 0x4000;
        public const ushort ExtendedKey = 0x8000;

        public bool TryGetKeyboardEvent(out ushort eventCode)
        {
            eventCode = 0;
            for (int i = 0; i < m_EventsThisFrame.Count; i++)
            {
                if (m_EventsThisFrame[i].Handled || !(m_EventsThisFrame[i] is InputEventKeyboard))
                    continue;

                InputEventKeyboard e = (InputEventKeyboard)m_EventsThisFrame[i];
                m_EventsThisFrame.RemoveAt(i);

                bool extended = false;
                ushort bitsKeycode = (byte)e.KeyCode;
                ushort bitsKeyChar = (byte)e.KeyChar;
                if (bitsKeyChar == 0)
                {
                    extended = true;
                    bitsKeyChar = bitsKeycode;
                }

                ushort bitsEvent;
                switch (e.EventType)
                {
                    case KeyboardEvent.Up:
                        bitsEvent = EventUp;
                        break;
                    case KeyboardEvent.Down:
                        bitsEvent = EventDown;
                        break;
                    case KeyboardEvent.Press:
                        bitsEvent = EventPress;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                eventCode = (ushort) (bitsKeyChar | bitsEvent | (e.Shift ? ShiftDown : 0) | (e.Alt ? AltDown : 0) | (e.Control ? CtrlDown : 0) | (extended ? ExtendedKey : 0));

                return true;
            }
            return false;
        }

        // BASE STUFF

        private InputManager m_InputManager;

        private List<InputEvent> m_EventsThisFrame;

        public InputProvider(InputManager input)
        {
            m_InputManager = input;
            m_EventsThisFrame = new List<InputEvent>();
            m_InputManager.Update(0, 0);
        }

        public void Update(float totalSeconds, float frameSeconds)
        {
            m_EventsThisFrame.Clear();

            bool shift = m_InputManager.IsShiftDown;
            bool ctrl = m_InputManager.IsCtrlDown;
            bool alt = m_InputManager.IsAltDown;

            foreach (InputEventKeyboard e in m_InputManager.GetKeyboardEvents())
            {
                m_EventsThisFrame.Add(e);
            }
        }

        public bool HandleKeyboardEvent(KeyboardEvent type, WinKeys key, bool shift, bool alt, bool ctrl)
        {
            foreach (InputEvent e in m_EventsThisFrame)
            {
                if (!e.Handled && e is InputEventKeyboard)
                {
                    InputEventKeyboard ek = (InputEventKeyboard) e;
                    if (ek.EventType == type && ek.KeyCode == key && ek.Shift == shift && ek.Alt == alt && ek.Control == ctrl)
                    {
                        e.Handled = true;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
