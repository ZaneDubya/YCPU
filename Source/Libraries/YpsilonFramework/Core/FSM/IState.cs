namespace Ypsilon.Core.FSM {
    public interface IState {
        StateMapping CurrentStateMap { get; }
        void Cleanup();
        void Update(float frameSeconds);
    }
}