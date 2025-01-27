using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CShiftStart : Command {
        public void Process() {
            CallRecordCore.Instance.UIProperties.BreakTimer = CallRecordCore.Instance.UIProperties.BreakTimes.NextBreak;
            CallRecordCore.Instance.UIProperties.BreakTimes.ShiftStart = TimeSpan.Zero;
            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.StartShiftButton);
            CallRecordCore.Instance.UICallbacks?.EnableButton(UICallbacks.UITriggerType.StartBreakButton);
            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.EndBreakButton);
            CallRecordCore.Instance.UICallbacks?.EnableButton(UICallbacks.UITriggerType.EndShiftButton);
        }
    }
}
