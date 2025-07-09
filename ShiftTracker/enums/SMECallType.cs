using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace com.tybern.ShiftTracker.enums {

    public enum SMECallType {
        [Description("Systems / Program Access")]
        [TooltipDescription("Agent does not have access to required tool/program")]
        Tools,

        [Description("General Query")]
        [TooltipDescription("General assistance; confirm possible system issue; etc")]
        Query
    }
}
