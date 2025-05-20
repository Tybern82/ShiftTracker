using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands
{
    public class CManualNotes : Command
    {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public void Process()
        {
            LOG.Info("Notes entered Manually");

            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.ANManualButton);
            CallRecordCore.Instance.CurrentCall.HasManualNotes = true;
        }
    }
}
