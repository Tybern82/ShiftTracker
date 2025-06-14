using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using com.tybern.CallRecordCore.commands;
using com.tybern.CallRecordCore.old;
using com.tybern.CMDProcessor;
using MimeKit.Text;

namespace com.tybern.CallRecordCore.dialogs {
    public class CallNotesResult : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public CallType Result { get; set; }
        public string Text { get; set; }

        public CallNotesResult(CallType callType, string text = "") {
            Result = callType;
            Text = text;
        }

        public void Process() {
            LOG.Info("DIALOG <CallNotes>: Completed");
            if (CallRecordCore.Instance.CurrentCall.IsInCall) {
                LOG.Info("Inline Notes");
                DateTime currTime = DateTime.Now;
                CallRecord callRecord = new CallRecord(currTime, currTime, TimeSpan.Zero, TimeSpan.Zero, 0, Result, formatNotes(Text));
                CallRecordCore.Instance.Messages.Enqueue(new AddCallRecord(callRecord));
            } else {
                LOG.Info("End-of-call Notes");
                DateTime endTime = DateTime.Now;
                TimeSpan duration = CallRecordCore.Instance.CurrentCall.WrapStartTime - CallRecordCore.Instance.CurrentCall.CallStartTime;
                CallRecordCore.Instance.ShiftCounter.TotalDuration += duration;

                TimeSpan wrap = endTime.TimeOfDay - CallRecordCore.Instance.CurrentCall.WrapStartTime;
                CallRecordCore.Instance.UIProperties.TotalWrap += wrap;

                string notes = CallRecordCore.Instance.UIProperties.Notes;
                if (CallRecordCore.Instance.CurrentCall.IsCallback) {
                    CallRecordCore.Instance.ShiftCounter.TotalDropped++;
                    notes = "Dropped / Callback\n" + notes;
                }

                CallRecordCore.Instance.ShiftCounter.CallTypeCounter.UpdateCallType(Result);
                notes = GetText(Result) + "\n" + notes;
                if (!string.IsNullOrWhiteSpace(Text)) notes += "\n" + Text;

                CallRecord callRecord = new CallRecord(CallRecordCore.fromCurrent(endTime, CallRecordCore.Instance.CurrentCall.CallStartTime), endTime, duration, wrap, CallRecordCore.Instance.UIProperties.CallMAE, Result, formatNotes(notes), 
                    CallRecordCore.Instance.CurrentCall.IsANGenerated, CallRecordCore.Instance.CurrentCall.IsANEdited, CallRecordCore.Instance.CurrentCall.IsANSaved, CallRecordCore.Instance.CurrentCall.HasManualNotes);
                CallRecordCore.Instance.Messages.Enqueue(new AddCallRecord(callRecord));

                CallRecordCore.Instance.UIProperties.CallMAE = 0;
                CallRecordCore.Instance.UIProperties.CallSME = 0;
                CallRecordCore.Instance.UIProperties.Notes = "";      // set the field to empty value
            }
        }

        private static Regex reg = new Regex("(\r\n|\n|\r)");
        private static string formatNotes(string notes) {
            // Replaces newline with semicolon
            return reg.Replace(notes, "; ");
        }

        public enum CallType {
            Mobile, NBN, ADSL, eMail, Billing, PA, Prepaid, PSTN, Opticomm, FetchTV, HomeWireless, Platinum, Misrouted, Helpdesk, Other, Note
        } 

        public static string GetText(CallType option) {
            switch (option) {
                case CallType.Mobile: return "Mobile Voice / Data";
                case CallType.NBN: return "NBN Voice / Data";
                case CallType.ADSL: return "ADSL Internet";
                case CallType.eMail: return "eMail / Bigpond";
                case CallType.Billing: return "Billing / Accounts / Sales";
                case CallType.PA: return "Priority Assist";
                case CallType.Prepaid: return "Prepaid";
                case CallType.PSTN: return "PSTN Voice";
                case CallType.Opticomm: return "Opticomm / Velocity";
                case CallType.FetchTV: return "Fetch TV / Telstra TV";
                case CallType.HomeWireless: return "4G Fixed / 5G Home Wireless";
                case CallType.Platinum: return "Platinum Support";
                case CallType.Misrouted: return "Misrouted / Invalid Call";
                case CallType.Helpdesk: return "SME Helpdesk";
                case CallType.Note: return "Additional Information";

                default: return "Other";
            }
        }

        public static string GetToolTip(CallType option) {
            switch (option) {
                case CallType.Mobile: return "Mobile Voice / Data Faults";
                case CallType.NBN: return "Consumer NBN Voice / Data Faults";
                case CallType.ADSL: return "ADSL Internet Faults";
                case CallType.eMail: return "eMail / Bigpond";
                case CallType.Billing: return "Billing / Accounts / Sales";
                case CallType.PA: return "Priority Assistance";
                case CallType.Prepaid: return "Prepaid Billing Team";
                case CallType.PSTN: return "PSTN / Fixed line Voice";
                case CallType.Opticomm: return "Opticomm / Velocity";
                case CallType.FetchTV: return "Fetch TV / Telstra TV";
                case CallType.HomeWireless: return "4G Fixed / 5G Home Wireless";
                case CallType.Platinum: return "Platinum Support";
                case CallType.Misrouted: return "Misrouted Agent / Non-Faults Team / SME Customer Call";
                case CallType.Helpdesk: return "SME Helpdesk";
                case CallType.Note: return "Additional Information";

                default: return "Other reason - please specify: ";
            }
        }
    }
}
