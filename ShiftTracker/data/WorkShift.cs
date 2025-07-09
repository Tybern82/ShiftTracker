using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using com.tybern.ShiftTracker.enums;

namespace com.tybern.ShiftTracker.data {
    public enum WorkShiftState {
        [Description("Offline")]
        Offline,

        [Description("In Calls")]
        InCalls,

        [Description("In Break")]
        OnBreak 
    }

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

            NextBreak = GetNextBreak(LastBreak);
        }

        public WorkBreak? NextBreak { get; private set; }
        public WorkBreak? LastBreak { get; private set; }

        public void doStartShift() {
            if (NextBreak == null) NextBreak = GetNextBreak(LastBreak);
        }

        public void doEndShift() {}

        public void doEndBreak() {}

        public void matchState(WorkShift other) {
            this.LastBreak = other.LastBreak;
            this.NextBreak = GetNextBreak(LastBreak);   // recalculate NextBreak as this may have changed
        }

        public SortedSet<WorkBreak> doStartBreak() {
            SortedSet<WorkBreak> _result = new SortedSet<WorkBreak>();

            TimeSpan currTime = DateTime.Now.TimeOfDay;

            WorkBreak? lastFound = NextBreak;
            TimeSpan breakLength = (NextBreak is null) ? TimeSpan.Zero : NextBreak.Length;
            LastBreak = NextBreak;
            if (!(NextBreak is null)) _result.Add(NextBreak);

            while (lastFound != null) {
                WorkBreak? nextBreak = GetNextBreak(lastFound);
                lastFound = null; NextBreak = nextBreak;
                if (nextBreak != null) {
                    if (nextBreak.StartTime <= (currTime + breakLength + TimeSpan.FromMinutes(5))) {    // automatically merge breaks when consecutive, or within 5 minutes of each other
                        // found extra break to add
                        _result.Add(nextBreak);
                        breakLength += nextBreak.Length;
                        LastBreak = lastFound;
                        lastFound = nextBreak;
                    }
                }
            }
            // NextBreak should be left set to the last break identified, that was not added into the list (or null if no more breaks left); LastBreak should be left with either previous NextBreak, or the last break of this block (if more than one) 

            return _result;
        }

        private WorkBreak? GetNextBreak(WorkBreak? currBreak) {
            SortedSet<WorkBreak> BreakSet = new SortedSet<WorkBreak>();
            foreach (WorkBreak brk in Breaks) if (typeIn(brk.Type, new[] { BreakType.ShiftBreak, BreakType.LunchBreak, BreakType.Meeting, BreakType.Coaching, BreakType.Training })) BreakSet.Add(brk);
            if (BreakSet.Count == 0) return null;         // no breaks in list - nothing to return
            var breakSet = BreakSet.ToImmutableSortedSet<WorkBreak>();
            if (currBreak == null) return breakSet[0];  // no current break, just return the first
            int currIndex = breakSet.IndexOf(currBreak);
            if (currIndex < 0) return null;             // current break not found in set
            return (currIndex + 1 < breakSet.Count) ? breakSet[currIndex + 1] : null;   // return the next break if there is one
        }

        private bool typeIn(BreakType type, BreakType[] list) {
            foreach (BreakType t in list)
                if (type == t) return true;
            return false;
        }

        public void doAddBreak() {
            Breaks.Add(new WorkBreak() { Type = BreakType.ShiftBreak, CurrentDate = this.CurrentDate, StartTime = this.StartTime, EndTime = this.StartTime });
            this.NextBreak = GetNextBreak(LastBreak);   // recalculate NextBreak as this may have changed
        }

        public void doRemoveBreak(WorkBreak brk) {
            Breaks.Remove(brk);
            this.NextBreak = GetNextBreak(LastBreak);   // recalculate NextBreak as this may have changed
        }

        public void doAddStandardBreaks() {
            Breaks.Add(new WorkBreak() { Type = BreakType.ShiftBreak, CurrentDate = this.CurrentDate, StartTime = this.StartTime, EndTime = this.StartTime + BREAK_LENGTH });
            Breaks.Add(new WorkBreak() { Type = BreakType.LunchBreak, CurrentDate = this.CurrentDate, StartTime = this.StartTime + BREAK_LENGTH, EndTime = this.StartTime + BREAK_LENGTH + LUNCH_LENGTH });
            Breaks.Add(new WorkBreak() { Type = BreakType.ShiftBreak, CurrentDate = this.CurrentDate, StartTime = this.StartTime + BREAK_LENGTH + LUNCH_LENGTH, EndTime = this.StartTime + BREAK_LENGTH + LUNCH_LENGTH + BREAK_LENGTH });

            if (!(CurrentDate.DayOfWeek == DayOfWeek.Saturday || CurrentDate.DayOfWeek == DayOfWeek.Sunday))    // only add Meeting to shifts on Mon-Fri (ie not Sat/Sun)
                if ((this.StartTime <= TrackerSettings.Instance.MeetingTime) && (this.EndTime >= TrackerSettings.Instance.MeetingTime + MEET_LENGTH))  // only add Meeting if it will fall within the current shift
                    Breaks.Add(new WorkBreak() { Type = BreakType.Meeting, CurrentDate = this.CurrentDate, StartTime = TrackerSettings.Instance.MeetingTime, EndTime = TrackerSettings.Instance.MeetingTime + MEET_LENGTH });
            this.NextBreak = GetNextBreak(LastBreak);   // recalculate NextBreak as this may have changed
        }

        public void doAddAllDayBreak() {
            Breaks.Add(new WorkBreak() { Type = BreakType.PersonalLeave, CurrentDate = this.CurrentDate, StartTime = this.StartTime, EndTime = this.EndTime });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void onPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public int CompareTo(WorkShift other) => CurrentDate.CompareTo(other.CurrentDate);
    }
}
