using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StateMachine;

namespace ShiftTrackerGUI.ViewModels {
    public class CallViewModel : ViewModelBase {

        public StateManager callState { get; private set; }

        public static readonly string CALL_WAITING = "Call-Waiting";
        public static readonly string CALL_ACTIVE = "Call-Active";
        public static readonly string CALL_INWRAP = "Call-InWrap";
        public static readonly string CALL_TRANSFER = "Call-Transfer";
        public static readonly string CALL_SME = "Call-SME";

        public CallViewModel() {
            State callWaiting = State.getState(CALL_WAITING);
            State callActive = State.getState(CALL_ACTIVE);
            State callInWrap = State.getState(CALL_INWRAP);
            State callTransfer = State.getState(CALL_TRANSFER);
            State callSME = State.getState(CALL_SME);

            callState = new StateManager(callWaiting)
                .add(callWaiting, callActive)           // Waiting  => Active
                .add(callActive, callInWrap, false)     // Active  <=> Wrap
                .add(callActive, callTransfer)          // Active   => Transfer
                .add(callTransfer, callInWrap)          // Transfer => Wrap
                .add(callActive, callSME, false)        // Active  <=> SME
                .add(callInWrap, callSME, false);       // Wrap    <=> SME
        }
    }
}
