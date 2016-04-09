/***************************************************************************
 *   WndProc.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using Microsoft.Xna.Framework.Input;
using Ypsilon.Core.Input;

namespace Ypsilon.Core.Windows
{
    public delegate void MouseEventHandler(InputEventMouse e);
    public delegate void KeyboardEventHandler(InputEventKeyboard e);

    /// <summary>
    /// Provides an asyncronous Input Event system that can be used to monitor Keyboard and Mouse events.
    /// </summary>
    public class WndProc : MessageHook
    {
        private const bool c_WpPassthrough = true;
        private const bool c_WpNopassthrough = false;

        public override int HookType => NativeConstants.WhCallwndproc;

        public WndProc(IntPtr hWnd)
            : base(hWnd)
        {
            
        }

        public MouseState MouseState => Mouse.GetState();

        /// <summary>
        /// Gets the currently pressed Modifier keys, Control, Alt, Shift
        /// </summary>
        public WinKeys ModifierKeys
        {
            get
            {
                WinKeys keys = WinKeys.None;

                if (NativeMethods.GetKeyState((int)WinKeys.ShiftKey) < 0)
                {
                    keys |= WinKeys.Shift;
                }

                if (NativeMethods.GetKeyState((int)WinKeys.ControlKey) < 0)
                {
                    keys |= WinKeys.Control;
                }

                if (NativeMethods.GetKeyState((int)WinKeys.Menu) < 0)
                {
                    keys |= WinKeys.Alt;
                }

                return keys;
            }
        }

        /// <summary>
        /// Gets the current pressed Mouse Buttons
        /// </summary>
        public WinMouseButtons MouseButtons(MouseState state)
        {
            WinMouseButtons none = WinMouseButtons.None;

            if (state.LeftButton == ButtonState.Pressed)
                none |= WinMouseButtons.Left;
            if (state.RightButton == ButtonState.Pressed)
                none |= WinMouseButtons.Right;
            if (state.MiddleButton == ButtonState.Pressed)
                none |= WinMouseButtons.Middle;
            if (state.XButton1 == ButtonState.Pressed)
                none |= WinMouseButtons.XButton1;
            if (state.XButton2 == ButtonState.Pressed)
                none |= WinMouseButtons.XButton2;

            return none;
        }

        protected override IntPtr WndProcHook(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            Message message = new Message(msg, wParam, lParam);
            if (WndPrc(ref message) == c_WpNopassthrough)
                return IntPtr.Zero;
            return base.WndProcHook(hWnd, msg, wParam, lParam);
        }

        protected bool WndPrc(ref Message message)
        {
            try
            {
                switch (message.Id)
                {
                    case NativeConstants.WmDeadchar:
                        {
                            break;
                        }
                    case NativeConstants.WmKeydown:
                    case NativeConstants.WmKeyup:
                    case NativeConstants.WmChar:
                        {
                            
                            WmKeyEvent(ref message);
                            
                            break;
                        }
                    case NativeConstants.WmUnichar:
                        break;
                    case NativeConstants.WmSyskeydown:
                    case NativeConstants.WmSyskeyup:
                    case NativeConstants.WmSyschar:
                        {
                            NativeMethods.TranslateMessage(ref message);
                            WmKeyEvent(ref message);
                            return c_WpNopassthrough;
                        }
                    case NativeConstants.WmSyscommand:
                        {
                            break;
                        }
                    case NativeConstants.WmMousemove:
                        {
                            WmMouseMove(ref message);
                            break;
                        }
                    case NativeConstants.WmLbuttondown:
                        {
                            WmMouseDown(ref message, WinMouseButtons.Left, 1);
                            break;
                        }
                    case NativeConstants.WmRbuttondown:
                        {
                            WmMouseDown(ref message, WinMouseButtons.Right, 1);
                            break;
                        }
                    case NativeConstants.WmMbuttondown:
                        {
                            WmMouseDown(ref message, WinMouseButtons.Middle, 1);
                            break;
                        }
                    case NativeConstants.WmLbuttonup:
                        {
                            WmMouseUp(ref message, WinMouseButtons.Left, 1);
                            return c_WpPassthrough;
                        }
                    case NativeConstants.WmRbuttonup:
                        {
                            WmMouseUp(ref message, WinMouseButtons.Right, 1);
                            return c_WpPassthrough;
                        }
                    case NativeConstants.WmMbuttonup:
                        {
                            WmMouseUp(ref message, WinMouseButtons.Middle, 1);
                            return c_WpPassthrough;
                        }
                    case NativeConstants.WmLbuttondblclk:
                        {
                            WmMouseDown(ref message, WinMouseButtons.Left, 2);
                            return c_WpPassthrough;
                        }
                    case NativeConstants.WmRbuttondblclk:
                        {
                            WmMouseDown(ref message, WinMouseButtons.Right, 2);
                            return c_WpPassthrough;
                        }
                    case NativeConstants.WmMbuttondblclk:
                        {
                            WmMouseDown(ref message, WinMouseButtons.Middle, 2);
                            return c_WpPassthrough;
                        }
                    case NativeConstants.WmMousewheel:
                        {
                            WmMouseWheel(ref message);
                            return c_WpPassthrough;
                        }
                    case NativeConstants.WmXbuttondown:
                        {
                            WmMouseDown(ref message, GetXButton(Message.HighWord(message.WParam)), 1);
                            return c_WpPassthrough;
                        }
                    case NativeConstants.WmXbuttonup:
                        {
                            WmMouseUp(ref message, GetXButton(Message.HighWord(message.WParam)), 1);
                            return c_WpPassthrough;
                        }
                    case NativeConstants.WmXbuttondblclk:
                        {
                            WmMouseDown(ref message, GetXButton(Message.HighWord(message.WParam)), 2);
                            return c_WpPassthrough;
                        }
                }
            }
            catch
            {
                //TODO: log...crash...what?
            }

            return c_WpPassthrough;
        }

        private WinMouseButtons TranslateWParamIntoMouseButtons(int wParam)
        {
            WinMouseButtons mb = WinMouseButtons.None;
            if ((wParam & 0x0001) == 0x0001)
                mb |= WinMouseButtons.Left;
            if ((wParam & 0x0002) == 0x0002)
                mb |= WinMouseButtons.Right;
            if ((wParam & 0x0002) == 0x0010)
                mb |= WinMouseButtons.Middle;
            if ((wParam & 0x0002) == 0x0020)
                mb |= WinMouseButtons.XButton1;
            if ((wParam & 0x0002) == 0x0040)
                mb |= WinMouseButtons.XButton2;
            return mb;
        }

        /// <summary>
        /// Gets the Mouse XButton deciphered from the wparam argument of a Message
        /// </summary>
        /// <param name="wparam"></param>
        /// <returns></returns>
        private WinMouseButtons GetXButton(int wparam)
        {
            switch (wparam)
            {
                case 1: return WinMouseButtons.XButton1;
                case 2: return WinMouseButtons.XButton2;
            }

            return WinMouseButtons.None;
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Wheel events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        private void WmMouseWheel(ref Message message)
        {
            InvokeMouseWheel(new InputEventMouse(MouseEvent.WheelScroll,
                TranslateWParamIntoMouseButtons(Message.SignedLowWord(message.WParam)),
                Message.SignedHighWord(message.WParam), 
                Message.SignedLowWord(message.LParam), 
                Message.SignedHighWord(message.LParam),
                (int)(long)message.WParam,
                ModifierKeys
                ));
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Move events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        private void WmMouseMove(ref Message message)
        {
            InvokeMouseMove(new InputEventMouse(MouseEvent.Move,
                TranslateWParamIntoMouseButtons(Message.SignedLowWord(message.WParam)),
                0, 
                message.Point.X, 
                message.Point.Y,
                (int)(long)message.WParam,
                ModifierKeys
                ));
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Down events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        /// <param name="button">The Mouse Button the Message is for</param>
        /// <param name="clicks">The number of clicks for the Message</param>
        private void WmMouseDown(ref Message message, WinMouseButtons button, int clicks)
        {
            // HandleMouseBindings();
            InvokeMouseDown(new InputEventMouse(MouseEvent.Down,
                button, 
                clicks, 
                Message.SignedLowWord(message.LParam), 
                Message.SignedHighWord(message.LParam),
                (int)(long)message.WParam,
                ModifierKeys
                ));
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Up events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        /// <param name="button">The Mouse Button the Message is for</param>
        /// <param name="clicks">The number of clicks for the Message</param>
        private void WmMouseUp(ref Message message, WinMouseButtons button, int clicks)
        {
            // HandleMouseBindings();
            InvokeMouseUp(new InputEventMouse(MouseEvent.Up,
                button, 
                clicks, 
                Message.SignedLowWord(message.LParam), 
                Message.SignedHighWord(message.LParam),
                (int)(long)message.WParam,
                ModifierKeys
                ));
        }

        /// <summary>
        /// Reads the supplied message and executes any Keyboard events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        /// <returns>A Boolean value indicating wether the Key events were handled or not</returns>
        private void WmKeyEvent(ref Message message)
        {
            // HandleKeyBindings();
            // KeyPressEventArgs keyPressEventArgs = null;

            if ((message.Id == NativeConstants.WmChar) || (message.Id == NativeConstants.WmSyschar))
            {
                // Is this extra information necessary?
                // wm_(sys)char: http://msdn.microsoft.com/en-us/library/ms646276(VS.85).aspx

                InputEventKeyboard e = new InputEventKeyboard(KeyboardEvent.Press,
                    (WinKeys)(int)(long)message.WParam,
                    (int)(long)message.LParam,
                    ModifierKeys
                    );
                IntPtr zero = (IntPtr)0;
                InvokeChar(e);
            }
            else
            {
                // wm_(sys)keydown: http://msdn.microsoft.com/en-us/library/ms912654.aspx
                // wm_(sys)keyup: http://msdn.microsoft.com/en-us/library/ms646281(VS.85).aspx

                if ((message.Id == NativeConstants.WmKeydown) || (message.Id == NativeConstants.WmSyskeydown))
                {
                    InputEventKeyboard e = new InputEventKeyboard(KeyboardEvent.Down,
                        (WinKeys)(int)(long)message.WParam,
                        (int)(long)message.LParam,
                        ModifierKeys
                        );
                    InvokeKeyDown(e);
                }
                else if ((message.Id == NativeConstants.WmKeyup) || (message.Id == NativeConstants.WmSyskeyup))
                {
                    InputEventKeyboard e = new InputEventKeyboard(KeyboardEvent.Up,
                        (WinKeys)(int)(long)message.WParam,
                        (int)(long)message.LParam,
                        ModifierKeys
                        );
                    InvokeKeyUp(e);
                }
            }
        }

        public event MouseEventHandler MouseWheel, MouseMove, MouseDown, MouseUp;
        public event KeyboardEventHandler KeyUp, KeyDown, KeyChar;

        /// <summary>
        /// Raises the MouseWheel event. Override this method to add code to handle when a mouse wheel is turned
        /// </summary>
        /// <param name="e">InputEventCM for the MouseWheel event</param>
        private void InvokeMouseWheel(InputEventMouse e)
        {
            if (MouseWheel != null)
                MouseWheel(e);
        }

        /// <summary>
        /// Raises the MouseMove event. Override this method to add code to handle when the mouse is moved
        /// </summary>
        /// <param name="e">InputEventCM for the MouseMove event</param>
        private void InvokeMouseMove(InputEventMouse e)
        {
            if (MouseMove != null)
                MouseMove(e);
        }

        /// <summary>
        /// Raises the MouseDown event. Override this method to add code to handle when a mouse button is pressed
        /// </summary>
        /// <param name="e">InputEventCM for the MouseDown event</param>
        private void InvokeMouseDown(InputEventMouse e)
        {
            if (MouseDown != null)
                MouseDown(e);
        }

        /// <summary>
        /// Raises the MouseUp event. Override this method to add code to handle when a mouse button is released
        /// </summary>
        /// <param name="e">InputEventCM for the MouseUp event</param>
        private void InvokeMouseUp(InputEventMouse e)
        {
            if (MouseUp != null)
                MouseUp(e);
        }

        /// <summary>
        /// Raises the KeyUp event. Override this method to add code to handle when a key is released
        /// </summary>
        /// <param name="e">KeyboardPressEventArgs for the KeyUp event</param>
        private void InvokeKeyUp(InputEventKeyboard e)
        {
            if (KeyUp != null)
                KeyUp(e);
        }

        /// <summary>
        /// Raises the KeyDown event. Override this method to add code to handle when a key is pressed
        /// </summary>
        /// <param name="e">InputEventCKB for the KeyDown event</param>
        private void InvokeKeyDown(InputEventKeyboard e)
        {
            if (KeyDown != null)
                KeyDown(e);
        }
        
        /// <summary>
        /// Raises the OnChar event. Override this method to add code to handle when a WM_CHAR message is received
        /// </summary>
        /// <param name="e">InputEventCKB for the OnChar event</param>
        private void InvokeChar(InputEventKeyboard e)
        {
            if (KeyChar != null)
                KeyChar(e);
        }
    }
}
