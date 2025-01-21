using System;
using System.Collections.Generic;
using System.Text;

namespace CallRecordCore {
    public class OutboundCall {

        public enum OutboundCallOption {
            Disconnect, ChangeNumber, Other
        }

        public static string getOutboundCallOptionText(OutboundCallOption option) {
            switch (option) {
                case OutboundCallOption.Disconnect:     return "Caller Disconnected";
                case OutboundCallOption.ChangeNumber:   return "Change of Number";

                default:    return "Other";
            }
        }

        public static string getOutboundCallOptionTooltip(OutboundCallOption option) {
            switch (option) {
                case OutboundCallOption.Disconnect:     return "Inbound call has dropped; calling customer back";
                case OutboundCallOption.ChangeNumber:   return "Callback to alternate number to allow testing";

                default:    return "Other reason - please specify: ";
            }
        }

        public class OutboundCallResult {
            public OutboundCallOption Reason { get; set; }
            public string Text { get; set; }

            public OutboundCallResult(OutboundCallOption option, string text = "") {
                Reason = option;
                Text = text;
            }
        }
    }
}
