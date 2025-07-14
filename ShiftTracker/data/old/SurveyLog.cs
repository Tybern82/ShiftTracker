using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.tybern.ShiftTracker.db;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.data.old {
    public class SurveyLog {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public SQLiteConnection conn;

        public SurveyLog(string dbPath) : this(new SQLiteConnection(dbPath)) {}
        public SurveyLog(SQLiteConnection conn) {
            this.conn = conn;

            // Ensure the required tables exist in the db
            conn.CreateTable<SurveyRecordV1>();
        }
        private DateTime fromCurrent(DateTime b, TimeSpan t) => new DateTime(b.Year, b.Month, b.Day, t.Hours, t.Minutes, t.Seconds);

        public SortedSet<SurveyRecordV1> LoadDay(DateTime currTime) {
            DateTime dayStart = fromCurrent(currTime, TimeSpan.Zero);
            DateTime dayEnd = fromCurrent(currTime.AddDays(1), TimeSpan.Zero);
            SortedSet<SurveyRecordV1> _result = new SortedSet<SurveyRecordV1>();
            try {
                var data = conn.Table<SurveyRecordV1>().Where(record => (record.CallTime >= dayStart) && (record.CallTime <= dayEnd)).ToList<SurveyRecordV1>();
                if (data != null) {
                    foreach (var record in data) {
                        _result.Add(record);
                    }
                }
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());
            }
            return _result;
        }

        public SortedSet<SurveyRecordV1> LoadAll() {
            SortedSet<SurveyRecordV1> _result = new SortedSet<SurveyRecordV1>();
            try {
                var data = conn.Table<SurveyRecordV1>().ToList<SurveyRecordV1>();
                if (data != null)
                    foreach (var record in data)
                        _result.Add(record);
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());
            }
            return _result;
        }

        public enums.SurveyStatus convertStatus(OptionSkipSurvey type) {
            return type switch {
                OptionSkipSurvey.None => enums.SurveyStatus.SurveyRequested,
                OptionSkipSurvey.Callback => enums.SurveyStatus.Callback,
                OptionSkipSurvey.Transfer => enums.SurveyStatus.Transfer,
                OptionSkipSurvey.Agent => enums.SurveyStatus.Agent,
                OptionSkipSurvey.NonTelstra => enums.SurveyStatus.NonTelstra,
                OptionSkipSurvey.NonFaults => enums.SurveyStatus.NonFaults,
                OptionSkipSurvey.Other => enums.SurveyStatus.Unspecified,
                _ => enums.SurveyStatus.Missing,
            };
        }

        public bool updateSurveyStatus(SortedSet<CallRecord> calls, SortedSet<SurveyRecordV1> surveys) {
            bool _result = true;
            try {
                foreach (CallRecord call in calls) {
                    SurveyRecordV1 s = surveys.First<SurveyRecordV1>();
                    while (s.CallTime.Date < call.StartTime.Date) {
                        _result = false;    // found extra surveys
                        LOG.Warn("Extra Survey: " + s.CallTime.Date + " <" + s.CallNumber + ">");
                        surveys.Remove(s);
                        s = surveys.First<SurveyRecordV1>();
                    }
                    if (s.CallTime.Date == call.StartTime.Date) {
                        call.Survey = convertStatus(s.Type);
                        surveys.Remove(s);
                    } else {
                        _result = false;    // missing survey
                        LOG.Warn("Mising survey: " + call.StartTime.ToString(DBShiftTracker.FORMAT_DATE));
                    }
                }
            } catch (ArgumentNullException) {
                LOG.Warn("Missing surveys");
            }

            return _result;
        }
    }

    public enum OptionSkipSurvey {
        Callback, Transfer, Agent, NonTelstra, None, Other, NonFaults
    }

    [Table("surveyRecord")]
    public class SurveyRecordV1 : IComparable<SurveyRecordV1> {

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

        public SurveyRecordV1(DateTime callTime, int callNumber, bool isPrompted, OptionSkipSurvey type, string text) {
            CallTime = callTime;
            CallNumber = callNumber;
            IsPrompted = isPrompted;
            Type = type;
            Text = text;
        }

        public SurveyRecordV1() : this(DateTime.Now, 0, false, OptionSkipSurvey.None, string.Empty) {}

        public int CompareTo(SurveyRecordV1 other) => CallTime.CompareTo(other.CallTime);
    }
}
