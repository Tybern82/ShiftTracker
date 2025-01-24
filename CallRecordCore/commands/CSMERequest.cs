using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CSMERequest : Command {
        public void Process() {
            CallRecordCore.Instance.Messages.Enqueue(new AppendNote("SME Outbound Call"));
            CallRecordCore.Instance.ShiftCounter.TotalSME++;
            CallRecordCore.Instance.UIProperties.CallSME++;
            CallRecordCore.Instance.CurrentCall.IsInSMECall = true;
            CallRecordCore.Instance.CurrentCall.SMEStartTime = DateTime.Now.TimeOfDay;
            CallRecordCore.Instance.UICallbacks?.ShowDialog(UICallbacks.CallRecordDialog.SMERequestDialog);
        }
    }
}
