using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.db {
    public class DBBreaks : DBBase<DBBreakRecord> {

        public const string DBTableName = "breakTimesv2";

        public DBBreaks() : this (com.tybern.CallRecordCore.CallRecordCore.Instance.Connection) { }
        public DBBreaks(SQLiteConnection conn) : base(DBTableName, conn) { }
        public DBBreaks(string dbFile) : base(DBTableName, dbFile) { }
    }

    public enum BreakType {
        [Description("Break")]
        ShiftBreak,

        [Description("Lunch Break")]
        LunchBreak,

        [Description("Meeting")]
        Meeting,

        [Description("Training")]
        Training,

        [Description("Coaching Session")]
        Coaching,

        [Description("Fault / System Issue")]
        SystemIssue,

        [Description("Personal / Sick Leave")]
        PersonalLeave,

        [Description("Public Holiday")]
        PublicHoliday
    };

    [Table(DBBreaks.DBTableName)]
    public class DBBreakRecord : IComparable<DBBreakRecord>, INotifyPropertyChanged {

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

        private static IEnumerable<BreakType> MODELS = Enum.GetValues(typeof(BreakType)).Cast<BreakType>();

        [Ignore] public IEnumerable<BreakType> Models => MODELS;

        [PrimaryKey, Indexed, Column("date")] public DateTime Date { get; set; } = DateTime.Now.Date;

        private TimeSpan _StartTime = TimeSpan.Zero;
        [PrimaryKey, Column("startTime")] public TimeSpan StartTime { 
            get { return _StartTime; }
            set { 
                _StartTime = value; OnPropertyChanged(nameof(StartTime)); 
                // Auto-Update end time for known items
                switch (Type) {
                    case BreakType.ShiftBreak:
                        EndTime = StartTime + TimeSpan.FromMinutes(15); break;

                    case BreakType.LunchBreak:
                        EndTime = StartTime + TimeSpan.FromMinutes(30); break;

                    case BreakType.Meeting:
                        EndTime = StartTime + TimeSpan.FromMinutes(15); break;

                }
            }
        }

        private TimeSpan _EndTime = TimeSpan.Zero;
        [Column("endTime")] public TimeSpan EndTime { 
            get { return _EndTime; }
            set { _EndTime = value; OnPropertyChanged(nameof(EndTime)); }
        }
        
        [Column("breakType")] public string BreakTypeText {
            get { return Type.ToString(); }
            set { Type = (BreakType)Enum.Parse(typeof(BreakType), value, true); }
        }
        
        [Ignore] public BreakType Type { get; set; } = BreakType.ShiftBreak;

        public DBBreakRecord() { }

        public DBBreakRecord(DateTime date, BreakType type, TimeSpan startTime, TimeSpan endTime) {
            Date = date;
            Type = type;
            StartTime = startTime;
            EndTime = endTime;
        }

        public TimeSpan Length() {
            return (EndTime - StartTime);
        }

        public int CompareTo(DBBreakRecord other) {
            DBBase<DBBreakRecord>.ValidateCompare(other);
            int _result = Date.CompareTo(other.Date);
            return (_result == 0) ? StartTime.CompareTo(other.StartTime) : _result;
        }
    }
}
