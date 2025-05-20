using System;
using com.tybern.CallRecordCore.commands;
using com.tybern.CallRecordCore.dialogs;
using SQLite;

namespace com.tybern.CallRecordCore {
    public class CallLog {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public SQLiteConnection conn;

        public CallLog(string dbPath) : this(new SQLiteConnection(dbPath, true)) { }
        public CallLog(SQLiteConnection conn) { 
            this.conn = conn;

            // Ensure the required tables exist in the db
            conn.CreateTable<CallRecord>();
        }

        public void LoadCurrentDay() => LoadDay(DateTime.Now);

        public void LoadDay(DateTime currTime) {
            DateTime dayStart = CallRecordCore.fromCurrent(currTime, TimeSpan.Zero);
            DateTime dayEnd = CallRecordCore.fromCurrent(currTime.AddDays(1), TimeSpan.Zero);
            try {
                const string query = "SELECT * FROM callRecord WHERE startTime BETWEEN @dayStart AND @dayEnd";
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = query;
                cmd.Bind("@dayStart", dayStart.Ticks);
                cmd.Bind("@dayEnd", dayEnd.Ticks);
                var data = cmd.ExecuteQuery<CallRecord>();
                if (data != null) {
                    data.Sort(new Comparison<CallRecord>((item1, item2) => { return item1.startTime.CompareTo(item2.startTime); }));
                    foreach (var record in data) {
                        CallRecordCore.Instance.Messages.Enqueue(new AddCallRecord(record, true));
                    }
                }
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());
            }
        }

        private bool isBetween(DateTime value, DateTime startTime, DateTime endTime) {
            int compare1 = DateTime.Compare(value, startTime);
            int compare2 = DateTime.Compare(value, endTime);
            return (compare1 >= 0) && (compare2 <= 0);
        }

        public void AddRecord(CallRecord record) {
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
        public CallNotesResult.CallType CallType { get; set; }

        [MaxLength(2048), Column("notes")]
        public string notes { get; set; }

        [Column("isANGenerated")]
        public bool isANGenerated { get; set; }

        [Column("isANEdited")]
        public bool isANEdited { get; set; }

        [Column("isANSaved")]
        public bool isANSaved { get; set; }

        [Column("hasManualNotes")]
        public bool hasManualNotes { get; set; }

        public string AsString { get { return ToString(); } }

        public CallRecord(DateTime startTime, DateTime endTime, TimeSpan duration, TimeSpan wrap, int MAE = 0, CallNotesResult.CallType type = CallNotesResult.CallType.Other, string notes = "", bool isANGenerated = false, bool isANEdited = false, bool isANSaved = false, bool hasManualNotes = false) {
            this.startTime = startTime;
            this.endTime = endTime;
            this.duration = duration;
            this.wrap = wrap;
            this.MAE = MAE;
            CallType = type;
            this.notes = notes;
            this.isANGenerated = isANGenerated;
            this.isANEdited = isANEdited;
            this.isANSaved = isANSaved;
            this.hasManualNotes = hasManualNotes;
        }

        public CallRecord() : this(DateTime.MinValue, DateTime.MinValue, TimeSpan.Zero, TimeSpan.Zero) { }

        public override string ToString() {
            string _result = string.Empty;
            _result += CallRecordCore.toShortTimeString(startTime.TimeOfDay) + " - " + CallRecordCore.toShortTimeString(endTime.TimeOfDay) + "\n";
            _result += "<" + CallRecordCore.toShortTimeString(duration) + " / " + CallRecordCore.toShortTimeString(wrap) + ">\n";
            _result += CallNotesResult.GetText(CallType) + "; Transfers: " + MAE + "\n";
            _result += getAutoNotesStatus() + "\n";
            _result += notes;
            return _result;
        }

        private string getAutoNotesStatus() {
            string _result = "AutoNotes {";
            bool hasEntry = false;
            if (isANGenerated)
            {
                _result += (hasEntry) ? ", " : "";
                hasEntry = true;
                _result += "Generated";
            }
            if (isANEdited)
            {
                _result += (hasEntry) ? ", " : "";
                hasEntry = true;
                _result += "Edited";
            }
            if (isANSaved)
            {
                _result += (hasEntry) ? ", " : "";
                hasEntry = true;
                _result += "Saved";
            }
            if (!hasEntry) _result += "Not Generated";
            _result += "}";
            if (hasManualNotes) _result += "; Manual Notes Added";
            return _result;
        }
    }
}
