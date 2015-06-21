using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon;
using Microsoft.Xna.Framework;
using XnaInput;
using Microsoft.Xna.Framework.Input;
using YCPUXNA.ServiceProviders.Input;

namespace YCPUXNA.ServiceProviders
{
    public class InputService : IInputProvider
    {
        // YPSILON STUFF

        private const ushort EventUp = 0x0100;
        private const ushort EventDown = 0x0200;
        private const ushort EventPress = 0x0300;

        private const ushort CtrlDown = 0x1000;
        private const ushort AltDown = 0x2000;
        private const ushort ShiftDown = 0x4000;

        public bool TryGetKeyboardEvent(out ushort eventCode)
        {
            eventCode = 0;
            for (int i = 0; i < m_EventsThisFrame.Count; i++)
            {
                if (!m_EventsThisFrame[i].Handled && m_EventsThisFrame[i] is InputEventKeyboard)
                {
                    InputEventKeyboard e = m_EventsThisFrame[i] as InputEventKeyboard;
                    m_EventsThisFrame.RemoveAt(i);

                    ushort bitsKeycode = (byte)e.Key;
                    ushort bitsEvent = 0;

                    switch (e.EventType)
                    {
                        case KeyboardEventType.Up:
                            bitsEvent = EventUp;
                            break;
                        case KeyboardEventType.Down:
                            bitsEvent = EventDown;
                            break;
                        case KeyboardEventType.Press:
                            bitsEvent = EventPress;
                            break;
                    }

                    eventCode = (ushort)(
                        bitsKeycode |
                        bitsEvent |
                        ((e.Shift) ? ShiftDown : 0) |
                        ((e.Alt) ? AltDown : 0) |
                        ((e.Control) ? CtrlDown : 0));
                    return true;
                }
            }
            return false;
        }

        // BASE STUFF

        private InputManager m_InputManager;

        private List<InputEvent> m_EventsThisFrame;

        public InputService(Game game)
        {
            m_InputManager = new InputManager(game);
            m_EventsThisFrame = new List<InputEvent>();
            m_InputManager.Keyboard.Update();
        }

        public void Update()
        {
            m_EventsThisFrame.Clear();

            m_InputManager.Keyboard.Update();

            Keys[] previous = m_InputManager.Keyboard.PreviousState.GetPressedKeys();
            Keys[] current = m_InputManager.Keyboard.State.GetPressedKeys();

            bool shift = m_InputManager.Keyboard.Down(Keys.LeftShift) || m_InputManager.Keyboard.Down(Keys.RightShift);
            bool ctrl = m_InputManager.Keyboard.Down(Keys.LeftControl) || m_InputManager.Keyboard.Down(Keys.RightControl);
            bool alt = m_InputManager.Keyboard.Down(Keys.LeftAlt) || m_InputManager.Keyboard.Down(Keys.RightAlt);

            foreach (Keys key in previous)
            {
                if (!current.Contains(key))
                {
                    m_EventsThisFrame.Add(new InputEventKeyboard(KeyboardEventType.Up, key, shift, ctrl, alt));
                }
            }

            foreach (Keys key in current)
            {
                if (!previous.Contains(key))
                {
                    m_EventsThisFrame.Add(new InputEventKeyboard(KeyboardEventType.Down, key, shift, ctrl, alt));
                    m_EventsThisFrame.Add(new InputEventKeyboard(KeyboardEventType.Press, key, shift, ctrl, alt));
                }
            }
        }

        public bool HandleKeyboardEvent(KeyboardEventType type, Keys key, bool shift, bool alt, bool ctrl)
        {
            foreach (InputEvent e in m_EventsThisFrame)
            {
                if (!e.Handled && e is InputEventKeyboard)
                {
                    InputEventKeyboard ek = (InputEventKeyboard)e;
                    if (ek.EventType == type &&
                       ek.Key == key &&
                       ek.Shift == shift &&
                       ek.Alt == alt &&
                       ek.Control == ctrl)
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
