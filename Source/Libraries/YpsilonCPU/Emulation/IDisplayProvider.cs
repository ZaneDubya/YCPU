
namespace Ypsilon.Emulation
{
    public interface IDisplayProvider
    {
        ITexture RenderLEM(IMemoryInterface bank, uint[] chr, uint[] pal);
    }
}
