using System;
using System.Collections.Generic;
using System.Text;

namespace StateMachineCore {
    public class StateMachine {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public delegate void StateMachineEvent(StateMachine m, object? obj);

        public event StateMachineEvent? SMEvent;

        public HashSet<State> States { get; } = new HashSet<State>();

        public HashSet<Transition> Transitions { get; } = new HashSet<Transition>();

        public State CurrentState { get; private set; }

        private bool _isTransition = false;
        private bool isTransition {
            get { lock (this) return _isTransition; }
            set { lock (this) _isTransition = value; }
        }

        private static int NEXTID = 0;
        private int SMID = NEXTID++;

        public StateMachine(State initialState, bool addDefault = true) {
            this.CurrentState = initialState;
            this.States.Add(initialState);
            if (addDefault) addDefaultEvent();
        }

        public void addDefaultEvent() {
            SMEvent += _callStateEvent;
        }

        private void _callStateEvent(StateMachine sm, object? param) {
            sm.CurrentState.triggerEvent(param);
        }

        public void triggerEvent(object? param) {
            if (!isTransition) {
                LOG.Info("Invoking <" + SMID + ">: " + CurrentState);
                SMEvent?.Invoke(this, param);
            }
        }

        public StateMachine add(State initial, State final, bool directional = true) {
            // Add the given states if they're not already present in the set
            if (!States.Contains(initial)) States.Add(initial);
            if (!States.Contains(final)) States.Add(final);

            // Add the given transition
            Transitions.Add(new Transition(initial, final));
            if (!directional) Transitions.Add(new Transition(final, initial));
            return this;
        }

        public bool gotoState(State newState) {
            isTransition = true;
            Transition? t = getTransition(CurrentState, newState);
            if (t != null) {
                CurrentState = newState;
                t.triggerTransition();
            }
            isTransition = false;
            return (t != null);
        }

        public HashSet<Transition> getTransitionsFrom(State initial) {
            HashSet<Transition> _result = new HashSet<Transition>();
            foreach (Transition t in Transitions) {
                if (t.Initial.Equals(initial)) _result.Add(t);
            }
            return _result;
        }

        public Transition? getTransition(State initial, State final) {
            foreach (Transition t in getTransitionsFrom(initial)) {
                if (t.Final.Equals(final)) return t;
            }
            return null;
        }

        public HashSet<Transition> getTransitionsTo(State final) {
            HashSet<Transition> _result = new HashSet<Transition>();
            foreach (Transition t in Transitions) {
                if (t.Final.Equals(final)) _result.Add(t);
            }
            return _result;
        }
    }
}
