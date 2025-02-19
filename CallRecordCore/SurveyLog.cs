using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CallRecordCore.commands;
using com.tybern.CallRecordCore.dialogs;
using SQLite;
using static com.tybern.CallRecordCore.dialogs.SkipSurveyResult;

namespace com.tybern.CallRecordCore {
    public class SurveyLog {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public SQLiteConnection conn;

        public SurveyLog(string dbPath) : this(new SQLiteConnection(dbPath, true)) {}
        public SurveyLog(SQLiteConnection conn) {
            this.conn = conn;

            // Ensure the required tables exist in the db
            conn.CreateTable<SurveyRecord>();
        }

        public void LoadCurrentDay() => LoadDay(DateTime.Now);

        public void LoadDay(DateTime currTime) {
            DateTime dayStart = CallRecordCore.fromCurrent(currTime, TimeSpan.Zero);
            DateTime dayEnd = CallRecordCore.fromCurrent(currTime.AddDays(1), TimeSpan.Zero);
            try {
                const string query = "SELECT * FROM surveyRecord WHERE startTime BETWEEN @dayStart AND @dayEnd";
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = query;
                cmd.Bind("@dayStart", dayStart.Ticks);
                cmd.Bind("@dayEnd", dayEnd.Ticks);
                var data = cmd.ExecuteQuery<SurveyRecord>();
                if (data != null) {
                    data.Sort(new Comparison<SurveyRecord>((item1, item2) => { return item1.CallTime.CompareTo(item2.CallTime); }));
                    int maxCallNumber = 0;
                    foreach (var record in data) {
                        CallRecordCore.Instance.Messages.Enqueue(new AddSurveyRecord(record, true));
                        maxCallNumber = max(maxCallNumber, record.CallNumber);
                    }
                    CallRecordCore.Instance.ShiftCounter.CallNumber = maxCallNumber;
                }
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());
            }
        }

        private int max(int i, int j) {
            return (i < j) ? j : i;
        }

        private bool isBetween(DateTime value, DateTime startTime, DateTime endTime) {
            int compare1 = DateTime.Compare(value, startTime);
            int compare2 = DateTime.Compare(value, endTime);
            return (compare1 >= 0) && (compare2 <= 0);
        }

        public void AddRecord(SurveyRecord record) {
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

    [Table("surveyRecord")]
    public class SurveyRecord {

        [PrimaryKey, Indexed, Column("startTime")]
        public DateTime CallTime { get; set; } = DateTime.Now;

        [Column("callNumber")]
        public int CallNumber { get; set; }

        [Column("isPrompted")]
        public bool IsPrompted { get; set; }

        [Column("surveyType")]
        public OptionSkipSurvey Type { get; set; } = OptionSkipSurvey.None;

        [MaxLength(1024), Column("details")]
        public string Text { get; set; } = string.Empty;

        [Ignore]
        public string AsString { get { return ToString(); } }

        public SurveyRecord(DateTime callTime, int callNumber, bool isPrompted, OptionSkipSurvey type, string text) {
            CallTime = callTime;
            CallNumber = callNumber;
            IsPrompted = isPrompted;
            Type = type;
            Text = text;
        }

        public SurveyRecord() : this(DateTime.Now, 0, false, OptionSkipSurvey.None, string.Empty) {}

        public override string ToString() {
            return CallNumber + ": " + IsPrompted + " - " + SkipSurveyResult.GetText(Type) + (string.IsNullOrWhiteSpace(Text) ? string.Empty : ": " + Text);
        }
    }
}
