using System;
using System.Collections.Generic;
using System.Text;

namespace CallRecordCore {
    public class SMERequest {

        public enum SMERequestOption {
            Tools, Query
        }

        public static string getSMERequestOptionText(SMERequestOption option) {
            switch (option) {
                case SMERequestOption.Tools: 
                    return "Systems / Program Access";

                case SMERequestOption.Query:
                default: 
                    return "General Query";
            }
        }

        public static string getSMERequestOptionTooltip(SMERequestOption option) {
            switch (option) {
                case SMERequestOption.Tools: 
                    return "Agent does not have access to the required program / tool to complete request";

                case SMERequestOption.Query:
                default:
                    return "Agent general query of SME chat for confirmation / advice";
            }
        }

        public class SMERequestResult {
            public SMERequestOption Type;
            public string Text;

            public SMERequestResult(SMERequestOption type, string text = "") {
                Type = type;
                Text = text;
            }
        }
    }
}
