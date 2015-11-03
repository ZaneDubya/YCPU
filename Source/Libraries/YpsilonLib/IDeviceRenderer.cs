
namespace Ypsilon
{
    public interface IDeviceRenderer
    {
        object RenderLEM(IMemoryBank bank, uint[] chr, uint[] pal);
    }
}
