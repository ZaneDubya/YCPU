
namespace Ypsilon.Emulation
{
    public interface IDeviceRenderer
    {
        ITexture RenderLEM(IMemoryBank bank, uint[] chr, uint[] pal);
    }
}
