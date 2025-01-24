using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CallRecordCore.commands;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.dialogs {
    public class SMERequestResult : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private OptionSMERequest Result { get; set; }
        private string Text { get; set; }

        public SMERequestResult(OptionSMERequest result = OptionSMERequest.Tools, string text = "") {
            Result = result;
            Text = text;
        }

        public void Process() {
            LOG.Info("DIALOG <SMERequest>: Completed");
            CallRecordCore.Instance.Messages.Enqueue(new AppendNote(GetText(Result)));
            if (!string.IsNullOrWhiteSpace(Text)) CallRecordCore.Instance.Messages.Enqueue(new AppendNote(Text));
            TimeSpan smeTime = DateTime.Now.TimeOfDay - CallRecordCore.Instance.CurrentCall.SMEStartTime;
            CallRecordCore.Instance.ShiftCounter.TotalSMETime += smeTime;
            CallRecordCore.Instance.CurrentCall.IsInSMECall = false;
            CallRecordCore.Instance.UIProperties.SMETime = TimeSpan.Zero;
            CallRecordCore.Instance.Messages.Enqueue(new AppendNote(CallRecordCore.toShortTimeString(smeTime)));
        }

        public enum OptionSMERequest { Tools, Query }

        public static string GetText(OptionSMERequest option) {
            switch (option) {
                case OptionSMERequest.Tools:
                    return "Systems / Program Access";

                case OptionSMERequest.Query:
                default:
                    return "General Query";
            }
        }

        public static string GetToolTip(OptionSMERequest option) {
            switch (option) {
                case OptionSMERequest.Tools:
                    return "Agent does not have access to the required program / tool to complete request";

                case OptionSMERequest.Query:
                default:
                    return "Agent general query of SME chat for confirmation / advice";
            }
        }
    }
}
