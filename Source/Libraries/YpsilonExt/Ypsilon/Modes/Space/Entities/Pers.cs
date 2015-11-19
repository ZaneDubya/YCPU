using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Modes.Space.Entities
{
    /// <summary>
    /// A person/pilot.
    /// </summary>
    class Pers : AEntity
    {
        public override float ViewSize
        {
            get
            {
                return 1f;
            }
        }

        public Pers(EntityManager manager, Serial serial)
            : base(manager, serial)
        {

        }
    }
}
