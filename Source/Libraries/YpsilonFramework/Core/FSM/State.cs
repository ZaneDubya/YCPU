/* Copyright (c) 2016 Made With Monster Love (Pty) Ltd
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software. */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ypsilon.Core.FSM {
    public class State<T> : IState where T : struct, IConvertible, IComparable {
        private StateMapping m_LastState;
        private readonly Dictionary<object, StateMapping> m_StateLookup;

        public T CurrentState {
            get { return (T)CurrentStateMap.State; }
        }

        public T LastState {
            get {
                if (m_LastState == null)
                    return default(T);
                return (T)m_LastState.State;
            }
        }

        protected State() {
            //Define States
            Array values = Enum.GetValues(typeof(T));
            if (values.Length < 1) {
                throw new ArgumentException("Enum provided to create this state must have at least 1 visible definition");
            }
            m_StateLookup = new Dictionary<object, StateMapping>();
            for (int i = 0; i < values.Length; i++) {
                StateMapping mapping = new StateMapping((Enum)values.GetValue(i));
                m_StateLookup.Add(mapping.State, mapping);
            }

            //Reflect methods
            MethodInfo[] methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public |
                                                        BindingFlags.NonPublic);

            //Bind methods to states
            char[] separator = "_".ToCharArray();
            for (int i = 0; i < methods.Length; i++) {
                if (methods[i].GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length != 0) {
                    continue;
                }
                string[] names = methods[i].Name.Split(separator);

                //Ignore functions without an underscore
                if (names.Length <= 1) {
                    continue;
                }
                Enum key;
                try {
                    key = (Enum)Enum.Parse(typeof(T), names[0]);
                }
                catch (ArgumentException) {
                    //Not an method as listed in the state enum
                    continue;
                }
                StateMapping targetState = m_StateLookup[key];
                switch (names[1]) {
                    case "Enter":
                        targetState.EnterCall = CreateDelegate<Action>(methods[i], this);
                        break;
                    case "Exit":
                        targetState.ExitCall = CreateDelegate<Action>(methods[i], this);
                        break;
                    case "Update":
                        targetState.Update = CreateDelegate<Action<float>>(methods[i], this);
                        break;
                }
            }

            //Create nil state mapping
            CurrentStateMap = new StateMapping(null);
        }

        public void Cleanup() {
            CurrentStateMap?.ExitCall();
        }

        public StateMapping CurrentStateMap { get; private set; }

        public void Update(float frameSeconds) {
            IState fsm = this;
            fsm.CurrentStateMap?.Update(frameSeconds);
        }

        public void ChangeState(T newState) {
            if (m_StateLookup == null) {
                throw new Exception("States have not been configured, please call initialized before trying to set state");
            }
            if (!m_StateLookup.ContainsKey(newState)) {
                throw new Exception("No state with the name " + newState + " can be found. Please make sure you are called the correct type the statemachine was initialized with");
            }
            StateMapping nextState = m_StateLookup[newState];
            if (CurrentStateMap == nextState)
                return;
            CurrentStateMap?.ExitCall();
            m_LastState = CurrentStateMap;
            CurrentStateMap = nextState;
            if (CurrentStateMap != null) {
                CurrentStateMap.EnterCall();
                OnChanged((T)CurrentStateMap.State);
            }
        }

        protected virtual void OnChanged(T newState) {}

        private TV CreateDelegate<TV>(MethodInfo method, object target) where TV : class {
            TV ret = Delegate.CreateDelegate(typeof(TV), target, method) as TV;
            if (ret == null) {
                throw new ArgumentException("Unabled to create delegate for method called " + method.Name);
            }
            return ret;
        }
    }
}