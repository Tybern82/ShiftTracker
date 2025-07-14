using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using com.tybern.ShiftTracker.enums;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.data.old {
    public class CallLog {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public SQLiteConnection conn;

        public CallLog(string dbPath) : this(new SQLiteConnection(dbPath)) { }
        public CallLog(SQLiteConnection conn) { 
            this.conn = conn;

            // Ensure the required tables exist in the db
            conn.CreateTable<CallRecordV1>();
        }

        private DateTime fromCurrent(DateTime b, TimeSpan t) => new DateTime(b.Year, b.Month, b.Day, t.Hours, t.Minutes, t.Seconds);

        public SortedSet<CallRecordV1> LoadDay(DateTime currTime) {
            DateTime dayStart = fromCurrent(currTime, TimeSpan.Zero);
            DateTime dayEnd =   fromCurrent(currTime.AddDays(1), TimeSpan.Zero);
            SortedSet<CallRecordV1> _result = new SortedSet<CallRecordV1>();
            try {
                var data = conn.Table<CallRecordV1>().Where(record => (record.startTime >= dayStart) && (record.startTime <= dayEnd)).ToList<CallRecordV1>();
                if (data != null) {
                    data.Sort(new Comparison<CallRecordV1>((item1, item2) => { return item1.startTime.CompareTo(item2.startTime); }));
                    foreach (var record in data) {
                        _result.Add(record);
                    }
                }
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());
            }
            return _result;
        }

        public SortedSet<CallRecordV1> LoadAll() {
            SortedSet<CallRecordV1> _result = new SortedSet<CallRecordV1>();
            try {
                var data = conn.Table<CallRecordV1>().ToList<CallRecordV1>();
                foreach (var record in data) _result.Add(record);
            } catch (SQLiteException e) {
                LOG.Error(e.ToString());
            }
            return _result;
        }

        public com.tybern.ShiftTracker.enums.CallType convertType(CallType callType) {
            return callType switch {
                CallType.Mobile => enums.CallType.Mobile,
                CallType.NBN => enums.CallType.NBN,
                CallType.ADSL => enums.CallType.ADSL,
                CallType.eMail => enums.CallType.eMail,
                CallType.Billing => enums.CallType.Billing,
                CallType.PA => enums.CallType.PA,
                CallType.Prepaid => enums.CallType.Prepaid,
                CallType.PSTN => enums.CallType.PSTN,
                CallType.Opticomm => enums.CallType.Opticomm,
                CallType.FetchTV => enums.CallType.FetchTV,
                CallType.HomeWireless => enums.CallType.HomeWireless,
                CallType.Platinum => enums.CallType.Platinum,
                CallType.Misrouted => enums.CallType.Misrouted,
                CallType.Helpdesk => enums.CallType.Helpdesk,
                CallType.Note => enums.CallType.Note,
                _ => enums.CallType.Other,
            };
        }

        public NoteRecord convertCall(CallRecordV1 callRecord) {
            switch (callRecord.CallType) {
                case CallType.Note:
                    // generic note
                    return new NoteRecord(callRecord.startTime) { NoteContent = callRecord.notes };


                default:
                    // full call
                    return new CallRecord(callRecord.startTime) {
                        Type = convertType(callRecord.CallType),
                        EndTime = callRecord.endTime,
                        WrapTime = callRecord.wrap,
                        TransferCount = callRecord.MAE,
                        NoteContent = callRecord.notes,
                        AutoNotesStatus = (callRecord.isANGenerated ? AutoNotesStatus.Generated : AutoNotesStatus.None)
                                        | (callRecord.isANEdited ? AutoNotesStatus.Edited : AutoNotesStatus.None)
                                        | (callRecord.isANSaved ? AutoNotesStatus.Saved : AutoNotesStatus.None)
                                        | (callRecord.hasManualNotes ? AutoNotesStatus.Manual : AutoNotesStatus.None),
                        IsPreferredNameRequested = callRecord.notes.Contains("Preferred Name requested")
                    };
            }
        }

        public SortedSet<NoteRecord> convertRecords(SortedSet<CallRecordV1> source) {
            SortedSet<NoteRecord> _result = new SortedSet<NoteRecord>();
            foreach (CallRecordV1 record in source) _result.Add(convertCall(record));
            return _result;
        }

        public CallLogConversionResult convertAll() {
            SortedSet<NoteRecord> notes = new SortedSet<NoteRecord>();
            SortedSet<CallRecord> calls = new SortedSet<CallRecord>();

            // Convert all the records to the new forma
            SortedSet<NoteRecord> allRecords = convertRecords(LoadAll());

            // Separate out regular notes from calls
            foreach (NoteRecord r in allRecords) {
                if (r is CallRecord) calls.Add((CallRecord)r);
                else notes.Add(r);
            }

            // Merge callbacks from the list of calls
            calls = mergeCallbacks(calls);

            // Return the result
            return new CallLogConversionResult(notes, calls);
        }

        public SortedSet<CallRecord> mergeCallbacks(SortedSet<CallRecord> source) {
            SortedSet<CallRecord> _result = new SortedSet<CallRecord>();
            if (source.Count == 0) return _result;
            // Pull out first record (need to have somewhere to store follow-up callbacks
            CallRecord latest = source.First();
            source.Remove(latest);  // remove so it's not counted twice
            foreach (CallRecord r in source) {
                if (r.NoteContent.Contains("Callback")) {
                    latest.appendNote(r.NoteContent);
                    latest.CallbackCount++;
                    latest.WrapTime += r.WrapTime;
                    latest.EndTime = r.EndTime;
                    latest.TransferCount += r.TransferCount;
                    latest.IsPreferredNameRequested = (latest.IsPreferredNameRequested || r.IsPreferredNameRequested);  // if pref name asked on either, set for result
                    latest.AutoNotesStatus |= r.AutoNotesStatus;    // merge auto-notes status
                } else {
                    _result.Add(latest);    // store previous record
                    latest = r;             // set this as the new base call
                }
            }
            _result.Add(latest);    // add last merged entry
            return _result;
        }
    }

    public class CallLogConversionResult {
        public SortedSet<NoteRecord> Notes { get; }
        public SortedSet<CallRecord> Calls { get; }

        public CallLogConversionResult(SortedSet<NoteRecord> notes, SortedSet<CallRecord> calls) {
            Notes = notes;
            Calls = calls;
        }
    }

    public enum CallType {
        [Description("Mobile Voice / Data")]
        Mobile,

        [Description("NBN Voice / Data")]
        NBN,

        [Description("ADSL Internet")]
        ADSL,

        [Description("eMail / Bigpond")]
        eMail,

        [Description("Billing / Accounts / Sales")]
        Billing,

        [Description("Priority Assistance")]
        PA,

        [Description("Prepaid")]
        Prepaid,

        [Description("PSTN Voice")]
        PSTN,

        [Description("Opticomm / Velocity")]
        Opticomm,

        [Description("Fetch TV / Telstra TV")]
        FetchTV,

        [Description("4G Fixed / 5G Home Wireless")]
        HomeWireless,

        [Description("Platinum Support (discontinued)")]
        Platinum,

        [Description("Misrouted / Invalid Call")]
        Misrouted,

        [Description("SME Helpdesk")]
        Helpdesk,

        [Description("Additional Information")]
        Note,

        [Description("Other / Unknown")]
        Other
    }

    [Table("callRecord")]
    public class CallRecordV1 : IComparable<CallRecordV1> {

        [PrimaryKey, Indexed, Column("startTime")]
        public DateTime startTime { get; set; }

        [Column("endTime")]
        public DateTime endTime { get; set; }

        [Ignore]
        public TimeSpan duration { get; set; }

        [Column("duration")]
        public long durationTicks {
            get { return duration.Ticks; }
            set { duration = new TimeSpan(value); }
        }

        [Ignore]
        public TimeSpan wrap { get; set; }
        [Column("wrap")]
        public long wrapTicks {
            get { return wrap.Ticks; }
            set { wrap = new TimeSpan(value); }
        }

        [Column("MAE")]
        public int MAE { get; set; }

        [Column("Type")]
        public CallType CallType { get; set; }

        [MaxLength(2048), Column("notes")]
        public string notes { get; set; }

        [Column("isANGenerated")]
        public bool isANGenerated { get; set; }

        [Column("isANEdited")]
        public bool isANEdited { get; set; }

        [Column("isANSaved")]
        public bool isANSaved { get; set; }

        [Column("hasManualNotes")]
        public bool hasManualNotes { get; set; }

        public string AsString { get { return ToString(); } }

        public CallRecordV1(DateTime startTime, DateTime endTime, TimeSpan duration, TimeSpan wrap, int MAE = 0, CallType type = CallType.Other, string notes = "", bool isANGenerated = false, bool isANEdited = false, bool isANSaved = false, bool hasManualNotes = false) {
            this.startTime = startTime;
            this.endTime = endTime;
            this.duration = duration;
            this.wrap = wrap;
            this.MAE = MAE;
            CallType = type;
            this.notes = notes;
            this.isANGenerated = isANGenerated;
            this.isANEdited = isANEdited;
            this.isANSaved = isANSaved;
            this.hasManualNotes = hasManualNotes;
        }

        public CallRecordV1() : this(DateTime.MinValue, DateTime.MinValue, TimeSpan.Zero, TimeSpan.Zero) { }

        public int CompareTo(CallRecordV1 other) => startTime.CompareTo(other.startTime);
    }
}
