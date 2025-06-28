using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CallRecordCore;
using com.tybern.CMDProcessor;
using com.tybern.ShiftTracker.shifts;

namespace com.tybern.ShiftTracker.ops {
    public class StartWorkShift : Command {

        private WorkShift currShift;
        private IShiftControls shiftControls;

        public StartWorkShift(WorkShift currShift, IShiftControls shiftControls) {
            this.currShift = currShift;
            this.shiftControls = shiftControls;
        }

        public void Process() {
            currShift.Status.doStartShift();
            shiftControls.EnableShiftStart = false;
            shiftControls.EnableBreakStart = true;
            shiftControls.EnableBreakEnd = false;
            shiftControls.EnableShiftEnd = true;
        }
    }
}
