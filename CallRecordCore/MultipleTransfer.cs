using System;
using System.Collections.Generic;
using System.Text;

namespace CallRecordCore {
    public class MultipleTransfer {

        public enum MultipleTransferOption {
            Misrouted, CallDrop, AgentDrop, Incorrect, Other
        }

        public static string getMultipleTransferOptionsText(MultipleTransferOption options) {
            switch (options) {
                case MultipleTransferOption.Misrouted: return "Call Misrouted";
                case MultipleTransferOption.CallDrop:  return "Caller Disconnected";
                case MultipleTransferOption.AgentDrop: return "Agent Disconnected";
                case MultipleTransferOption.Incorrect: return "Incorrect Department";

                default:    return "Other";
            }
        }

        public static string getMultipleTransferOptionsTooltip(MultipleTransferOption options) {
            switch (options) {
                case MultipleTransferOption.Misrouted: return "Call misrouted to incorrect department";
                case MultipleTransferOption.CallDrop:  return "Caller disconnected - callback and re-transfer";
                case MultipleTransferOption.AgentDrop: return "Agent disconnected - callback to same department";
                case MultipleTransferOption.Incorrect: return "Incorrect department - referred to alternate department";

                default:    return "Other reason - please specify: ";
            }
        }

        public class MultipleTransferResult {
            public MultipleTransferOption Reason { get; set; }
            public string Text { get; set; }

            public MultipleTransferResult(MultipleTransferOption options, string text = "") {
                Reason = options;
                Text = text;
            }
        }
    }
}
