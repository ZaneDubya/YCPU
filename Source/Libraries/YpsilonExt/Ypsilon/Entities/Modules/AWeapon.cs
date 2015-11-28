using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Entities.Modules
{
    class AWeapon : AModule
    {
        public bool Activated { get; set; }
        public virtual float ReloadSeconds { get { return 1.0f; } }

        // private float m_SecondsSinceLastFireEvent = 0.0f;

        public AWeapon()
        {
            Activated = true;
        }

        public override void Update(float frameSeconds)
        {
            base.Update(frameSeconds);
        }
    }
}
