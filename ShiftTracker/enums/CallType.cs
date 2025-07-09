using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace com.tybern.ShiftTracker.enums {
    public enum CallType {
        [Description("Mobile Voice / Data")]
        Mobile,

        [Description("NBN Voice / Data")]
        NBN,

        [Description("ADSL Internet")]
        ADSL,

        [Description("eMail / Bigpond")]
        eMail,

        [Description("Billing / Accounts / Sales")]
        Billing,

        [Description("Priority Assistance")]
        PA,

        [Description("Prepaid")]
        Prepaid,

        [Description("PSTN Voice")]
        PSTN,

        [Description("Opticomm / Velocity")]
        Opticomm,

        [Description("Fetch TV / Telstra TV")]
        FetchTV,

        [Description("4G Fixed / 5G Home Wireless")]
        HomeWireless,

        [Description("Platinum Support (discontinued)")]
        Platinum,

        [Description("Misrouted / Invalid Call")]
        Misrouted,

        [Description("SME Helpdesk")]
        Helpdesk,

        [Description("Additional Information")]
        Note,

        [Description("Other / Unknown")]
        Other
    }
}
