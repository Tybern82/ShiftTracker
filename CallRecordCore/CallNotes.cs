using System;
using System.Collections.Generic;
using System.Text;
using MimeKit.Text;

namespace CallRecordCore {
    public class CallNotes {

        public enum CallType {
            Mobile, NBN, ADSL, eMail, Billing, PA, Prepaid, PSTN, Opticomm, FetchTV, HomeWireless, Platinum, Misrouted, Helpdesk, Other
        }

        public static string getCallNotesOptionText(CallType option) {
            switch (option) {
                case CallType.Mobile:        return "Mobile Voice / Data";
                case CallType.NBN:           return "NBN Voice / Data";
                case CallType.ADSL:          return "ADSL Internet";
                case CallType.eMail:         return "eMail / Bigpond";
                case CallType.Billing:       return "Billing / Accounts / Sales";
                case CallType.PA:            return "Priority Assist";
                case CallType.Prepaid:       return "Prepaid";
                case CallType.PSTN:          return "PSTN Voice";
                case CallType.Opticomm:      return "Opticomm / Velocity";
                case CallType.FetchTV:       return "Fetch TV / Telstra TV";
                case CallType.HomeWireless:  return "4G Fixed / 5G Home Wireless";
                case CallType.Platinum:      return "Platinum Support";
                case CallType.Misrouted:     return "Misrouted / Invalid Call";
                case CallType.Helpdesk:      return "SME Helpdesk";

                default:    return "Other";
            }
        }

        public static string getCallNotesOptionTooltip(CallType option) {
            switch (option) {
                case CallType.Mobile:        return "Mobile Voice / Data Faults";
                case CallType.NBN:           return "Consumer NBN Voice / Data Faults";
                case CallType.ADSL:          return "ADSL Internet Faults";
                case CallType.eMail:         return "eMail / Bigpond";
                case CallType.Billing:       return "Billing / Accounts / Sales";
                case CallType.PA:            return "Priority Assistance";
                case CallType.Prepaid:       return "Prepaid Billing Team";
                case CallType.PSTN:          return "PSTN / Fixed line Voice";
                case CallType.Opticomm:      return "Opticomm / Velocity";
                case CallType.FetchTV:       return "Fetch TV / Telstra TV";
                case CallType.HomeWireless:  return "4G Fixed / 5G Home Wireless";
                case CallType.Platinum:      return "Platinum Support";
                case CallType.Misrouted:     return "Misrouted Agent / Non-Faults Team / SME Customer Call";
                case CallType.Helpdesk:      return "SME Helpdesk";

                default:    return "Other reason - please specify: ";
            }
        }

        public class CallNotesResult {
            public CallType CallType { get; set; }
            public string Text { get; set; }

            public CallNotesResult(CallType callType, string text = "") {
                CallType = callType;
                Text = text;
            }
        }
    }
}
