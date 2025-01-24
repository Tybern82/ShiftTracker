using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CallRecordCore.commands;
using com.tybern.CMDProcessor;
using Org.BouncyCastle.Utilities;

namespace com.tybern.CallRecordCore.dialogs {
    public class MultipleTransferResult : Command {

        private OptionMultipleTransfer Result { get; set; }
        private string Text { get; set; } = string.Empty;

        public MultipleTransferResult(OptionMultipleTransfer result, string text = "") {
            Result = result; Text = text;
        }
        public void Process() {
            CallRecordCore.Instance.Messages.Enqueue(new AppendNote(GetText(Result)));
            if (!string.IsNullOrWhiteSpace(Text)) CallRecordCore.Instance.Messages.Enqueue(new AppendNote(Text));
            if (CallRecordCore.Instance.UIProperties.CallMAE > 1) CallRecordCore.Instance.Messages.Enqueue(new CTransferRequest());
        }

        public enum OptionMultipleTransfer {
            Misrouted, CallDrop, AgentDrop, Incorrect, Other
        }

        public static string GetText(OptionMultipleTransfer options) {
            switch (options) {
                case OptionMultipleTransfer.Misrouted: return "Call Misrouted";
                case OptionMultipleTransfer.CallDrop: return "Caller Disconnected";
                case OptionMultipleTransfer.AgentDrop: return "Agent Disconnected";
                case OptionMultipleTransfer.Incorrect: return "Incorrect Department";

                default: return "Other";
            }
        }

        public static string GetToolTip(OptionMultipleTransfer options) {
            switch (options) {
                case OptionMultipleTransfer.Misrouted: return "Call misrouted to incorrect department";
                case OptionMultipleTransfer.CallDrop: return "Caller disconnected - callback and re-transfer";
                case OptionMultipleTransfer.AgentDrop: return "Agent disconnected - callback to same department";
                case OptionMultipleTransfer.Incorrect: return "Incorrect department - referred to alternate department";

                default: return "Other reason - please specify: ";
            }
        }
    }
}
