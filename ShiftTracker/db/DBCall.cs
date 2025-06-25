using System;
using System.Collections.Generic;
using System.Text;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.db {
    public class DBCall : DBBase<DBCallRecord> {

        public const string DBTableName = "callRecordv2";

        public DBCall(SQLiteConnection conn) : base(DBTableName, conn) { }
        public DBCall(string dbFile) : base(DBTableName, dbFile) { }

    }

    public enum CallType { Mobile, NBN, ADSL, eMail, Billing, PA, Prepaid, PSTN, Opticomm, FetchTV, HomeWireless, Platinum, Misrouted, Helpdesk, Other, Note }
    [Flags] public enum AutoNotesStatus { 
        None = 0,
        Generated = 1, 
        Edited = 2, 
        Saved = 4, 
        Manual = 8 
    }
    public enum SurveyType { Prompted, Callback, Transfer, Agent, NonTelstra, NonFaults, Other }

    [Table(DBCall.DBTableName)]
    public class DBCallRecord : IComparable<DBCallRecord> {

        [PrimaryKey, Indexed, Column("startTime")]
        public DateTime StartTime { get; set; } = DateTime.Now;

        [Column("endTime")]
        public DateTime EndTime { get; set; } = DateTime.Now;

        [Ignore]
        public TimeSpan CallTime { get; set; } = TimeSpan.Zero;

        [Column("callTime")]
        public long DurationTicks {
            get { return CallTime.Ticks; }
            set { CallTime = new TimeSpan(value); }
        }

        [Ignore]
        public TimeSpan WrapTime { get; set; } = TimeSpan.Zero;
        [Column("wrapTime")]
        public long WrapTicks {
            get { return WrapTime.Ticks; }
            set { WrapTime = new TimeSpan(value); }
        }

        [Column("MAE")]
        public int MAE { get; set; } = 0;

        [Column("callType")]
        public CallType Type { get; set; } = CallType.Other;

        [Column("autoNotes")]
        public AutoNotesStatus AutoNotes { get; set; } = AutoNotesStatus.None;

        [Column("surveyStatus")]
        public SurveyType SurveyStatus { get; set; } = SurveyType.Other;

        [MaxLength(2048), Column("notes")]
        public string Notes { get; set; } = string.Empty;

        public bool isANGenerated() => AutoNotes.HasFlag(AutoNotesStatus.Generated);
        public bool isANEdited() => AutoNotes.HasFlag(AutoNotesStatus.Edited);
        public bool isANSaved() => AutoNotes.HasFlag(AutoNotesStatus.Saved);
        public bool hasManualNotes() => AutoNotes.HasFlag(AutoNotesStatus.Manual);
        public bool isSurveyPrompted() => (SurveyStatus == SurveyType.Prompted);

        public int CompareTo(DBCallRecord other) => StartTime.CompareTo(other.StartTime);
    }
}
