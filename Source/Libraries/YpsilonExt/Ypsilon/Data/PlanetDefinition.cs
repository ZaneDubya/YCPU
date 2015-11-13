using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Data
{
    /// <summary>
    /// Planet definition keeps all essential planet variables organized.
    /// 1. The base values of the variables in the definition are loaded from a file.
    /// 2. Eventually, the planet definition will query a planet's variables to get the current values of these variables.
    /// 3. Right now, everything is hardcoded.
    /// </summary>
    class PlanetDefinition
    {
        public float Size = 10f;
    }
}
