using System;
using SQLite;

namespace CallRecordCore {
    public class CallLog {

        public SQLiteConnection conn;

        public CallLog(string dbPath) {
            conn = new SQLiteConnection(dbPath, true);

            // Ensure the required tables exist in the db
            conn.CreateTable<CallRecord>();
        }
    }

    [Table("callRecord")]
    public class CallRecord {

		[PrimaryKey, Indexed, Column("startTime")]
		public DateTime startTime { get; set; }

		[Column("endTime")]
		public DateTime endTime { get; set; }

		[Ignore]
		public TimeSpan duration { get; set; }

		[Column("duration")]
		public long durationTicks { 
			get { return duration.Ticks; } 
			set { duration = new TimeSpan(value); } 
		}

		[Ignore]
		public TimeSpan wrap { get; set; }
		[Column("wrap")]
		public long wrapTicks { 
			get { return wrap.Ticks; } 
			set { wrap = new TimeSpan(value); } 
		}

		[Column("MAE")]
		public int MAE { get; set; }

		[Column("Type")]
		public CallNotes.CallType CallType { get; set; }

		[MaxLength(2048), Column("notes")] 
		public string notes { get; set; }

		public CallRecord(DateTime startTime, DateTime endTime, TimeSpan duration, TimeSpan wrap, int MAE = 0, CallNotes.CallType type = CallNotes.CallType.Other, string notes = "") {
			this.startTime = startTime;
			this.endTime = endTime;
			this.duration = duration;
			this.wrap = wrap;
			this.MAE = MAE;
			this.CallType = type;
			this.notes = notes;
		}
    }
}
