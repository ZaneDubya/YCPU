
namespace Ypsilon.Emulation
{
    public interface IDisplayProvider
    {
        ITexture RenderLEM(IMemoryBank bank, uint[] chr, uint[] pal);
    }
}
