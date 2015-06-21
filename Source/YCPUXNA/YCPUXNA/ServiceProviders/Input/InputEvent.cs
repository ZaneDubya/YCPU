using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPUXNA.ServiceProviders.Input
{
    public class InputEvent
    {
        public virtual bool Alt
        {
            get;
            private set;
        }

        public bool Control
        {
            get;
            private set;
        }

        public virtual bool Shift
        {
            get;
            private set;
        }

        public bool Handled
        {
            get;
            set;
        }

        public InputEvent(bool shift, bool ctrl, bool alt)
        {
            Shift = shift;
            Control = ctrl;
            Alt = alt;
        }

        public void SuppressEvent()
        {
            Handled = true;
        }
    }
}
