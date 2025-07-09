using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using com.tybern.ShiftTracker.db;
using com.tybern.ShiftTracker.enums;
using StateMachine;

namespace com.tybern.ShiftTracker.data {
    public class CallSM : INotifyPropertyChanged, IDisposable, NoteStore {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public static readonly string CALL_WAITING = "Call-Waiting";
        public static readonly string CALL_ACTIVE = "Call-Active";
        public static readonly string CALL_INWRAP = "Call-InWrap";
        public static readonly string CALL_TRANSFER = "Call-Transfer";
        public static readonly string CALL_SME = "Call-SME";

        public StateManager callState { get; private set; }

        private CallRecord? _CurrentCall;
        public CallRecord? CurrentCall { 
            get => _CurrentCall;
            set {
                _CurrentCall = value;
                lock (this) {
                    if ((_CurrentCall != null) && (!string.IsNullOrEmpty(_NoteContent))) {
                        // add any existing note to the start of the new call record and clear the between-call buffer
                        _CurrentCall.prependNote(_NoteContent);
                        _NoteContent = string.Empty;
                    }
                }
                onPropertyChanged(nameof(CurrentCall));
                onPropertyChanged(nameof(NoteContent));
            }
        }

        public CallType Type {
            get => (CurrentCall != null) ? CurrentCall.Type : CallType.Other;
            set {
                if (CurrentCall != null) {
                    CurrentCall.Type = value;
                    onPropertyChanged(nameof(Type));
                }
            }
        }

        private string _NoteContent = string.Empty;
        public string NoteContent {
            get {
                lock(this) return (CurrentCall?.NoteContent ?? _NoteContent).Trim();
            }
            set {
                lock (this) {
                    if (CurrentCall != null) {
                        CurrentCall.NoteContent = value;
                    } else {
                        _NoteContent = value;
                    }
                    onPropertyChanged(nameof(NoteContent));
                }
            } 
        }

        public void prependNote(string note) {
            lock (this) {
                if (CurrentCall != null) {
                    CurrentCall.prependNote(note);
                    onPropertyChanged(nameof(NoteContent));
                } else {
                    string sep = (string.IsNullOrWhiteSpace(_NoteContent)) ? string.Empty : "\n";
                    NoteContent = note.Trim() + sep + NoteContent;
                }
            }
        }

        public void appendNote(string note) {
            lock (this) {
                if (CurrentCall != null) {
                    CurrentCall.appendNote(note);
                    onPropertyChanged(nameof(NoteContent));
                } else {
                    string sep = (string.IsNullOrWhiteSpace(_NoteContent)) ? string.Empty : "\n";
                    NoteContent += sep + note.Trim();
                }
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
                .add(callInWrap, callWaiting)           // Wrap     => Waiting
                .add(callTransfer, callInWrap)          // Transfer => Wrap
                .add(callActive, callTransfer)          // Active   => Transfer
                .add(callWaiting, callSME, false)       // Waiting <=> SME
                .add(callActive, callSME, false)        // Active  <=> SME
                .add(callInWrap, callSME, false);       // Wrap    <=> SME

            Transition? initialCall = callState.getTransition(callWaiting, callActive);
            if (initialCall != null) {
                initialCall.onTransition += (initial, final) => {
                    CurrentCall = new CallRecord(DateTime.Now);
                    SegmentStartTime = null;
                };
            } else {
                LOG.Error("Missing Transition: <Waiting> -> <Active>");
            }

            Transition? directSMEReturn = callState.getTransition(callSME, callWaiting);    // Call was direct to SME, no active call
            if (directSMEReturn != null) {
                directSMEReturn.onTransition += (initial, final) => {
                    // Add the SME call details as a note record
                    NoteRecord nr = new NoteRecord(DateTime.Now) { NoteContent = "Direct " + this.NoteContent };
                    DBShiftTracker.Instance.save(nr);
                    NoteContent = string.Empty;
                };
            } else {
                LOG.Error("Missing Transition: <SME> -> <Waiting>");
            }

            State.getState(CALL_SME).enterState += (oldState, param) => {
                SegmentStartTime = DateTime.Now;
            };

            State.getState(CALL_SME).leaveState += (newState, param) => {
                if ((SegmentStartTime != null) && (CurrentCall != null)) {
                    TimeSpan smeTime = DateTime.Now - (DateTime)SegmentStartTime;
                    SegmentStartTime = null;
                    CurrentCall.SMETime += smeTime;
                }
            };

            State.getState(CALL_TRANSFER).enterState += (oldState, param) => {
                SegmentStartTime = DateTime.Now;
            };

            State.getState(CALL_TRANSFER).leaveState += (newState, param) => {
                if ((SegmentStartTime != null) && (CurrentCall != null)) {  // should ALWAYS be valid
                    TimeSpan transferTime = DateTime.Now - (DateTime)SegmentStartTime;
                    SegmentStartTime = null;
                    CurrentCall.TransferTime += transferTime;
                    CurrentCall.TransferCount++;  // increment Transfer count
                } else LOG.Error("Missing StartTime or no active call after Transfer");                
            };

            State.getState(CALL_INWRAP).enterState += (oldState, param) => {
                SegmentStartTime = DateTime.Now;
            };

            State.getState(CALL_INWRAP).leaveState += (newState, param) => {
                if ((SegmentStartTime != null) && (CurrentCall != null)) {
                    TimeSpan wrapTime = DateTime.Now - (DateTime)SegmentStartTime;
                    SegmentStartTime = null;
                    CurrentCall.WrapTime += wrapTime;
                } else LOG.Error("Missing StartTime or no active call after Wrap");
            };

            State.getState(CALL_WAITING).enterState += (oldState, param) => {
                if (CurrentCall != null)
                    DBShiftTracker.Instance.save(CurrentCall);
                CurrentCall = null;
            };
        }

        private DateTime? SegmentStartTime { get; set; }

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
