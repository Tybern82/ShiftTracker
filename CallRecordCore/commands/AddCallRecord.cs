using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class AddCallRecord : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private CallRecord Record {get; set;}
        private bool DBLoaded { get; set; } = false;

        public AddCallRecord(CallRecord record, bool DBLoaded = false) { this.Record = record; this.DBLoaded = DBLoaded; }

        public void Process() {
            LOG.Info("AddCallRecord: " + Record);
            CallRecordCore.Instance.UIProperties.CallRecordsList.Add(Record);
            if (!DBLoaded) {
                CallRecordCore.Instance.CallRecordLog.AddRecord(Record); // don't readd to DB if already loaded from DB
            } else {
                // Update the stats with the details from the log
                CallRecordCore.Instance.UIProperties.TotalCalls++;
                CallRecordCore.Instance.UIProperties.TotalMAE += Record.MAE;
                CallRecordCore.Instance.UIProperties.TotalWrap += Record.wrap;
                CallRecordCore.Instance.ShiftCounter.TotalDuration += Record.duration;
                CallRecordCore.Instance.ShiftCounter.CallTypeCounter.UpdateCallType(Record.CallType);
            }
        }
    }
}
