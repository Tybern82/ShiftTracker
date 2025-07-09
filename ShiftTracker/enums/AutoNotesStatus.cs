using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace com.tybern.ShiftTracker.enums {
    [Flags]
    public enum AutoNotesStatus {
        [Description("Notes Generated")]
        Generated = 1,

        [Description("Notes required Editing")]
        Edited = 2,

        [Description("Notes saved to Case")]
        Saved = 4,

        [Description("Added Manual Note to Case")]
        Manual = 8,

        [Description("No Notes")]
        None = 0,
    }
}
