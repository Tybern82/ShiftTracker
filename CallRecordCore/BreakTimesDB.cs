using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.tybern.CallRecordCore.commands;
using SQLite.Net2;

namespace com.tybern.CallRecordCore {
    public class BreakTimesDB {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public SQLiteConnection conn;

        public BreakTimesDB(SQLiteConnection conn) {
            this.conn = conn;

            conn.CreateTable<BreakTimeRecord>();
        }

        public void LoadBreakTimes(DateTime date, BreakTimes breakTimes) {
            try {
                var data = conn.Table<BreakTimeRecord>().Where(record => record.Date == date).ToList();
                // const string query = "SELECT * FROM breakTimes WHERE date IS @date";
                // var cmd = new SQLiteCommand(conn);
                // cmd.CommandText = query;
                // cmd.Bind("@date", date.Date.Ticks);
                // var data = cmd.ExecuteQuery<BreakTimeRecord>();
                bool updated = false;
                if (data != null) {
                    data.Sort(new Comparison<BreakTimeRecord>((item1, item2) => { return item1.Date.CompareTo(item2.Date); }));
                    foreach (var record in data) {
                        // Should only have zero/one record
                        breakTimes.Update(record);
                        updated = true;
                    }
                }
                if (!updated) breakTimes.Update(new BreakTimeRecord()); // no record found, set to default
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());
            }
        }

        public List<BreakTimeRecord> LoadAllBreakTimes() {
            try {
                return conn.Query<BreakTimeRecord>("SELECT * FROM breakTimes").ToList<BreakTimeRecord>() ?? new List<BreakTimeRecord>();
                // return conn.Table<BreakTimeRecord>().ToList() ?? new List<BreakTimeRecord>();
                // const string query = "SELECT * FROM breakTimes";
                // var cmd = new SQLiteCommand(conn);
                // cmd.CommandText = query;
                // var data = cmd.ExecuteQuery<BreakTimeRecord>();
                // return data ?? new List<BreakTimeRecord>();
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());
                return new List<BreakTimeRecord>();
            }
        }

        public void AddRecord(BreakTimeRecord record) {
            try {
                conn.BeginTransaction();
                conn.InsertOrReplace(record);
                conn.Commit();
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());
                try { conn.Rollback(); } catch (SQLiteException) { }
            }
        }
    }

    [Table("breakTimes")]
    public class BreakTimeRecord {

        [PrimaryKey, Indexed, Column("date")]
        public DateTime Date { get; set; } = DateTime.Now.Date;

        [Column("shiftStart")]
        public TimeSpan ShiftStart { get; set; } = TimeSpan.Zero;

        [Column("shiftEnd")]
        public TimeSpan ShiftEnd { get; set; } = TimeSpan.Zero;

        [Column("firstBreak")]
        public TimeSpan FirstBreak { get; set; } = TimeSpan.Zero;

        [Column("lastBreak")]
        public TimeSpan LastBreak { get; set; } = TimeSpan.Zero;

        [Column("lunchBreak")]
        public TimeSpan LunchBreak { get; set; } = TimeSpan.Zero;

        [Column("meetingBreak")]
        public TimeSpan MeetingBreak { get; set; } = TimeSpan.Zero;

        [Column("trainingBreak")]
        public TimeSpan TrainingBreak { get; set; } = TimeSpan.Zero;

        public BreakTimeRecord() {}

        public BreakTimeRecord(DateTime date, BreakTimes breakTimes) {
            this.Date = date;
            this.ShiftStart = breakTimes.ShiftStart;
            this.ShiftEnd = breakTimes.ShiftEnd;
            this.FirstBreak = breakTimes.FirstBreak;
            this.LastBreak = breakTimes.LastBreak;
            this.LunchBreak = breakTimes.LunchBreak;
            this.MeetingBreak = breakTimes.MeetingBreak;
            this.TrainingBreak = breakTimes.TrainingBreak;
        }
    }
}
