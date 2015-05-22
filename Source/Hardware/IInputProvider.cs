
namespace Ypsilon
{
    public interface IInputProvider
    {
        bool TryGetKeypress(out ushort keycode);
    }
}
