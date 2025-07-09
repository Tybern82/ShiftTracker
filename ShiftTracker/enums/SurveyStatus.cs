using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace com.tybern.ShiftTracker.enums {

    public enum SurveyStatus {
        [Description("Survey Requested")]
        [TooltipDescription("Survey was prompted / requested normally")]
        SurveyRequested,

        [Description("Callback / Caller Dropped")]
        [TooltipDescription("Caller disconnected; callback or unable to reach back to the customer")]
        Callback,

        [Description("Agent Call / Incomplete Transfer")]
        [TooltipDescription("Did not speak with customer / Agent-only call")]
        Agent,

        [Description("Non-Telstra Caller")]
        [TooltipDescription("Caller could not be authenticated / identified, or was not a Telstra customer")]
        NonTelstra,

        [Description("Non-Faults Query")]
        [TooltipDescription("Non-Faults related query; no Natama WF started to trigger Survey")]
        NonFaults,

        [Description("Caller Transferred")]
        [TooltipDescription("Non-Faults related query; customer was transferred to another departement for assistance")]
        Transfer,

        [Description("Unspecified")]
        [TooltipDescription("Unspecified reason")]
        Unspecified,

        [Description("Missing")]
        Missing
    }

    public class SurveyStatusUtility {

        private static IEnumerable<SurveyStatus>? MODELS;

        public static IEnumerable<SurveyStatus> generateModels() {
            if (MODELS == null) {
                // Generate on first use
                var values = Enum.GetValues(typeof(SurveyStatus));
                List<SurveyStatus> nonMissing = new List<SurveyStatus>();
                foreach (var v in values) {
                    if (SurveyStatus.Missing != (SurveyStatus)v) nonMissing.Add((SurveyStatus)v);
                }
                MODELS = nonMissing;
            }
            return MODELS;
        }
    }
}
