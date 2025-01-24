using System;
using System.Collections.Generic;
using System.Text;

namespace com.tybern.CallRecordCore {
    public class ShiftCallCounter {

        public int TotalSME { get; set; } = 0;
        public int TotalDropped { get; set; } = 0;
        public TimeSpan TotalDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan TotalSMETime { get; set; } = TimeSpan.Zero;
        public TimeSpan TotalTransferTime { get; set; } = TimeSpan.Zero;
        public int CallNumber { get; set; } = 0;

        /// <summary>
        /// Tracks calls by type across the shift.
        /// </summary>
        public CallTypeCounter CallTypeCounter { get; } = new CallTypeCounter();
    }
}
