using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CallRecordCore.dialogs;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CStopCall : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private bool resetType = true;

        public CStopCall(bool reset = true)
        {
            this.resetType = reset;
        }
        public void Process() {
            LOG.Info("Stopping Call");
            DateTime timeStamp = DateTime.Now;


            CallRecordCore.Instance.CurrentCall.IsInCall = false;
            CallRecordCore.Instance.CurrentCall.IsInWrap = false;

            if (CallRecordCore.Instance.UIProperties.CallSME > 0) CallRecordCore.Instance.Messages.Enqueue(new AppendNote("SME Calls: " + CallRecordCore.Instance.UIProperties.CallSME));
            CallRecordCore.Instance.Messages.Enqueue(new CallNotesResult(CallRecordCore.Instance.UIProperties.CurrentCallType));
            // CallRecordCore.Instance.Messages.Enqueue(new CCallNotes());
            CallRecordCore.Instance.Messages.Enqueue(new CEndCallSettings(resetType));
        }
    }
}
