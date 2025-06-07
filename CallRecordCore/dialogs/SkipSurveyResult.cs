using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CallRecordCore.commands;
using com.tybern.CallRecordCore.old;
using static SQLite.SQLite3;

namespace com.tybern.CallRecordCore.dialogs {
    public class SkipSurveyResult : CMDProcessor.Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private OptionSkipSurvey Result { get; }
        private string Text { get; }

        public SkipSurveyResult(OptionSkipSurvey result = OptionSkipSurvey.None, string text = "") { Result = result; Text = text; }
        public void Process() {
            LOG.Info("DIALOG <SkipSurvey> : Completed");
            CallRecordCore.Instance.ShiftCounter.CallNumber++;
            bool isPrompted = Result == SkipSurveyResult.OptionSkipSurvey.None;
            CallRecordCore.Instance.Messages.Enqueue(new AddSurveyRecord(CallRecordCore.Instance.ShiftCounter.CallNumber, isPrompted, Result, (string.IsNullOrWhiteSpace(Text) ? string.Empty : Text)));
        }

        public enum OptionSkipSurvey {
            Callback, Transfer, Agent, NonTelstra, None, Other, NonFaults
        }

        public static string GetText(OptionSkipSurvey reason) {
            switch (reason) {
                case OptionSkipSurvey.Callback: return "Callback / Caller Dropped";
                case OptionSkipSurvey.Agent: return "Agent / Rejected Transfer";
                case OptionSkipSurvey.Transfer: return "Immediate Transfer";
                case OptionSkipSurvey.NonTelstra: return "Non-Telstra Caller";
                case OptionSkipSurvey.None: return "Survey Prompted";
                case OptionSkipSurvey.NonFaults: return "Non-Faults Query";

                default: return "Other";
            }
        }

        public static string GetTooltip(OptionSkipSurvey reason) {
            switch (reason) {
                case OptionSkipSurvey.Callback: return "Caller disconnected; callback or unable to reach back to the customer";
                case OptionSkipSurvey.Agent: return "Did not speak with customer / Agent-only call";
                case OptionSkipSurvey.Transfer: return "Caller transferred to another department without assistance";
                case OptionSkipSurvey.NonTelstra: return "Non-Telstra customer on the call";
                case OptionSkipSurvey.None: return "Survey was prompted/submitted normally";
                case OptionSkipSurvey.NonFaults: return "Non-Faults related query, no Natama WF started";

                default: return "Other reason - please specify: ";
            }
        }
    }
}
