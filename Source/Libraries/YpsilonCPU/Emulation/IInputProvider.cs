
namespace Ypsilon.Emulation
{
    public interface IInputProvider
    {
        bool TryGetKeyboardEvent(out ushort keycode);
    }
}
