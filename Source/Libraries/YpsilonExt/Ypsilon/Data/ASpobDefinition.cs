using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ypsilon.Data
{
    /// <summary>
    /// Planet definition keeps all essential planet variables organized.<para />
    /// 1. The base values of the variables in the definition are loaded from a file.<para />
    /// 2. Eventually, the planet definition will query a planet's variables to get the current values of these variables.<para />
    /// 3. Right now, everything is hardcoded.
    /// </summary>
    class ASpobDefinition
    {
        public float Size;
        public Color Color;
        public float VertexRandomizationFactor;

        public bool DoRandomizeVertexes
        {
            get
            {
                return VertexRandomizationFactor != 0f;
            }
        }
    }
}
