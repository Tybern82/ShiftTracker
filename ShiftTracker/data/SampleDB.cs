using System;
using System.Collections.Generic;
using System.Text;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.data {
    public class SampleDB {

        public SampleDB() {
            TrackerSettings.Instance.DBConnection.CreateTable<SampleTableRecord>();
        }

        public void populate() {
            Random r = new Random();
            SQLiteConnection conn = TrackerSettings.Instance.DBConnection;
            conn.BeginTransaction();
            for (int x = 0; x < 1500; x++) {
                SampleTableRecord rec = new SampleTableRecord();
                rec.TimeStamp = new DateTime(r.Next(1990, 2040), r.Next(1, 12), r.Next(1, 29));
                conn.InsertOrReplace(rec);                
            }
            conn.Commit();
        }

        public bool check() {
            var data = TrackerSettings.Instance.DBConnection.Table<SampleTableRecord>();
            bool _result = false;
            if (data != null) {
                _result = true;
                foreach (var record in data) {
                    if (record != null) _result &= record.check();
                }
            }
            return _result;
        }

        public int recordCount() {
            var data = TrackerSettings.Instance.DBConnection.Table<SampleTableRecord>();
            return (data == null) ? 0 : data.Count();
        }

        public bool hasRecords() {
            return (recordCount() != 0);
        }
    }

    [Table("sampleTable")]
    public class SampleTableRecord {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private DateTime _TimeStamp = DateTime.Now.Date;
        [PrimaryKey, Column("timeStamp")] public DateTime TimeStamp { 
            get { return _TimeStamp; }
            set { _TimeStamp = value.Date; }
        }

        private DateTime? _ReadableTime;
        [Column("readableTime")] public string ReadableTime { 
            get { return TimeStamp.ToString("yyyy-MM-dd"); } 
            set {
                _ReadableTime = DateTime.Parse(value).Date;
            }
        }

        public bool check() {
            if (_ReadableTime != null) {
                // LOG.Info("Compare <" + TimeStamp.Ticks + ">:<" + _ReadableTime?.Ticks + "> - " + (_ReadableTime?.Ticks == TimeStamp.Ticks));
                return (_ReadableTime?.Ticks == TimeStamp.Ticks);
            }
            return false;
        }
    }
}
