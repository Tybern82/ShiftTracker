using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CMAERequest : Command {
        public void Process() {
            CallRecordCore.Instance.UIProperties.CallMAE++;
            CallRecordCore.Instance.UIProperties.TotalMAE++;
            CallRecordCore.Instance.CurrentCall.TransferStartTime = DateTime.Now.TimeOfDay;
            CallRecordCore.Instance.CurrentCall.IsInTransferCall = true;
            if (CallRecordCore.Instance.UIProperties.CallMAE > 1) {
                CallRecordCore.Instance.UICallbacks?.ShowDialog(UICallbacks.CallRecordDialog.MAERequestDialog);
            } else {
                CallRecordCore.Instance.Messages.Enqueue(new CTransferRequest());
            }
        }
    }
}
