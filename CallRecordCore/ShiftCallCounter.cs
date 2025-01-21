using System;
using System.Collections.Generic;
using System.Text;

namespace CallRecordCore {
    public class ShiftCallCounter {

        public int TotalMAE { get; set; } = 0;
        public int TotalSME { get; set; } = 0;
        public int TotalCalls { get; set; } = 0;
        public int TotalDropped { get; set; } = 0;
        public TimeSpan TotalWrap { get; set; } = TimeSpan.Zero;
        public TimeSpan TotalDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan TotalSMETime { get; set; } = TimeSpan.Zero;
        public TimeSpan TotalTransferTime { get; set; } = TimeSpan.Zero;
        public int CallNumber { get; set; } = 0;
    }
}
