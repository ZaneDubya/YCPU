using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Core.Graphics
{
    public class EffectState
    {
        public readonly Effect Effect;
        public readonly SamplerState Sampler;
        public readonly RasterizerState Raster;
        public readonly bool TextureOverride;
        public readonly DepthStencilState stencil;

        public EffectState(Effect effect, SamplerState sampler, RasterizerState raster, DepthStencilState stencil, bool texture)
        {
            Effect = effect;
            Sampler = sampler;
            Raster = raster;
            TextureOverride = texture;
        }

        public EffectState(Effect effect, SamplerState sampler, RasterizerState raster)
            : this(effect, sampler, raster, DepthStencilState.Default, false)
        {

        }

        public EffectState(Effect effect, SamplerState sample)
            : this(effect, sample, RasterizerState.CullNone, DepthStencilState.Default, false)
        {

        }
    }
}
