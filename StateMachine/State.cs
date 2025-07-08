using System;
using System.Collections.Generic;

namespace StateMachine {
    public class State {

        public delegate void StateEvent(State s, object? param);

        public event StateEvent? inState;
        public event StateEvent? enterState;
        public event StateEvent? leaveState;

        private static int NEXT_ID = 0;
        private static Dictionary<string,State> states = new Dictionary<string,State>();

        public int StateID { get; private set; } = NEXT_ID++;
        public string StateName { get; set; }

        private State(string stateName) {
            StateName = stateName;
        }

        public void doEnterState(State? oldState) {
            enterState?.Invoke(this, oldState);
        }

        public void doLeaveState(State? newState) {
            leaveState?.Invoke(this, newState);
        }

        public void triggerEvent(object? param) {
            inState?.Invoke(this, param);
        }

        public static State getState(string stateName) {
            if (states.ContainsKey(stateName)) return states[stateName];
            State _result = new State(stateName);
            states[stateName] = _result;
            return _result;
        }

        public override string ToString() => StateName;

        public override int GetHashCode() => StateID;

        public override bool Equals(object obj) {
            if (!(obj is State)) return false;
            State other = (State)obj;
            return Equals(other);
        }

        public bool Equals(State other) => (StateID == other.StateID);
    }
}
