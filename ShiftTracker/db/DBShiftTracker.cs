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
        public static readonly string FORMAT_TIME = "hh\\:mm\\:ss";
        public static readonly string FORMAT_DT   = FORMAT_DATE + " HH\\:mm\\:ss";

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
            dbConnection.CreateTable<DBTableNotes>();
            dbConnection.CreateTable<DBTableCalls>();

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

        // Notes
        public bool save(NoteRecord nr) {
            DBTableNotes dbNote = new DBTableNotes(nr);
            return addRecord<DBTableNotes>(dbNote);
        }

        public SortedSet<NoteRecord> loadNotes(DateTime dt) {
            var data = dbConnection.Query<DBTableNotes>("SELECT * FROM shiftNotes WHERE startTime LIKE ?", dt.ToString(FORMAT_DATE) + "%");
            SortedSet<NoteRecord> _result = new SortedSet<NoteRecord>();
            if (data != null) foreach (DBTableNotes note in data) _result.Add(note.asNoteRecord());
            return _result;
        }

        public SortedSet<NoteRecord> allNotes() {
            var data = dbConnection.Table<DBTableNotes>();
            SortedSet<NoteRecord> _result = new SortedSet<NoteRecord>();
            if (data != null) foreach (DBTableNotes note in data) _result.Add(note.asNoteRecord());
            return _result;
        }

        // CallRecord
        public bool save(CallRecord cr) {
            DBTableCalls dbCall = new DBTableCalls(cr);
            bool _result = true;
            try {
                dbConnection.BeginTransaction();

                dbConnection.InsertOrReplace(dbCall.DBNotes);   // save the notes...
                dbConnection.InsertOrReplace(dbCall);           // ... and the main call details

                dbConnection.Commit();
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());
                try { dbConnection.Rollback(); } catch (SQLiteException) { }
                _result = false;
            }
            return _result;
        }

        public SortedSet<CallRecord> loadCallRecords(DateTime dt) {
            var data = dbConnection.Query<DBTableCalls>("SELECT * FROM callRecords WHERE startTime LIKE ?", dt.ToString(FORMAT_DATE) + "%");
            SortedSet<CallRecord> _result = new SortedSet<CallRecord>();
            foreach (DBTableCalls call in data) {
                var note = dbConnection.Query<DBTableNotes>("SELECT * FROM shiftNotes WHERE startTime = ?", call.StartTimeText);
                if ((note.Count == 0) || (note.Count > 1))
                    LOG.Error("Missing / multiple notes records");
                else
                    call.DBNotes = note[0]; // attach the notes from this timestamp
                _result.Add(call.asCallRecord());
            }
            return _result;
        }

        public SortedSet<NoteRecord> loadNCNotes(DateTime dt) {
            var data = dbConnection.Query<DBTableCalls>("SELECT * FROM callRecords WHERE startTime LIKE ?", dt.ToString(FORMAT_DATE) + "%");
            SortedSet<NoteRecord> _result = loadNotes(dt);
            foreach (DBTableCalls call in data) {
                NoteRecord q = new NoteRecord(call.StartTime);
                NoteRecord? note = null;
                if (_result.TryGetValue(q, out note)) {
                    // we found the entry, remove that record
                    _result.Remove(note);
                }
            }
            return _result;     // any remaining items have no associated call record
        }

        public SortedSet<CallRecord> allCallRecords() {
            var data = dbConnection.Table<DBTableCalls>();
            SortedSet<NoteRecord> allNotes = this.allNotes();
            SortedSet<CallRecord> _result = new SortedSet<CallRecord>();
            foreach (DBTableCalls call in data) {
                NoteRecord q = new NoteRecord(call.StartTime);
                NoteRecord? note = null;
                if (allNotes.TryGetValue(q, out note)) {
                    call.DBNotes = new DBTableNotes(note);
                }
                _result.Add(call.asCallRecord());
            }
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
