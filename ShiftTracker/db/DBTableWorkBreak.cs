using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.ShiftTracker.data;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.db {
    [Table("breakTimes")]
    public class DBTableWorkBreak {

        [Ignore] public DateTime Date { get; set; } = DateTime.Now.Date;
        [PrimaryKey, Indexed, Column("date")] public string DateText {
            get { return Date.ToString(DBShiftTracker.FORMAT_DATE); }
            set { Date = DateTime.Parse(value); }
        }

        [Ignore] public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
        [PrimaryKey, Indexed, Column("startTime")] public string StartTimeText {
            get { return StartTime.ToString(DBShiftTracker.FORMAT_TIME); }
            set { StartTime = TimeSpan.Parse(value); }
        }

        [Ignore] public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
        [Column("endTime"), NotNull] public string EndTimeText {
            get { return EndTime.ToString(DBShiftTracker.FORMAT_TIME); }
            set { EndTime = TimeSpan.Parse(value); }
        }

        [Ignore] public BreakType Type { get; set; } = BreakType.ShiftBreak;
        [Column("breakType"), NotNull] public string BreakTypeName {
            get { return Type.ToString(); }
            set { Type = (BreakType)Enum.Parse(typeof(BreakType), value, true); }
        }

        public DBTableWorkBreak() { }

        public DBTableWorkBreak(WorkBreak brk) {
            this.Date = brk.CurrentDate;
            this.StartTime = brk.StartTime;
            this.EndTime = brk.EndTime;
            this.Type = brk.Type;
        }

        public WorkBreak asWorkBreak() {
            return new WorkBreak() { 
                CurrentDate = this.Date, 
                StartTime = this.StartTime, 
                EndTime = this.EndTime, 
                Type = this.Type 
            };
        }
    }
}
