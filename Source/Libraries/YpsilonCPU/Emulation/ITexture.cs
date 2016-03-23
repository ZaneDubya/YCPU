namespace Ypsilon.Emulation
{
    public interface ITexture
    {
        int Width { get; }
        int Height { get; }
        int DeviceBusIndex { get; set;  }
    }
}
