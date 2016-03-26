
namespace Ypsilon.Emulation
{
    public interface IDisplayProvider
    {
        ITexture RenderLEM(byte[] memory, uint[] chr, uint[] pal, bool selectPage1);
    }
}
