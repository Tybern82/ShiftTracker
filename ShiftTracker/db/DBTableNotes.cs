using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.ShiftTracker.data;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.db {
    [Table("shiftNotes")]
    public class DBTableNotes {

        [Ignore] public DateTime StartTime { get; set; }
        [PrimaryKey, Indexed, Column("startTime")] public string StartTimeText {
            get { return StartTime.ToString(DBShiftTracker.FORMAT_DT); }
            set { StartTime = DateTime.Parse(value); }
        }

        [Column("notes")] public string Notes { get; set; }

        public DBTableNotes() : this(DateTime.Now) { }

        public DBTableNotes(DateTime startTime) {
            StartTime = startTime;
            Notes = string.Empty;
        }

        public DBTableNotes(NoteRecord nr) {
            this.StartTime = nr.StartTime;
            this.Notes = nr.NoteContent;
        }

        public NoteRecord asNoteRecord() {
            return new NoteRecord(StartTime) { NoteContent = Notes };
        }
    }
}
