using System;
using System.Collections.Generic;
using System.Text;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.db {
    public class DBShift : DBBase<DBShiftRecord> {

        public const string DBTableName = "shiftRecordv2";

        public DBShift() : this(com.tybern.CallRecordCore.CallRecordCore.Instance.Connection) { }
        public DBShift(SQLiteConnection conn) : base(DBTableName, conn) { }
        public DBShift(string dbFile) : base(DBTableName, dbFile) { }
    }

    [Table(DBShift.DBTableName)]
    public class DBShiftRecord : IComparable<DBShiftRecord> {

        [PrimaryKey, Indexed, Column("date")] public DateTime Date { get; set; } = DateTime.Now.Date;
        [Column("startTime")] public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
        [Column("endTime")] public TimeSpan EndTime { get; set; } = TimeSpan.Zero;


        public int CompareTo(DBShiftRecord other) {
            DBBase<DBShiftRecord>.ValidateCompare(other);
            return Date.CompareTo(other.Date);
        }
    }
}
