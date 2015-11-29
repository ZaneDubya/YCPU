using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Entities.Weapons
{
    class AWeapon : AModule
    {
        /// <summary>
        /// Is is active, then will fire.
        /// </summary>
        public bool IsWeaponActive { get; set; }

        /// <summary>
        /// Reload speed, in seconds.
        /// </summary>
        public virtual float Speed { get { return 0.333f; } }

        /// <summary>
        /// 
        /// </summary>
        public virtual float MinDamage { get { return 1.0f; } }

        /// <summary>
        /// 
        /// </summary>
        public virtual float MaxDamage { get { return 1.0f; } }

        /// <summary>
        /// The distance, in pixels, that a projectile will travel before being disposed.
        /// </summary>
        public virtual float Range { get { return 100.0f; } }

        /// <summary>
        /// 
        /// </summary>
        public virtual float EnergyCost { get { return 1.0f; } }

        /// <summary>
        /// 
        /// </summary>
        public virtual float HeatCost { get { return 1.0f; } }

        /// <summary>
        /// 
        /// </summary>
        public virtual WeaponType WeaponType { get { return WeaponType.Energy; } }

        private float m_SecondsBeforeFireEvents = 0f;

        public AWeapon()
        {
            IsWeaponActive = true;
        }

        public override void Update(float frameSeconds)
        {
            base.Update(frameSeconds);

            m_SecondsBeforeFireEvents -= frameSeconds;
            if (m_SecondsBeforeFireEvents < 0)
                m_SecondsBeforeFireEvents = 0;
        }

        /// <summary>
        /// Returns true if weapon fired.
        /// </summary>
        public bool Fire()
        {
            if (IsWeaponActive && m_SecondsBeforeFireEvents == 0)
            {
                m_SecondsBeforeFireEvents = Speed;
                return OnFire();
            }
            return false;
        }

        /// <summary>
        /// Return true if firing was successful, else return false.
        /// </summary>
        protected virtual bool OnFire()
        {
            return true;
        }
    }
}
