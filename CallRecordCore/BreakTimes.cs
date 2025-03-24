using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace com.tybern.CallRecordCore {
    public class BreakTimes : INotifyPropertyChanged {

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Trigger PropertyChanged event when values change. This method is called when any UI/GUI exposed property is updated
        /// to trigger the notification to the UI of the change.
        /// </summary>
        /// <remarks>This method should be called outside any thread lock (ie release lock before calling this method).</remarks>
        /// <param name="propertyName">name of the property which has been modified</param>
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private TimeSpan _shiftStart = TimeSpan.Zero;
        public TimeSpan ShiftStart {
            get { lock (this) { return _shiftStart; } }
            set {
                lock (this) { _shiftStart = value; }
                if (isActive) CallRecordCore.Instance.UIProperties.BreakTimer = NextBreak;
                OnPropertyChanged(nameof(ShiftStart));
            }
        }

        private TimeSpan _shiftEnd = TimeSpan.Zero;
        public TimeSpan ShiftEnd {
            get { lock (this) { return _shiftEnd; } }
            set {
                lock (this) { _shiftEnd = value; }
                if (isActive) CallRecordCore.Instance.UIProperties.BreakTimer = NextBreak;
                OnPropertyChanged(nameof(ShiftEnd));
            }
        }

        private TimeSpan _firstBreak = TimeSpan.Zero;
        public TimeSpan FirstBreak {
            get { lock (this) { return _firstBreak; } }
            set {
                lock (this) { _firstBreak = value; }
                if (isActive) CallRecordCore.Instance.UIProperties.BreakTimer = NextBreak;
                OnPropertyChanged(nameof(FirstBreak));
            }
        }

        private TimeSpan _lastBreak = TimeSpan.Zero;
        public TimeSpan LastBreak {
            get { lock (this) { return _lastBreak; } }
            set {
                lock (this) { _lastBreak = value; }
                if (isActive) CallRecordCore.Instance.UIProperties.BreakTimer = NextBreak;
                OnPropertyChanged(nameof(LastBreak));
            }
        }

        private TimeSpan _lunchBreak = TimeSpan.Zero;
        public TimeSpan LunchBreak {
            get { lock (this) { return _lunchBreak; } }
            set {
                lock (this) { _lunchBreak = value; }
                if (isActive) CallRecordCore.Instance.UIProperties.BreakTimer = NextBreak;
                OnPropertyChanged(nameof(LunchBreak));
            }
        }

        private TimeSpan _meetingBreak = TimeSpan.Zero;
        public TimeSpan MeetingBreak {
            get { lock (this) { return _meetingBreak; } }
            set {
                lock (this) { _meetingBreak = value; }
                if (isActive) CallRecordCore.Instance.UIProperties.BreakTimer = NextBreak;
                OnPropertyChanged(nameof(MeetingBreak));
            }
        }

        private TimeSpan _trainingBreak = TimeSpan.Zero;
        public TimeSpan TrainingBreak {
            get { lock (this) { return _trainingBreak; } }
            set {
                lock (this) { _trainingBreak = value; }
                if (isActive) CallRecordCore.Instance.UIProperties.BreakTimer = NextBreak;
                OnPropertyChanged(nameof(TrainingBreak));
            }
        }

        public TimeSpan NextBreak {
            get {
                SortedList<TimeSpan, TimeSpan> lst = new SortedList<TimeSpan, TimeSpan>();
                TimeSpan currTime = DateTime.Now.TimeOfDay;
                addEntry(lst, currTime, ShiftStart);
                addEntry(lst, currTime, ShiftEnd);
                addEntry(lst, currTime, FirstBreak);
                addEntry(lst, currTime, LastBreak);
                addEntry(lst, currTime, LunchBreak);
                addEntry(lst, currTime, MeetingBreak);
                addEntry(lst, currTime, TrainingBreak);
                return (lst.Count > 0) ? lst.First<KeyValuePair<TimeSpan,TimeSpan>>().Value : TimeSpan.Zero;
            }
        }

        private void addEntry(SortedList<TimeSpan, TimeSpan> lst, TimeSpan currTime, TimeSpan entry) {
            if ((currTime <= entry) && (entry != TimeSpan.Zero) && !lst.ContainsKey(entry)) lst.Add(entry, entry);
        }

        private bool isActive = false;

        public BreakTimes(bool isActive = false) {
            this.isActive = isActive;
        }

        public void Update(BreakTimeRecord record) {
            lock (this) {
                ShiftStart = record.ShiftStart;
                ShiftEnd = record.ShiftEnd;
                FirstBreak = record.FirstBreak;
                LastBreak = record.LastBreak;
                LunchBreak = record.LunchBreak;
                MeetingBreak = record.MeetingBreak;
                TrainingBreak = record.TrainingBreak;
            }
        }
    }
}
