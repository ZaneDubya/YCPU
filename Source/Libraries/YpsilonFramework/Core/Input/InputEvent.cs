/***************************************************************************
 *   InputEvent.cs
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
    public class InputEvent
    {
        protected readonly WinKeys Modifiers;
        protected bool m_handled;

        public virtual bool Alt => ((Modifiers & WinKeys.Alt) == WinKeys.Alt);

        public bool Control => ((Modifiers & WinKeys.Control) == WinKeys.Control);

        public virtual bool Shift => ((Modifiers & WinKeys.Shift) == WinKeys.Shift);

        public InputEvent(WinKeys modifiers)
        {
            Modifiers = modifiers;
        }

        protected InputEvent(InputEvent parent)
        {
            Modifiers = parent.Modifiers;
        }

        public bool Handled
        {
            get { return m_handled; }
            set { m_handled = value; }
        }

        public void SuppressEvent()
        {
            m_handled = true;
        }
    }
}
