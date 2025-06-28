using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.shifts;
using com.tybern.CallRecordCore;
using SQLite.Net2;

namespace com.tybern.ShiftTracker {
    public class DBConverter {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private static bool isConverting = false;   // prevent circular loop during conversion process - if we're already converting table, don't mark the table to be converted again

        public static bool needsConversion(DBMarkerTable marker) {
            // mark for conversion if not on latest version
            return (!isConverting) && (marker.Version != DBMarkerTable.CurrentVersion);
        }

        public static DBMarkerTable convertDB(SQLiteConnection dbConnection, DBMarkerTable currMarker) {
            isConverting = true; 
            // Convert Shift / Break Times to new format
            DBShift shiftDB = new DBShift(dbConnection);
            DBBreaks breaksDB = new DBBreaks(dbConnection);
            BreakTimesDB oldDB = new BreakTimesDB(dbConnection);
            List<BreakTimeRecord> oldBreaks = oldDB.LoadAllBreakTimes();
            foreach (BreakTimeRecord record in oldBreaks) {
                // Convert from the old format to new format; as training breaks are flexible length, default to no length
                LOG.Info("Loading Shift: " + record.Date.ToString("dd-MM-yyyy") + " <" + record.ShiftStart + ">:<" + record.ShiftEnd + ">");
                WorkShift shift = new WorkShift(record.Date, record.ShiftStart, record.ShiftEnd, record.FirstBreak, record.LunchBreak, record.LastBreak, record.MeetingBreak,
                    (record.TrainingBreak != TimeSpan.Zero) ? new SortedSet<DBBreakRecord>(new []{ new DBBreakRecord(BreakType.Training, record.Date+record.TrainingBreak, record.Date + record.TrainingBreak) }) : new SortedSet<DBBreakRecord>());
                if (record.TrainingBreak != TimeSpan.Zero) {
                    // LOG that a training break needs to be checked
                    LOG.Info("Training Break added with no length: " + record.Date + " <" + record.TrainingBreak + ">");
                }
                shift.Save(shiftDB, breaksDB);
            }
            try {
                dbConnection.BeginTransaction();
                DBMarkerTable _result = new DBMarkerTable() { Version = DBMarkerTable.CurrentVersion };
                dbConnection.Insert(_result);
                dbConnection.Commit();
                // Load the newly created marker
                var data = dbConnection.Table<DBMarkerTable>();
                foreach (var marker in data) {
                    // should only have one entry in marker; assume highest MarkerID is latest entry if multiple present
                    if ((_result.MarkerID == null) || (_result.MarkerID < marker.MarkerID)) _result = marker;
                }
                LOG.Info("Converted database to v" + _result.Version + " <" + _result.MarkerID + ">");
                isConverting = false;
                return _result;
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());    // Log the error
                try { dbConnection.Rollback(); } catch (SQLiteException) { }    // rollback and ignore rollback exceptions
                isConverting = false;
                return currMarker;
            }
        }
    }
}
