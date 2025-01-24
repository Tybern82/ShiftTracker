using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CEndCallSettings : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();
        public void Process() {
            LOG.Info("Call Ended - Clearing counters");

            CallRecordCore.Instance.CurrentCall.IsCallback = false;
            CallRecordCore.Instance.CurrentCall.CurrentMode = CallDetails.CallMode.Disconnect;
        }
    }
}
