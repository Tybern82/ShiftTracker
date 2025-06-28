using System;
using System.Collections.Generic;
using System.Text;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.data {
    public class DBShift : DBBase<DBShiftRecord> {

        public const string DBTableName = "shiftRecordv2";

        public DBShift() : this(com.tybern.CallRecordCore.CallRecordCore.Instance.Connection) { }
        public DBShift(SQLiteConnection conn) : base(DBTableName, conn) { }
        public DBShift(string dbFile) : base(DBTableName, dbFile) { }
    }

    [Table(DBShift.DBTableName)]
    public class DBShiftRecord : IComparable<DBShiftRecord> {

        [PrimaryKey, Indexed, Column("startTime")] public DateTime StartTime { get; set; } = DateTime.Now;
        [Column("endTime")] public DateTime EndTime { get; set; } = DateTime.Now;

        [Ignore] public DateTime Date { get { return StartTime.Date; } }

        /*
        [PrimaryKey, Indexed, Column("date")] public DateTime Date { get; set; } = DateTime.Now.Date;
        [Column("startTime")] public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
        [Column("endTime")] public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
        */

        public override string ToString() {
            return "SHIFT: <" + StartTime + ">:<" + EndTime + ">";
        }

        public int CompareTo(DBShiftRecord other) {
            DBBase<DBShiftRecord>.ValidateCompare(other);
            return StartTime.CompareTo(other.StartTime);
        }
    }
}
