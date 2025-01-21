using System;
using System.Collections.Generic;
using System.Text;

namespace CallRecordCore {
    public class SkipSurvey {
        public enum SkipSurveyOption {
            Callback, Transfer, Agent, NonTelstra, None, Other
        }

        public static string getSkipSurveyOptionText(SkipSurveyOption reason) {
            switch (reason) {
                case SkipSurveyOption.Callback: return "Callback / Caller Dropped";
                case SkipSurveyOption.Agent: return "Agent / Rejected Transfer";
                case SkipSurveyOption.Transfer: return "Immediate Transfer";
                case SkipSurveyOption.NonTelstra: return "Non-Telstra Caller";
                case SkipSurveyOption.None: return "Survey Prompted";

                default: return "Other";
            }
        }

        public static string getSkipSurveyOptionTooltip(SkipSurveyOption reason) {
            switch (reason) {
                case SkipSurveyOption.Callback: return "Caller disconnected; callback or unable to reach back to the customer";
                case SkipSurveyOption.Agent: return "Did not speak with customer / Agent-only call";
                case SkipSurveyOption.Transfer: return "Caller transferred to another department without assistance";
                case SkipSurveyOption.NonTelstra: return "Non-Telstra customer on the call";
                case SkipSurveyOption.None: return "Survey was prompted/submitted normally";

                default: return "Other reason - please specify: ";
            }
        }

        public class SkipSurveyResult {
            public SkipSurveyOption Reason { get; set; }
            public string Text { get; set; }

            public SkipSurveyResult(SkipSurveyOption reason, string text = "") {
                this.Reason = reason;
                this.Text = text;
            }
        }
    }
}
