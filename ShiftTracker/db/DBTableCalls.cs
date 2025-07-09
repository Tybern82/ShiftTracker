using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.enums;
using Org.BouncyCastle.Asn1.Mozilla;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.db {

    [Table("callRecords")]
    public class DBTableCalls {

        [Ignore] public DateTime StartTime { get; set; }
        [PrimaryKey, Indexed, Column("startTime")]
        public string StartTimeText {
            get { return StartTime.ToString(DBShiftTracker.FORMAT_DT); }
            set { StartTime = DateTime.Parse(value); }
        }

        [Ignore] public DateTime EndTime { get; set; }
        [Column("endTime")] public string EndTimeText {
            get { return EndTime.ToString(DBShiftTracker.FORMAT_DT); }
            set { EndTime = DateTime.Parse(value); }
        }

        [Ignore] public TimeSpan SMETime { get; set; }
        [Column("smeTime")] public string SMETimeText {
            get { return SMETime.ToString(DBShiftTracker.FORMAT_TIME); }
            set { SMETime = TimeSpan.Parse(value); }
        }

        [Ignore] public TimeSpan WrapTime { get; set; }
        [Column("wrapTime")] public string WrapTimeText {
            get { return WrapTime.ToString(DBShiftTracker.FORMAT_TIME); }
            set { WrapTime =  TimeSpan.Parse(value); }
        }

        [Ignore] public TimeSpan TransferTime { get; set; }
        [Column("transferTime")] public string TransferTimeText {
            get { return TransferTime.ToString(DBShiftTracker.FORMAT_TIME); }
            set { TransferTime = TimeSpan.Parse(value); }
        }

        [Column("transferCount")] public int TransferCount { get; set; }

        [Column("isPrefName")] public bool isPreferredName { get; set; }

        [Ignore] public SurveyStatus Survey { get; set; }
        [Column("survey")] public string SurveyText {
            get { return Survey.ToString(); }
            set { Survey = (SurveyStatus)Enum.Parse(typeof(SurveyStatus), value, true); }
        }

        [Ignore] public CallType Type { get; set; }
        [Column("type")] public string TypeText {
            get { return Type.ToString(); }
            set { Type = (CallType)Enum.Parse(typeof(CallType), value, true); }
        }

        [Ignore] public AutoNotesStatus AutoNotes { get; set; }
        [Column("autoNotes")] public string AutoNotesText {
            get { return AutoNotes.ToString(); }
            set { AutoNotes = (AutoNotesStatus)Enum.Parse(typeof(AutoNotesStatus), value, true); }
        }

        [Ignore] public DBTableNotes DBNotes { get; set; }

        public DBTableCalls() {
            DBNotes = new DBTableNotes(StartTime);
        }

        public DBTableCalls(CallRecord cr) {
            this.StartTime = cr.StartTime;
            this.EndTime = cr.EndTime;
            this.SMETime = cr.SMETime;
            this.WrapTime = cr.WrapTime;
            this.TransferTime = cr.TransferTime;
            this.TransferCount = cr.TransferCount;
            this.isPreferredName = cr.IsPreferredNameRequested;
            this.Survey = cr.Survey;
            this.Type = cr.Type;
            this.AutoNotes = cr.AutoNotesStatus;
            this.DBNotes = new DBTableNotes(cr);
        }

        public CallRecord asCallRecord() {
            return new CallRecord(StartTime) {
                EndTime = this.EndTime,
                SMETime = this.SMETime,
                WrapTime = this.WrapTime,
                TransferTime = this.TransferTime,
                TransferCount = this.TransferCount,
                IsPreferredNameRequested = this.isPreferredName,
                Survey = this.Survey,
                Type = this.Type,
                AutoNotesStatus = this.AutoNotes,
                NoteContent = this.DBNotes.Notes
            };
        }
    }
}
