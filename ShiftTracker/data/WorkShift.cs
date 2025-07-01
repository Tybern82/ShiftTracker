using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace com.tybern.ShiftTracker.data {
    public class WorkShift : INotifyPropertyChanged, IComparable<WorkShift> {

        public static readonly TimeSpan BREAK_LENGTH = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan LUNCH_LENGTH = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan MEET_LENGTH = TimeSpan.FromMinutes(15);

        private DateTime _CurrentDate;
        public DateTime CurrentDate { 
            get { return _CurrentDate; }
            set { _CurrentDate = value; onPropertyChanged(nameof(CurrentDate)); }
        }

        public DateTimeOffset CurrentDateOffset {
            get { return CurrentDate; }
            set { CurrentDate = value.Date; }
        }

        private TimeSpan _StartTime;
        public TimeSpan StartTime {
            get { return _StartTime; }
            set { _StartTime = value; onPropertyChanged(nameof(StartTime)); }
        }

        private TimeSpan _EndTime;
        public TimeSpan EndTime {
            get { return _EndTime; }
            set { _EndTime = value; onPropertyChanged(nameof(EndTime)); }
        }

        public ObservableCollection<WorkBreak> Breaks { get; } = new ObservableCollection<WorkBreak>();

        public WorkShift(DateTime dt) {
            CurrentDate = dt;
        }

        public void doAddBreak() {
            Breaks.Add(new WorkBreak() { Type = BreakType.ShiftBreak, CurrentDate = this.CurrentDate, StartTime = this.StartTime, EndTime = this.StartTime });
        }

        public void doRemoveBreak(WorkBreak brk) {
            Breaks.Remove(brk);
        }

        public void doAddStandardBreaks() {
            Breaks.Add(new WorkBreak() { Type = BreakType.ShiftBreak, CurrentDate = this.CurrentDate, StartTime = this.StartTime, EndTime = this.StartTime + TimeSpan.FromMinutes(15) });
            Breaks.Add(new WorkBreak() { Type = BreakType.LunchBreak, CurrentDate = this.CurrentDate, StartTime = this.StartTime + TimeSpan.FromMinutes(15), EndTime = this.StartTime + TimeSpan.FromMinutes(30) });
            Breaks.Add(new WorkBreak() { Type = BreakType.ShiftBreak, CurrentDate = this.CurrentDate, StartTime = this.StartTime + TimeSpan.FromMinutes(30), EndTime = this.StartTime + TimeSpan.FromMinutes(45) });
            Breaks.Add(new WorkBreak() { Type = BreakType.Meeting, CurrentDate = this.CurrentDate, StartTime = this.StartTime + TimeSpan.FromMinutes(45), EndTime = this.StartTime + TimeSpan.FromMinutes(60) });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void onPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public int CompareTo(WorkShift other) => CurrentDate.CompareTo(other.CurrentDate);
    }
}
