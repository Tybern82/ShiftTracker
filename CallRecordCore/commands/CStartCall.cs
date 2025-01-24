using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CStartCall : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();
        public void Process() {
            LOG.Info("Starting Call");
            DateTime timeStamp = DateTime.Now;
            if (CallRecordCore.Instance.CurrentCall.IsInCall) {
                (new CStopCall()).Process();        // Process Stop Call immediately, rather than in queue, this will queue the <stop> tasks before the following items
                CallRecordCore.Instance.Messages.Enqueue(new CSetCallback());
                CallRecordCore.Instance.Messages.Enqueue(new CStartCall());
                CallRecordCore.Instance.Messages.Enqueue(new CSetSurvey(CallRecordCore.Instance.CurrentCall.IsSurveyRecorded));
                CallRecordCore.Instance.Messages.Enqueue(new AppendNote("Callback / Outbound"));
                CallRecordCore.Instance.Messages.Enqueue(new COutboundCall());
            } else {
                CallRecordCore.Instance.CurrentCall.IsSurveyRecorded = false;

                CallRecordCore.Instance.UIProperties.TotalCalls++;
                CallRecordCore.Instance.CurrentCall.CallStartTime = timeStamp.TimeOfDay;
                CallRecordCore.Instance.CurrentCall.WrapStartTime = timeStamp.TimeOfDay;
                CallRecordCore.Instance.CurrentCall.IsInCall = true;
                CallRecordCore.Instance.CurrentCall.IsInWrap = false;

                CallRecordCore.Instance.CurrentCall.CurrentMode = CallDetails.CallMode.InCall;  // Update UI status to In-Call
            }
        }
    }
}
