using System;
using System.Collections.Generic;
using System.Text;

namespace com.tybern.ShiftTracker {
    public interface IShiftControls {

        public event CommandEvent? ShiftStart;
        public event CommandEvent? ShiftEnd;
        public event CommandEvent? BreakStart;
        public event CommandEvent? BreakEnd;

        public bool EnableShiftStart { set; }
        public bool EnableShiftEnd { set; }
        public bool EnableBreakStart { set; }
        public bool EnableBreakEnd { set; }

    }
}
