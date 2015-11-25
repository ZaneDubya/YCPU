using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Materials
{
    enum MaterialQuality
    {
        IsInorganic,
        #region Inorganics
            IsMineral,
            #region Minerals
                IsOre,
                IsMetal,
            #endregion
            IsChemical,
            IsGas,
            IsLiquid,
        #endregion

        IsOrganic,

    }
}
