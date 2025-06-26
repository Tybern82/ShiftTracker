using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace com.tybern.ShiftTracker {
    public class ClockTimer {

        public static ClockTimer GlobalTimer { get; } = new ClockTimer();

        public delegate void ClockTask(DateTime currTime);

        public event ClockTask? ClockUpdate;

        private Timer _clockTimer;

        private void doClockUpdate(object? o) {
            DateTime currTime = DateTime.Now;
            ClockUpdate?.Invoke(currTime);
        }

        public ClockTimer() {
            _clockTimer = new Timer(doClockUpdate, this, 2000, 1000);
        }
    }
}
