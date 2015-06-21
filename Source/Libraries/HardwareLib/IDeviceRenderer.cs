
namespace Ypsilon
{
    public interface IDeviceRenderer
    {
        void RenderLEM(IMemoryBank bank, uint[] chr, uint[] pal);
    }
}
