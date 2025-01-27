using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CBreakStart : Command {
        public void Process() {
            CallRecordCore.Instance.BreakStartTime = DateTime.Now.TimeOfDay;
            CallRecordCore.Instance.InBreak = true;
            TimeSpan currBreak = CallRecordCore.Instance.UIProperties.BreakTimer;
            if (CallRecordCore.Instance.UIProperties.BreakTimes.FirstBreak == currBreak) CallRecordCore.Instance.UIProperties.BreakTimes.FirstBreak = TimeSpan.Zero;
            if (CallRecordCore.Instance.UIProperties.BreakTimes.LastBreak == currBreak) CallRecordCore.Instance.UIProperties.BreakTimes.LastBreak = TimeSpan.Zero;
            if (CallRecordCore.Instance.UIProperties.BreakTimes.LunchBreak == currBreak) CallRecordCore.Instance.UIProperties.BreakTimes.LunchBreak = TimeSpan.Zero;
            if (CallRecordCore.Instance.UIProperties.BreakTimes.MeetingBreak == currBreak) CallRecordCore.Instance.UIProperties.BreakTimes.MeetingBreak = TimeSpan.Zero;
            if (CallRecordCore.Instance.UIProperties.BreakTimes.TrainingBreak == currBreak) CallRecordCore.Instance.UIProperties.BreakTimes.TrainingBreak = TimeSpan.Zero;
            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.StartShiftButton);
            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.StartBreakButton);
            CallRecordCore.Instance.UICallbacks?.EnableButton(UICallbacks.UITriggerType.EndBreakButton);
            CallRecordCore.Instance.UICallbacks?.EnableButton(UICallbacks.UITriggerType.EndShiftButton);
        }
    }
}
