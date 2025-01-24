using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CStartWrap : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();
        public void Process() {
            LOG.Info("Start Wrap");
            DateTime timeStamp = DateTime.Now;
            if (!CallRecordCore.Instance.CurrentCall.IsInCall) 
                (new CStartCall()).Process();    // Start call immediately if not already in-call; NOTE: Should never be called 

            CallRecordCore.Instance.CurrentCall.WrapStartTime = timeStamp.TimeOfDay;
            CallRecordCore.Instance.CurrentCall.IsInWrap = true;
            CallRecordCore.Instance.CurrentCall.CurrentMode = CallDetails.CallMode.InWrap;      // Update UI state to In-Wrap
        }
    }
}
