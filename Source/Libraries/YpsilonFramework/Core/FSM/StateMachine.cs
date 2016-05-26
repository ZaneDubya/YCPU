using System;

namespace Ypsilon.Core.FSM {
    public static class StateMachine {
        /// <summary>
        /// Inspects passed entity for state methods matching supplied Enum, and returns a stateMachine instance used to trasition states.
        /// </summary>
        public static IState Create<T>(IHasState entity, State<T> state) where T : struct, IConvertible, IComparable {
            entity.State?.Cleanup();
            entity.State = state;
            return state;
        }

        /// <summary>
        /// Inspects passed entity for state methods matching supplied Enum, and returns a stateMachine instance used to trasition states.
        /// </summary>
        public static IState Create<T>(IHasState entity, State<T> state, T startState) where T : struct, IConvertible, IComparable {
            state = (State<T>)Create(entity, state);
            state.ChangeState(startState);
            return state;
        }
    }
}