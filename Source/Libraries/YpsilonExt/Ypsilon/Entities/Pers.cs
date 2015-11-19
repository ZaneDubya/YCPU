using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Entities
{
    /// <summary>
    /// A person/pilot.
    /// </summary>
    class Pers : AEntity
    {
        public Pers(EntityManager manager, Serial serial)
            : base(manager, serial)
        {

        }
    }
}
