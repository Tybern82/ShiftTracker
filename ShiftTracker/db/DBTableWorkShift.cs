using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.ShiftTracker.data;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.db {

    [Table("shifts")]
    public class DBTableWorkShift {

        [Ignore] public DateTime Date { get; set; } = DateTime.Now.Date;
        [PrimaryKey, Indexed, Column("date")] public string DateText {
            get { return Date.ToString(DBShiftTracker.FORMAT_DATE); }
            set { Date = DateTime.Parse(value); }
        }

        [Ignore] public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
        [Column("startTime"), NotNull] public string StartTimeText {
            get { return StartTime.ToString(DBShiftTracker.FORMAT_TIME); }
            set { StartTime = TimeSpan.Parse(value); }
        }

        [Ignore] public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
        [Column("endTime"), NotNull] public string EndTimeText {
            get { return EndTime.ToString(DBShiftTracker.FORMAT_TIME); }
            set { EndTime = TimeSpan.Parse(value); }
        }

        public DBTableWorkShift() { }

        public DBTableWorkShift(WorkShift shift) {
            this.Date = shift.CurrentDate;
            this.StartTime = shift.StartTime;
            this.EndTime = shift.EndTime;
        }

        public WorkShift asWorkShift() {
            WorkShift _result = new WorkShift(Date) {
                StartTime = this.StartTime,
                EndTime = this.EndTime
            };
            _result.Breaks.Clear();
            SortedSet<WorkBreak> breaks = DBShiftTracker.Instance.loadWorkBreaks(Date);
            foreach (WorkBreak brk in breaks) _result.Breaks.Add(brk);
            return _result;
        }
    }
}
