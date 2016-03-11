
namespace Ypsilon.Emulation
{
    public interface IDisplayProvider
    {
        ITexture RenderLEM(ISegmentProvider bank, uint[] chr, uint[] pal);
    }
}
