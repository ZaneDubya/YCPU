namespace Ypsilon.Emulation {
    public interface IInputProvider {
        bool TryGetKeyboardEvent(bool translateASCII, out ushort keycode);
    }
}