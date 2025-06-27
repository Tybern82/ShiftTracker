using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using com.tybern.CallRecordCore.old;

namespace com.tybern.CallRecordCore {

    /// <summary>
    /// Class to implement the clock-update processing. 
    /// </summary>
    public class ApplicationClockTask {

        private readonly Timer _clockTimer;

        /// <summary>
        /// Create a new clock task; Clock updates triggered every second, initial delay of 2 seconds to allow UI to be fully loaded before first trigger
        /// </summary>
        /// <param name="core"><c>CallRecordCore</c> instance to access UI parameters to update each clock cycle</param>
        public ApplicationClockTask(CallRecordCore core) {
            _clockTimer = new Timer(doClockUpdate, core, 2000, 1000);
        }

        private void doClockUpdate(object? callback) {
            if ((callback != null) && (callback is CallRecordCore)) {
                CallRecordCore core = (CallRecordCore)callback;
                DateTime currTime = DateTime.Now;
                CallRecordCore.Instance.UIProperties.CurrentTime = currTime;

                // DateTime breakTime = CallRecordCore.fromCurrent(currTime, CallRecordCore.Instance.UIProperties.BreakTimer);
                // CallRecordCore.Instance.UIProperties.BreakTimerText = (breakTime <= currTime ? "BREAK" : CallRecordCore.toShortTimeString(breakTime - currTime));

                if (CallRecordCore.Instance.InBreak) {
                    CallRecordCore.Instance.UIProperties.CurrentBreakText = CallRecordCore.toShortTimeString(DateTime.Now.TimeOfDay - CallRecordCore.Instance.BreakStartTime);
                }

                DateTime eosTime = CallRecordCore.fromCurrent(currTime, CallRecordCore.Instance.UIProperties.BreakTimes.ShiftEnd);
                CallRecordCore.Instance.UIProperties.EOSTimerText = (
                    eosTime <= currTime 
                    ? "END-OF-SHIFT" 
                    : (
                        CallRecordCore.Instance.UIProperties.BreakTimes.ShiftEnd == CallRecordCore.Instance.UIProperties.BreakTimer 
                        ? "EOS" 
                        : CallRecordCore.toShortTimeString(eosTime - currTime)
                    )
                );

                if (CallRecordCore.Instance.CurrentCall.IsInCall && CallRecordCore.Instance.CurrentCall.CallStartTime != TimeSpan.Zero) {
                    CallRecordCore.Instance.UIProperties.CallTime = currTime.TimeOfDay - CallRecordCore.Instance.CurrentCall.CallStartTime;
                } else {
                    CallRecordCore.Instance.UIProperties.CallTime = TimeSpan.Zero;
                }

                if (CallRecordCore.Instance.CurrentCall.IsInSMECall) CallRecordCore.Instance.UIProperties.SMETime = (currTime.TimeOfDay - CallRecordCore.Instance.CurrentCall.SMEStartTime);
                if (CallRecordCore.Instance.CurrentCall.IsInTransferCall) CallRecordCore.Instance.UIProperties.TransferTime = (currTime.TimeOfDay - CallRecordCore.Instance.CurrentCall.TransferStartTime);
            }
        }
    }
}