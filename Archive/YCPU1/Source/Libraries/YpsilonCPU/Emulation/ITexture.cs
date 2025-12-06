namespace Ypsilon.Emulation {
    public interface ITexture {
        int DeviceBusIndex { get; set; }
        int Height { get; }
        int Width { get; }
    }
}