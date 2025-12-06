namespace Ypsilon.Emulation {
    public interface IDisplayProvider {
        ITexture RenderLEM(int busIndex, int chrsWidth, int chrHeight, byte[] memory, uint[] chr, uint[] pal, bool selectPage1, bool doSprites);
    }
}