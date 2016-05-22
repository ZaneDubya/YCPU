/***************************************************************************
 *   InputEventMouse.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using Ypsilon.Core.Windows;

namespace Ypsilon.Core.Input {
    public class InputEventMouse : InputEvent {
        private const int c_WheelDelta = 120;

        private readonly WinMouseButtons m_Button;
        private readonly int m_Clicks;

        public MouseButton Button {
            get {
                if ((m_Button & WinMouseButtons.Left) == WinMouseButtons.Left)
                    return MouseButton.Left;
                if ((m_Button & WinMouseButtons.Right) == WinMouseButtons.Right)
                    return MouseButton.Right;
                if ((m_Button & WinMouseButtons.Middle) == WinMouseButtons.Middle)
                    return MouseButton.Middle;
                if ((m_Button & WinMouseButtons.XButton1) == WinMouseButtons.XButton1)
                    return MouseButton.XButton1;
                if ((m_Button & WinMouseButtons.XButton2) == WinMouseButtons.XButton2)
                    return MouseButton.XButton2;
                return MouseButton.None;
            }
        }

        public MouseEvent EventType { get; }

        public int MouseData { get; }

        public Point Position => new Point(X, Y);
        public int WheelValue => m_Clicks / c_WheelDelta;

        public int X { get; }

        public int Y { get; }

        public InputEventMouse(MouseEvent eventType, WinMouseButtons button, int clicks, int x, int y, int mouseData, WinKeys modifiers)
            : base(modifiers) {
            Vector2 dpi = DpiManager.GetSystemDpiScalar();
            EventType = eventType;
            m_Button = button;
            m_Clicks = clicks;
            X = (int)(x / dpi.X);
            Y = (int)(y / dpi.Y);
            MouseData = mouseData;
        }

        public InputEventMouse(MouseEvent eventType, InputEventMouse parent)
            : base(parent) {
            EventType = eventType;
            m_Button = parent.m_Button;
            m_Clicks = parent.m_Clicks;
            X = parent.X;
            Y = parent.Y;
            MouseData = parent.MouseData;
        }

        public bool IsEvent(MouseEvent e, MouseButton b) {
            if (e == EventType && b == Button)
                return true;
            return false;
        }
    }
}