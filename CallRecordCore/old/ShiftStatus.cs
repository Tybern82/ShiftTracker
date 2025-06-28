using System;
using System.Collections.Generic;
using System.Text;

namespace com.tybern.ShiftTracker.shifts
{
    public enum ShiftState { Offline, WaitingForCall, InCall, InBreak }

    public enum CallState { Waiting, CallStart, InCall, InWrap }

    public class ShiftStatus {

        public ShiftState CurrentShiftState { get; set; } = ShiftState.Offline;

        public CallState CurrentCallState { get; set; } = CallState.Waiting;

        public void doStartShift() {
            CurrentShiftState = ShiftState.WaitingForCall;
            CurrentCallState = CallState.Waiting;
        }

        public void doEndShift() {
            CurrentShiftState = ShiftState.Offline;
            CurrentCallState = CallState.Waiting;
        }

        public void doStartCall() {
            CurrentShiftState = ShiftState.InCall;
            CurrentCallState = CallState.CallStart;
        }

        public void doStartWrap() {
            CurrentCallState = CallState.InWrap;
        }

        public void doEndCall() {
            CurrentShiftState = ShiftState.WaitingForCall;
            CurrentCallState = CallState.Waiting;
        }

        public void doStartBreak() {
            CurrentShiftState = ShiftState.InBreak;
            CurrentCallState = CallState.Waiting;
        }

        public void doEndBreak() {
            CurrentShiftState = ShiftState.WaitingForCall;
            CurrentCallState = CallState.Waiting;
        }
    }
}
