using System;
using System.Collections.Generic;
using System.Text;

namespace StateMachineCore {
    public class Transition {

        public delegate void TransitionEvent(State initial, State final);

        public event TransitionEvent? onTransition;

        public State Initial { get; set; }
        public State Final { get; set; }

        public Transition(State initialState, State finalState) {
            this.Initial = initialState;
            this.Final = finalState;
        }

        public void triggerTransition() => onTransition?.Invoke(Initial, Final);

        public override string ToString() => (Initial + "->" + Final);

        public override int GetHashCode() => (Initial.GetHashCode() | (0 - Final.GetHashCode()));

        public override bool Equals(object obj) {
            if (!(obj is Transition)) return false;
            Transition other = (Transition)obj;
            return Equals(other);
        }

        public bool Equals(Transition other) {
            return (Initial.Equals(other.Initial) && Final.Equals(other.Final));
        }
    }
}
