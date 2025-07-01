using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.ShiftTracker.data;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.db {

    [Table("dbVersion")]
    public class DBMarkerTable {

        public static readonly int DefaultVersion = 1;
        public static readonly int CurrentVersion = 2;

        [PrimaryKey] public int? MarkerID { get; set; } = null;
        [Column("version")] public int Version { get; set; } = 0;
    }

    public class DBShiftTracker {

        public static readonly string FORMAT_DATE = "yyyy-MM-dd";
        public static readonly string FORMAT_TIME = "c";

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private static DBShiftTracker? _Instance = null;
        public static DBShiftTracker Instance {
            get { _Instance ??= new DBShiftTracker(); return _Instance; }
        }

        private SQLiteConnection dbConnection;
        public static DBMarkerTable DBMarker { get; private set; } = new DBMarkerTable();

        public DBShiftTracker() {
            dbConnection = TrackerSettings.Instance.DBConnection;

            dbConnection.CreateTable<DBMarkerTable>();
            dbConnection.CreateTable<DBTableWorkShift>();
            dbConnection.CreateTable<DBTableWorkBreak>();

            if (DBMarker.MarkerID == null) DBMarker = loadMarker(); // load the marker ID from the database
            if (DBMarker.MarkerID == null) {            // if still null, marker was not found in the table:
                addRecord<DBMarkerTable>(DBMarker);     // record the marker in the table
                loadMarker();                           // reload to get the MarkerID
            }
        }

        // WorkBreak
        public bool save(WorkBreak brk) {
            DBTableWorkBreak workBreak = new DBTableWorkBreak(brk);
            return addRecord<DBTableWorkBreak>(workBreak);
        }

        public SortedSet<WorkBreak> loadWorkBreaks(DateTime dt) {
            var data = dbConnection.Query<DBTableWorkBreak>("SELECT * FROM breakTimes WHERE date = ?", dt.ToString(FORMAT_DATE));
            SortedSet<WorkBreak> _result = new SortedSet<WorkBreak>();
            if (data != null) {
                foreach (DBTableWorkBreak record in data) {
                    _result.Add(record.asWorkBreak());
                }
            }
            return _result;
        }

        public SortedSet<WorkBreak> allWorkBreaks() {
            var data = dbConnection.Table<DBTableWorkBreak>();
            SortedSet<WorkBreak> _result = new SortedSet<WorkBreak>();
            if (data != null) foreach (DBTableWorkBreak record in data) _result.Add(record.asWorkBreak());
            return _result;
        }


        // WorkShift
        public bool save(WorkShift shift) {
            DBTableWorkShift workShift = new DBTableWorkShift(shift);
            List<DBTableWorkBreak> breaks = new List<DBTableWorkBreak>();
            foreach (WorkBreak brk in shift.Breaks) breaks.Add(new DBTableWorkBreak(brk));
            bool _result = true;
            try {
                dbConnection.BeginTransaction();

                // Remove any entries which are no longer associated with this shift
                SortedSet<WorkBreak> currBreaks = loadWorkBreaks(shift.CurrentDate);
                foreach (WorkBreak brk in shift.Breaks) {
                    currBreaks.Remove(brk);
                }
                // any remaining entries no longer exist and need to be removed
                List<DBTableWorkBreak> toRemove = new List<DBTableWorkBreak>();
                foreach (WorkBreak brk in currBreaks) toRemove.Add(new DBTableWorkBreak(brk));
                foreach (DBTableWorkBreak brk in toRemove) {
                    dbConnection.Execute("DELETE FROM breakTimes WHERE date = ? AND startTime = ?", brk.DateText, brk.StartTimeText);
                }

                if ((workShift.StartTime != TimeSpan.Zero) || (workShift.EndTime != TimeSpan.Zero) || (breaks.Count > 0)) {
                    // WorkShift has valid data; update the record in the DB
                    dbConnection.InsertOrReplace(workShift);
                    foreach (DBTableWorkBreak brk in breaks) dbConnection.InsertOrReplace(brk);
                } else {
                    // WorkShift is <blank>; remove any current entry from the DB
                    // Any Breaks associated with this date will have already been removed earlier
                    dbConnection.Execute("DELETE FROM shifts WHERE date = ?", workShift.DateText);
                }

                    dbConnection.Commit();
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());    // Log the error
                try { dbConnection.Rollback(); } catch (SQLiteException) { }
                _result = false;
            }
            return _result;
        }

        public WorkShift? loadWorkShift(DateTime dt) {
            var data = dbConnection.Query<DBTableWorkShift>("SELECT * FROM shifts WHERE date = ?", dt.ToString(FORMAT_DATE));
            if (data == null) return null;
            // Should only return a single result, as Date is PrimaryKey; 
            if (data.Count > 1) LOG.Warn("PRIMARY KEY VIOLATION: Found multiple entries in WorkShifts with the same date");
            WorkShift? _result = null;
            foreach (DBTableWorkShift shift in data)
                _result = shift.asWorkShift(); // asWorkShift will load the associated breaks
            return _result;
        }

        public SortedSet<WorkShift> allWorkShifts() {
            var data = dbConnection.Table<DBTableWorkShift>();
            SortedSet<WorkShift> _result = new SortedSet<WorkShift>();
            if (data != null) foreach (DBTableWorkShift shift in data) _result.Add(shift.asWorkShift());
            return _result;
        }


        // General Utilities
        private bool isDate(DateTime d1, DateTime d2) {
            if (d1 == null) return (d2 == null);
            return (d1.Year == d2.Year) && (d1.Month == d2.Month) && (d1.Day == d2.Day);
        }

        // Helper method for adding a single database record
        private bool addRecord<T>(T record) {
            try {
                dbConnection.BeginTransaction();
                dbConnection.InsertOrReplace(record);
                dbConnection.Commit();
                return true;
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());    // Log the error
                try { dbConnection.Rollback(); } catch (SQLiteException) { }    // rollback and ignore rollback exceptions
                return false;
            }
        }

        private DBMarkerTable loadMarker() {
            var data = this.dbConnection.Table<DBMarkerTable>();
            DBMarkerTable _result = new DBMarkerTable() { Version = DBMarkerTable.CurrentVersion };
            foreach (var marker in data) {
                // should only have one entry in marker; assume highest MarkerID is latest entry if multiple present
                if ((_result.MarkerID == null) || (_result.MarkerID < marker.MarkerID)) _result = marker;
            }
            return _result;
        }
    }
}
