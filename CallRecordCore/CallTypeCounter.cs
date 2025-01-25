using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CallRecordCore.dialogs;
using static com.tybern.CallRecordCore.dialogs.CallNotesResult;

namespace com.tybern.CallRecordCore {
    public class CallTypeCounter {

        public int Mobile { get; set; } = 0;
        public int NBN { get; set; } = 0;
        public int ADSL { get; set; } = 0;
        public int eMail { get; set; } = 0;
        public int Billing { get; set; } = 0;
        public int PriorityAssist { get; set; } = 0;
        public int Prepaid { get; set; } = 0;
        public int PSTN { get; set; } = 0;
        public int Opticomm { get; set; } = 0;
        public int FetchTV { get; set; } = 0;
        public int HomeWireless { get; set; } = 0;
        public int Platinum { get; set; } = 0;
        public int Misrouted { get; set; } = 0;
        public int Helpdesk { get; set; } = 0;
        public int Other { get; set; } = 0;

        public void UpdateCallType(CallNotesResult.CallType type) {
            CallTypeCounter callTypeCounter = CallRecordCore.Instance.ShiftCounter.CallTypeCounter;
            switch (type) {
                case CallNotesResult.CallType.Mobile: callTypeCounter.Mobile++; break;
                case CallNotesResult.CallType.NBN: callTypeCounter.NBN++; break;
                case CallNotesResult.CallType.ADSL: callTypeCounter.ADSL++; break;
                case CallNotesResult.CallType.eMail: callTypeCounter.eMail++; break;
                case CallNotesResult.CallType.Billing: callTypeCounter.Billing++; break;
                case CallNotesResult.CallType.PA: callTypeCounter.PriorityAssist++; break;
                case CallNotesResult.CallType.Prepaid: callTypeCounter.Prepaid++; break;
                case CallNotesResult.CallType.PSTN: callTypeCounter.PSTN++; break;
                case CallNotesResult.CallType.Opticomm: callTypeCounter.Opticomm++; break;
                case CallNotesResult.CallType.FetchTV: callTypeCounter.FetchTV++; break;
                case CallNotesResult.CallType.HomeWireless: callTypeCounter.HomeWireless++; break;
                case CallNotesResult.CallType.Platinum: callTypeCounter.Platinum++; break;
                case CallNotesResult.CallType.Misrouted: callTypeCounter.Misrouted++; break;
                case CallNotesResult.CallType.Helpdesk: callTypeCounter.Helpdesk++; break;

                default: callTypeCounter.Other++; break;
            }
        }

        public override string ToString() {
            string _result = "";
            _result += addItem(Mobile, "Mobile");
            _result += addItem(NBN, "NBN");
            _result += addItem(ADSL, "ADSL");
            _result += addItem(eMail, "eMail");
            _result += addItem(Billing, "Billing");
            _result += addItem(PriorityAssist, "Priority Assist");
            _result += addItem(Prepaid, "Prepaid");
            _result += addItem(PSTN, "PSTN");
            _result += addItem(Opticomm, "Opticomm");
            _result += addItem(FetchTV, "Fetch / Telstra TV");
            _result += addItem(HomeWireless, "4G Fixed / 5G Home Wireless");
            _result += addItem(Platinum, "Platinum");
            _result += addItem(Helpdesk, "Helpdesk");
            _result += addItem(Misrouted+Other, "Misrouted / Other");
            return _result;
        }

        private string addItem(int item, string name) {
            return (item > 0) ? name + ": " + item + "; " : string.Empty;
        }
    }
}
