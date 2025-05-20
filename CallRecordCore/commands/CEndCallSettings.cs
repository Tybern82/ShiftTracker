using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CEndCallSettings : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private bool resetType = true;

        public CEndCallSettings(bool reset = true)
        {
            this.resetType = reset;
        }
        public void Process() {
            LOG.Info("Call Ended - Clearing counters");

            CallRecordCore.Instance.CurrentCall.IsCallback = false;
            CallRecordCore.Instance.CurrentCall.CurrentMode = CallDetails.CallMode.Disconnect;

            if (resetType) {
                CallRecordCore.Instance.UIProperties.CurrentCallType = dialogs.CallNotesResult.CallType.NBN; // default type
            }
        }
    }
}
