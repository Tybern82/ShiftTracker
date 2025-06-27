using System;
using System.Collections.Generic;
using System.Linq;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.db {

    [Table("dbVersion")]
    public class DBMarkerTable {

        public static readonly int DefaultVersion = 1;
        public static readonly int CurrentVersion = 2;

        [PrimaryKey] public int? MarkerID { get; set; } = null;
        [Column("version")] public int Version { get; set; } = 0;
    }

    public abstract class DBBase<T> where T : class, IComparable<T>  {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        protected SQLiteConnection dbConnection;
        private string dbTable;

        public static DBMarkerTable DBMarker { get; private set; } = new DBMarkerTable();

        protected DBBase(string dbTable, SQLiteConnection conn) {
            this.dbTable = dbTable;
            this.dbConnection = conn;
            dbConnection.CreateTable<DBMarkerTable>();
            dbConnection.CreateTable<T>();

            if (DBMarker.MarkerID == null) DBMarker = loadMarker();
            if (DBConverter.needsConversion(DBMarker)) 
                DBMarker = DBConverter.convertDB(dbConnection, DBMarker);
        }

        protected DBBase(string dbTable, string dbFile) : this(dbTable, new SQLiteConnection(dbFile)) { }

        private DBMarkerTable loadMarker() {
            var data = this.dbConnection.Table<DBMarkerTable>();
            DBMarkerTable _result = new DBMarkerTable();
            foreach (var marker in data) {
                // should only have one entry in marker; assume highest MarkerID is latest entry if multiple present
                if ((_result.MarkerID == null) || (_result.MarkerID < marker.MarkerID)) _result = marker;
            }
            return _result;
        }

        public bool AddRecord(T record) {
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

        public List<T> LoadToday() => LoadRecords(DateTime.Now.Date);

        public List<T> LoadRecords(DateTime dt) {
            List<T> _result = new List<T>();

            try {
                DateTime dayStart = fromCurrent(dt, TimeSpan.Zero);
                DateTime dayEnd = fromCurrent(dt.AddDays(1), TimeSpan.Zero);
                var data = this.dbConnection.Query<T>("SELECT * FROM " + dbTable + " WHERE date BETWEEN ? AND ?", dayStart.Ticks, dayEnd.Ticks).ToList<T>();
                // string query = "SELECT * FROM " + dbTable + " WHERE date IS @date";
                // var cmd = new SQLiteCommand(dbConnection);
                // cmd.CommandText = query;
                // cmd.Bind("@date", dt.Date.Ticks);
                // var data = cmd.ExecuteQuery<T>();
                if (data != null) {
                    foreach (var record in data) _result.Add(record);
                }
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());    // log any error identified
            }
            _result.Sort(LoadComparison());
            return _result;
        }
        private static DateTime fromCurrent(DateTime currDay, TimeSpan timeOffset) => new DateTime(currDay.Year, currDay.Month, currDay.Day, timeOffset.Hours, timeOffset.Minutes, timeOffset.Seconds);

        public List<T> LoadAll() {
            List<T> _result = new List<T>();
            try {
                var data = this.dbConnection.Query<T>("SELECT * FROM " + dbTable).ToList<T>();
                if (data != null)
                    foreach (var record in data) _result.Add(record);
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());
            }
            _result.Sort(LoadComparison());
            return _result;
        }

        public static Comparison<T> LoadComparison() => new Comparison<T>((item1, item2) => { 
            if (item1 is null) {
                if (item2 is null) return 0;
                throw new ArgumentException("Comparison of <null> to non-null value");
            } else if (item2 is null) {
                throw new ArgumentNullException("Comparison of <null> to non-null value");
            } else {
                return item1.CompareTo(item2);
            }
        });

        public static void ValidateCompare(object obj) {
            if (obj is null) throw new ArgumentNullException("Comparison of <null> to non-null value");
            if (!(obj is T)) throw new ArgumentException("Comparison to incompatible type");
        }
    }
}
