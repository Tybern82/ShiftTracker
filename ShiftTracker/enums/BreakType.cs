using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace com.tybern.ShiftTracker.enums {
    public enum BreakType {
        [Description("Break")]
        [TooltipDescription("15-Minute Health Break")]
        ShiftBreak,

        [Description("Lunch Break")]
        [TooltipDescription("30-Minute Lunch Break (unpaid)")]
        LunchBreak,

        [Description("Meeting")]
        [TooltipDescription("Team Meeting")]
        Meeting,

        [Description("Training")]
        [TooltipDescription("Scheduled Training; Leader Led / Self-Directed")]
        Training,

        [Description("Coaching Session")]
        [TooltipDescription("TL Coaching Session / CUE Feedback")]
        Coaching,

        [Description("Fault / System Issue")]
        [TooltipDescription("PC/Laptop Fault; System Forced Restart; BigIP Drop; etc")]
        SystemIssue,

        [Description("Personal / Sick Leave")]
        [TooltipDescription("Personal / Sick Leave (paid)")]
        PersonalLeave,

        [Description("Unpaid Leave")]
        [TooltipDescription("Personal / Sick Leave (unpaid)")]
        UnpaidLeave,

        [Description("Public Holiday")]
        [TooltipDescription("Public Holiday - not required to work")]
        PublicHoliday,

        [Description("Annual Leave")]
        [TooltipDescription("Annual Leave (paid)")]
        AnnualLeave
    }
}
