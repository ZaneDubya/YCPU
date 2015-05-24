
namespace Ypsilon
{
    public interface IInputProvider
    {
        bool TryGetKeyboardEvent(out ushort keycode);
    }
}
