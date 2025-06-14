using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CallRecordCore.commands;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.dialogs {
    public class OutboundCallResult : Command {

        private OptionOutboundCall Result { get; set; }
        private string Text { get; set; }

        public OutboundCallResult(OptionOutboundCall result, string text = "") {
            Result = result; Text = text;
        }
        public void Process() {
            string reason = GetText(Result);
            if (!string.IsNullOrWhiteSpace(Text)) reason += " - " + Text;
            CallRecordCore.Instance.Messages.Enqueue(new AppendNote(reason));
        }

        public enum OptionOutboundCall {
            Disconnect, ChangeNumber, NetworkReset, Other
        }

        public static string GetText(OptionOutboundCall option) {
            switch (option) {
                case OptionOutboundCall.Disconnect: return "Caller Disconnected";
                case OptionOutboundCall.ChangeNumber: return "Change of Number";
                case OptionOutboundCall.NetworkReset: return "Network Reset";

                default: return "Other";
            }
        }

        public static string GetToolTip(OptionOutboundCall option) {
            switch (option) {
                case OptionOutboundCall.Disconnect: return "Inbound call has dropped; calling customer back";
                case OptionOutboundCall.ChangeNumber: return "Callback to alternate number to allow testing";
                case OptionOutboundCall.NetworkReset: return "Network reset on device; calling customer back";

                default: return "Other reason - please specify: ";
            }
        }
    }
}
