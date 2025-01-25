using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace com.tybern.CallRecordCore {
    public class SurveyLog {

        public SQLiteConnection conn;

        public SurveyLog(string dbPath) {
            conn = new SQLiteConnection(dbPath, true);

            // Ensure the required tables exist in the db
            conn.CreateTable<SurveyRecord>();
        }
    }

    public class SurveyRecord {

        public int CallNumber { get; set; }

        public bool IsPrompted { get; set; }

        [MaxLength(1024)]
        public string Text { get; set; } = string.Empty;

        public string AsString { get { return ToString(); } }

        public SurveyRecord(int callNumber, bool isPrompted, string text) {
            CallNumber = callNumber;
            IsPrompted = isPrompted;
            Text = text;
        }

        public override string ToString() {
            return CallNumber + ": " + IsPrompted + (string.IsNullOrWhiteSpace(Text) ? string.Empty : " - " + Text);
        }
    }
}
