using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CBreakEnd : Command {
        public void Process() {
            CallRecordCore.Instance.InBreak = false;
            CallRecordCore.Instance.UIProperties.CurrentBreakText = string.Empty;
            CallRecordCore.Instance.UIProperties.BreakTimer = CallRecordCore.Instance.UIProperties.BreakTimes.NextBreak;
            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.StartShiftButton);
            // Only enable Start Break button if not at EOS
            if (CallRecordCore.Instance.UIProperties.BreakTimes.ShiftEnd != CallRecordCore.Instance.UIProperties.BreakTimer) CallRecordCore.Instance.UICallbacks?.EnableButton(UICallbacks.UITriggerType.StartBreakButton);
            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.EndBreakButton);
            CallRecordCore.Instance.UICallbacks?.EnableButton(UICallbacks.UITriggerType.EndShiftButton);
        }
    }
}
