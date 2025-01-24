using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class AddCallRecord : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private CallRecord Record {get; set;}

        public AddCallRecord(CallRecord record) { this.Record = record; }

        public void Process() {
            LOG.Info("AddCallRecord: " + Record);
            CallRecordCore.Instance.UIProperties.CallRecordsList.Add(Record);
            // TODO: Add to Call Log
        }
    }
}
