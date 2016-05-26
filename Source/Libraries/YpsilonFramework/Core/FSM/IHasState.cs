namespace Ypsilon.Core.FSM {
    public interface IHasState {
        IState State { get; set; }
    }
}