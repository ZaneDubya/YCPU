using System;

namespace Ypsilon.Core.FSM {
    public class StateMapping {
        public Action EnterCall = DoNothing;
        public Action ExitCall = DoNothing;
        public readonly object State;
        public Action<float> Update = DoNothing;

        public StateMapping(object state) {
            State = state;
        }

        private static void DoNothing(float frameSeconds) {}
        private static void DoNothing() {}
    }
}