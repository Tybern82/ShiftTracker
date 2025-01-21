using System;
using System.Collections.Generic;
using System.Text;

namespace CallRecordCore {
    public class CallTypeCounter {

        public int Mobile { get; set; } = 0;
        public int NBN { get; set; } = 0;
        public int ADSL { get; set; } = 0;
        public int eMail { get; set; } = 0;
        public int Billing { get; set; } = 0;
        public int PriorityAssist { get; set; } = 0;
        public int Prepaid { get; set; } = 0;
        public int PSTN { get; set; } = 0;
        public int Opticomm {  get; set; } = 0;
        public int FetchTV { get; set; } = 0;
        public int HomeWireless { get; set; } = 0;
        public int Platinum { get; set; } = 0;
        public int Misrouted { get; set; } = 0;
        public int Helpdesk { get; set; } = 0;
        public int Other { get; set; } = 0;
    }
}
