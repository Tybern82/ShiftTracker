using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using StateMachine;

namespace com.tybern.ShiftTracker.data {
    public class CallSM : INotifyPropertyChanged, IDisposable, NoteStore {

        public static readonly string CALL_WAITING = "Call-Waiting";
        public static readonly string CALL_ACTIVE = "Call-Active";
        public static readonly string CALL_INWRAP = "Call-InWrap";
        public static readonly string CALL_TRANSFER = "Call-Transfer";
        public static readonly string CALL_SME = "Call-SME";

        public StateManager callState { get; private set; }

        private string _NoteContent = string.Empty;
        public string NoteContent {
            get {
                lock(_NoteContent) return _NoteContent;
            }
            set {
                lock (_NoteContent) {
                    _NoteContent = value;
                    onPropertyChanged(nameof(NoteContent));
                }
            } 
        }

        public void prependNote(string note) {
            lock (_NoteContent) {
                string sep = (string.IsNullOrEmpty(_NoteContent)) ? string.Empty : "\n";
                NoteContent = note + sep + NoteContent;
            }
        }

        public void appendNote(string note) {
            lock (_NoteContent) {
                string sep = (string.IsNullOrEmpty(_NoteContent)) ? string.Empty : "\n";
                NoteContent += sep + note;
            }
        }

        private bool disposedValue;

        public CallSM() {
            State callWaiting = State.getState(CALL_WAITING);
            State callActive = State.getState(CALL_ACTIVE);
            State callInWrap = State.getState(CALL_INWRAP);
            State callTransfer = State.getState(CALL_TRANSFER);
            State callSME = State.getState(CALL_SME);

            callState = new StateManager(callWaiting)
                .add(callWaiting, callActive)           // Waiting  => Active
                .add(callActive, callInWrap, false)     // Active  <=> Wrap
                .add(callInWrap, callWaiting)           // Active   => Waiting
                .add(callTransfer, callInWrap)          // Transfer => Wrap
                .add(callActive, callTransfer)          // Active   => Transfer
                .add(callWaiting, callSME, false)       // Waiting <=> SME
                .add(callActive, callSME, false)        // Active  <=> SME
                .add(callInWrap, callSME, false);       // Wrap    <=> SME
                        
            callState.getTransition(callSME, callWaiting).onTransition += (initial, final) => {
                // TODO: Add Transition (SME -> WAITING) - add notes as additional note and clear; (SME -> ACTIVE) and (SME -> WRAP) add note when closing call
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void onPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // ClockTimer.GlobalTimer.ClockUpdate -= triggerState; // remove the timer when disposing this class
                }
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ShiftSM()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
