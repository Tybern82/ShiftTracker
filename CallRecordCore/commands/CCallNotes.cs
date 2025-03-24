using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CallRecordCore.dialogs;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CCallNotes : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();
        public void Process() {
            // LOG.Info("Skip dialog and use dropdown for CallType");
            // CallRecordCore.Instance.Messages.Enqueue(new CallNotesResult(CallRecordCore.Instance.UIProperties.CurrentCallType));
            
            LOG.Info("DIALOG <CallNotes>");
            CallRecordCore.Instance.UICallbacks?.ShowDialog(UICallbacks.CallRecordDialog.CallNotesDialog);
            
        }
    }
}
