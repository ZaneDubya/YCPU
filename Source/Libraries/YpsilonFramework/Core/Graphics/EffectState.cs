using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Core.Graphics
{
    public class EffectState
    {
        public readonly Effect Effect;
        public readonly SamplerState Sampler;

        public EffectState(Effect effect, SamplerState sampler)
        {
            Effect = effect;
            Sampler = sampler;
        }
    }
}
