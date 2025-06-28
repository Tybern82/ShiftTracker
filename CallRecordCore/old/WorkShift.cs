using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using com.tybern.ShiftTracker.data;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.shifts {
    public class WorkShift : INotifyPropertyChanged {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

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

        public static readonly TimeSpan BREAK_LENGTH = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan LUNCH_LENGTH = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan MEET_LENGTH = TimeSpan.FromMinutes(15);

        private DateTimeOffset _Date = DateTime.Now.Date;
        public DateTimeOffset Date {
            get { return _Date; }
            set { _Date = value; OnPropertyChanged(nameof(Date)); }
        }

        private TimeSpan _StartTime = TimeSpan.Zero;
        public TimeSpan StartTime {
            get { return _StartTime; }
            set { _StartTime = value; OnPropertyChanged(nameof(StartTime)); }
        }

        private TimeSpan _EndTime = TimeSpan.Zero;
        public TimeSpan EndTime {
            get { return _EndTime; }
            set { _EndTime = value; OnPropertyChanged(nameof(EndTime)); }
        }

        private SortedSet<DBBreakRecord> BreakSet { get; } = new SortedSet<DBBreakRecord>();

        public ObservableCollection<DBBreakRecord> Breaks { get; } = new ObservableCollection<DBBreakRecord>();

        public DBBreakRecord? NextBreak { get; private set; }

        public DBBreakRecord? LastBreak { get; private set; }


        // <TextBlock Text="{Binding BreakCountdown, StringFormat='hh\\:mm\\:ss'}" />
        private TimeSpan _BreakCountdown = TimeSpan.Zero;
        public TimeSpan BreakCountdown {
            get { return _BreakCountdown; }
            set { if (_BreakCountdown != value) { _BreakCountdown = value; OnPropertyChanged(nameof(BreakCountdown)); } }
        }

        public ShiftStatus Status { get; } = new ShiftStatus();

        public WorkShift() {
            ClockTimer.GlobalTimer.ClockUpdate += (currTime) => {
                if (Status.CurrentShiftState == ShiftState.Offline) {
                    BreakCountdown = (StartTime <= currTime.TimeOfDay) ? TimeSpan.Zero : StartTime - currTime.TimeOfDay;
                } else {
                    BreakCountdown = (NextBreak is null) ? TimeSpan.Zero : ((NextBreak.StartTime.TimeOfDay <= currTime.TimeOfDay) ? TimeSpan.Zero : (NextBreak.StartTime.TimeOfDay - currTime.TimeOfDay));
                }
            };
        }

        // Used when converting over from old format
        public WorkShift(DateTime date, TimeSpan startTime, TimeSpan endTime, TimeSpan firstBreak, TimeSpan lunchBreak, TimeSpan lastBreak, TimeSpan meetingBreak, SortedSet<DBBreakRecord> extraBreaks) : this() {
            LOG.Info("Creating WorkShift: " + date + " <" + startTime + ">:<" + endTime + ">");
            this.Date = date;
            this.StartTime = startTime;
            this.EndTime = endTime;

            if (firstBreak != TimeSpan.Zero) BreakSet.Add(new DBBreakRecord(BreakType.ShiftBreak, date + firstBreak, date + firstBreak + BREAK_LENGTH));
            if (lunchBreak != TimeSpan.Zero) BreakSet.Add(new DBBreakRecord(BreakType.LunchBreak, date + lunchBreak, date + lunchBreak + LUNCH_LENGTH));
            if (lastBreak != TimeSpan.Zero) BreakSet.Add(new DBBreakRecord(BreakType.ShiftBreak, date + lastBreak, date + lastBreak + BREAK_LENGTH));
            if (meetingBreak != TimeSpan.Zero) BreakSet.Add(new DBBreakRecord(BreakType.Meeting, date + meetingBreak, date + meetingBreak + MEET_LENGTH));
            foreach (DBBreakRecord rec in extraBreaks) BreakSet.Add(rec);
            foreach (DBBreakRecord rec in BreakSet) Breaks.Add(rec);
            this.NextBreak = GetNextBreak(LastBreak);
        }

        public WorkShift(DBShiftRecord shift, List<DBBreakRecord> breaks) : this() {
            this.Date = shift.StartTime.Date;
            this.StartTime = shift.StartTime.TimeOfDay;
            this.EndTime = shift.EndTime.TimeOfDay;
            foreach (DBBreakRecord rec in breaks) BreakSet.Add(rec);
            foreach (DBBreakRecord rec in BreakSet) Breaks.Add(rec);
            this.NextBreak = GetNextBreak(LastBreak);
        }

        public void Save(SQLiteConnection dbConnection) => Save(new DBShift(dbConnection), new DBBreaks(dbConnection));

        public void Save(DBShift shiftDB, DBBreaks breaksDB) {
            DBShiftRecord shiftRecord = new DBShiftRecord();
            shiftRecord.StartTime = Date.Date + StartTime;
            shiftRecord.EndTime = Date.Date + EndTime;
            LOG.Info("Saving " + shiftRecord);
            shiftDB.AddRecord(shiftRecord);
            foreach (DBBreakRecord breakRecord in BreakSet) breaksDB.AddRecord(breakRecord);
        }

        public bool HasMeetingBreak() {
            foreach (DBBreakRecord rec in BreakSet) if (rec.Type == BreakType.Meeting) return true;
            return false;
        }

        public bool HasTraining() {
            foreach (DBBreakRecord rec in BreakSet) if (rec.Type == BreakType.Training) return true;
            return false;
        }

        public bool HasNextBreak() {
            return (NextBreak != null);
        }

        public TimeSpan TimeUntilBreak(DateTime currTime) {
            if (NextBreak == null) return TimeSpan.Zero;                                // default 0 if no break
            TimeSpan timeUntilBreak = NextBreak.StartTime.TimeOfDay - currTime.TimeOfDay;     // calculate difference between now and break start
            return (timeUntilBreak < TimeSpan.Zero) ? TimeSpan.Zero : timeUntilBreak;   // return time until break; default to 0 if already past
        }

        public SortedSet<DBBreakRecord> doStartBreak() {
            SortedSet<DBBreakRecord> _result = new SortedSet<DBBreakRecord>();

            TimeSpan currTime = DateTime.Now.TimeOfDay;

            DBBreakRecord? lastFound = NextBreak;
            TimeSpan breakLength = (NextBreak is null) ? TimeSpan.Zero : NextBreak.Length();
            LastBreak = NextBreak;
            if (!(NextBreak is null)) _result.Add(NextBreak);

            while (lastFound != null) {
                DBBreakRecord? nextBreak = GetNextBreak(lastFound);
                lastFound = null; NextBreak = nextBreak;
                if (nextBreak != null) {
                    if (nextBreak.StartTime.TimeOfDay <= (currTime + breakLength + TimeSpan.FromMinutes(5))) {    // automatically merge breaks when consecutive, or within 5 minutes of each other
                        // found extra break to add
                        _result.Add(nextBreak);
                        breakLength += nextBreak.Length();
                        LastBreak = lastFound;
                        lastFound = nextBreak;
                    }
                }
            }
            // NextBreak should be left set to the last break identified, that was not added into the list (or null if no more breaks left); LastBreak should be left with either previous NextBreak, or the last break of that block (if more than one) 

            return _result;
        }

        public void doStandardShift() {
            EndTime = StartTime + TimeSpan.FromHours(7) + TimeSpan.FromMinutes(50);
        }

        public void doExtendedShift() {
            EndTime = StartTime + TimeSpan.FromHours(7) + TimeSpan.FromMinutes(55);
        }

        public void doAddStandardBreaks() {
            doAddBreak(new DBBreakRecord(BreakType.ShiftBreak, Date.Date+StartTime, Date.Date + StartTime + TimeSpan.FromMinutes(15)));   // First Break
            doAddBreak(new DBBreakRecord(BreakType.LunchBreak, Date.Date + StartTime + TimeSpan.FromMinutes(15), Date.Date + StartTime + TimeSpan.FromMinutes(30)));   // Lunch Break
            doAddBreak(new DBBreakRecord(BreakType.ShiftBreak, Date.Date + StartTime + TimeSpan.FromMinutes(45), Date.Date + StartTime + TimeSpan.FromMinutes(15)));  // Last Break
            doAddBreak(new DBBreakRecord(BreakType.Meeting, Date.Date + StartTime + TimeSpan.FromMinutes(60), Date.Date + StartTime + TimeSpan.FromMinutes(15)));
        }

        public void doAddBreak() {
            doAddBreak(new DBBreakRecord(BreakType.ShiftBreak, Date.Date, Date.Date));
        }

        public bool doAddBreak(DBBreakRecord newBreak) {
            bool _result = BreakSet.Add(newBreak);
            if (_result) Breaks.Add(newBreak);
            this.NextBreak = GetNextBreak(LastBreak);
            return _result;
        }

        public bool doRemoveBreak(object obj) {
            return ((obj != null) && (obj is DBBreakRecord)) ? doRemoveBreak((DBBreakRecord)obj) : false;
        }

        public bool doRemoveBreak(DBBreakRecord breakToRemove) {
            bool _result = BreakSet.Remove(breakToRemove);
            if (_result) Breaks.Remove(breakToRemove);
            this.NextBreak = GetNextBreak(LastBreak);
            return _result;
        }

        public void doClearBreaks() {
            BreakSet.Clear();
            Breaks.Clear();
            LastBreak = null; NextBreak = null;
        }

        public static TimeSpan GetTotalLength(List<DBBreakRecord> breaks) {
            TimeSpan _result = TimeSpan.Zero;
            foreach (DBBreakRecord record in breaks) _result += record.Length();
            return _result;
        }

        private DBBreakRecord? GetNextBreak(DBBreakRecord? currBreak) {
            if (BreakSet.Count == 0) return null;         // no breaks in list - nothing to return
            var breakSet = BreakSet.ToImmutableSortedSet<DBBreakRecord>();
            if (currBreak == null) return breakSet[0];  // no current break, just return the first
            int currIndex = breakSet.IndexOf(currBreak);
            if (currIndex < 0) return null;             // current break not found in set
            return (currIndex + 1 < breakSet.Count) ? breakSet[currIndex + 1] : null;
        }

        public static WorkShift LoadToday() {
            DBShift dbShift = new DBShift();
            DBBreaks dbBreaks = new DBBreaks();
            List<DBShiftRecord> shiftToday = dbShift.LoadToday();
            if (shiftToday.Count > 0) { // should only be a single entry as Date is a PK
                List<DBBreakRecord> breaksToday = dbBreaks.LoadToday();
                return new WorkShift(shiftToday[0], breaksToday);
            }

            // no record found, return default
            return new WorkShift();
        }
    }
}
