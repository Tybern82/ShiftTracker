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
                (new CStopCall(false)).Process();        // Process Stop Call immediately, rather than in queue, this will queue the <stop> tasks before the following items
                CallRecordCore.Instance.Messages.Enqueue(new CSetCallback());
                CallRecordCore.Instance.Messages.Enqueue(new CStartCall());
                CallRecordCore.Instance.Messages.Enqueue(new CCopyState(CallRecordCore.Instance.CurrentCall.IsSurveyRecorded, CallRecordCore.Instance.CurrentCall.IsPrefNameRequested));
                // Remove duplicate: CallRecordCore.Instance.Messages.Enqueue(new AppendNote("Callback / Outbound"));
                CallRecordCore.Instance.Messages.Enqueue(new COutboundCall());
            } else {
                // If still in Break, end the break before starting new call
                if (CallRecordCore.Instance.InBreak) (new CBreakEnd()).Process();

                // Reset Call Markers for survey/preferred name
                CallRecordCore.Instance.CurrentCall.IsSurveyRecorded = false;
                CallRecordCore.Instance.CurrentCall.IsPrefNameRequested = false;

                CallRecordCore.Instance.UIProperties.TotalCalls++;
                CallRecordCore.Instance.CurrentCall.CallStartTime = timeStamp.TimeOfDay;
                CallRecordCore.Instance.CurrentCall.WrapStartTime = timeStamp.TimeOfDay;
                CallRecordCore.Instance.CurrentCall.IsInCall = true;
                CallRecordCore.Instance.CurrentCall.IsInWrap = false;

                CallRecordCore.Instance.CurrentCall.IsANGenerated = false;
                CallRecordCore.Instance.CurrentCall.IsANEdited = false;
                CallRecordCore.Instance.CurrentCall.IsANSaved = false;
                CallRecordCore.Instance.CurrentCall.HasManualNotes = false;

                CallRecordCore.Instance.CurrentCall.CurrentMode = CallDetails.CallMode.InCall;  // Update UI status to In-Call
            }
        }
    }
}
