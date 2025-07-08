using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using StateMachine;

namespace com.tybern.ShiftTracker.data {
    public class ShiftSM : INotifyPropertyChanged, IDisposable {

        public static readonly string SHIFT_OFFLINE = "Shift-Offline";
        public static readonly string SHIFT_INCALLS = "Shift-InCalls";
        public static readonly string SHIFT_INBREAK = "Shift-InBreak";

        private string _BreakTimerText = string.Empty;
        public string BreakTimerText {
            get { lock (_BreakTimerText) return _BreakTimerText; }
            private set { lock (_BreakTimerText) { _BreakTimerText = value; onPropertyChanged(nameof(BreakTimerText)); } }
        }

        private string _CurrentBreakText = string.Empty;
        public string CurrentBreakText {
            get { return _CurrentBreakText; }
            private set { _CurrentBreakText = value; onPropertyChanged(nameof(CurrentBreakText)); }
        }

        private string _EOSTimerText = string.Empty;
        public string EOSTimerText {
            get { return _EOSTimerText; }
            private set { _EOSTimerText = value; onPropertyChanged(nameof(EOSTimerText)); }
        }

        public SortedSet<WorkBreak> CurrentBreak = new SortedSet<WorkBreak>();
        public TimeSpan BreakStartTime = TimeSpan.Zero;
        private bool disposedValue;

        public StateManager shiftState { get; private set; }
        public WorkShift ActiveShift { get; set; } = new WorkShift(DateTime.Now);

        public ShiftSM() {
            State offline = State.getState(SHIFT_OFFLINE);
            State inCalls = State.getState(SHIFT_INCALLS);
            State inBreak = State.getState(SHIFT_INBREAK);

            // Offline <-> InCalls <=> InBreak
            shiftState = new StateManager(offline)
                .add(offline, inCalls, false)
                .add(inCalls, inBreak, false);

            offline.inState += (s, param) => {
                TimeSpan currTime = decodeStateParam(param);
                BreakTimerText = offlineBreakTimerText(currTime);
                EOSTimerText = offlineEOSTimerText(currTime);
                CurrentBreakText = string.Empty;
            };

            inCalls.inState += (s, param) => {
                TimeSpan currTime = decodeStateParam(param);
                BreakTimerText = onlineBreakTimerText(currTime);
                EOSTimerText = onlineEOSTimerText(currTime);
                CurrentBreakText = string.Empty;
            };

            inBreak.inState += (s, param) => {
                TimeSpan currTime = decodeStateParam(param);
                BreakTimerText = onlineBreakTimerText(currTime);
                EOSTimerText = onlineEOSTimerText(currTime);
                CurrentBreakText = breakText(currTime);
            };

            ClockTimer.GlobalTimer.ClockUpdate += triggerState;
        }

        private string offlineBreakTimerText(TimeSpan currTime) => (currTime < ActiveShift.StartTime) ? ClockTimer.toShortTimeString(ActiveShift.StartTime - currTime) : "OFFLINE";
        private string onlineBreakTimerText(TimeSpan currTime) => (ActiveShift.NextBreak == null)
                ? ((currTime < ActiveShift.EndTime) ? ClockTimer.toShortTimeString(ActiveShift.EndTime - currTime) : "EOS")
                : ((currTime < ActiveShift.NextBreak.StartTime) ? ClockTimer.toShortTimeString(ActiveShift.NextBreak.StartTime - currTime) : ("<" + ClockTimer.toShortTimeString(currTime - ActiveShift.NextBreak.StartTime) + ">"));

        private string offlineEOSTimerText(TimeSpan currTime) => (currTime < ActiveShift.EndTime) ? ClockTimer.toShortTimeString(ActiveShift.EndTime - currTime) : "OFFLINE";
        private string onlineEOSTimerText(TimeSpan currTime) => ((ActiveShift.NextBreak == null) && (currTime >= ActiveShift.StartTime))
            ? "EOS" // mark as EOS if after start of shift, and no further breaks present on shift
            : ((currTime < ActiveShift.EndTime) ? ClockTimer.toShortTimeString(ActiveShift.EndTime - currTime) : "EOS");

        private string breakText(TimeSpan currTime) {
            TimeSpan breakRemaining = (CurrentBreak.Count == 0) ? TimeSpan.Zero : (WorkBreak.GetTotalLength(CurrentBreak) - (currTime - BreakStartTime));
            return (breakRemaining < TimeSpan.Zero) ? "<" + ClockTimer.toShortTimeString(breakRemaining) + ">" : ClockTimer.toShortTimeString(breakRemaining);
        }

        private void triggerState(DateTime currTime) => shiftState.triggerEvent(currTime);

        private TimeSpan decodeStateParam(object? param) => (param == null) ? DateTime.Now.TimeOfDay : ((DateTime)param).TimeOfDay;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void onPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    ClockTimer.GlobalTimer.ClockUpdate -= triggerState; // remove the timer when disposing this class
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
