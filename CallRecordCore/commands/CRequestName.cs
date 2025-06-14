using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands
{
    public class CRequestName : Command
    {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();
        public void Process()
        {
            LOG.Info("Requested Preferred Name");
            CallRecordCore.Instance.Messages.Enqueue(new AppendNote("Preferred Name requested"));
            CallRecordCore.Instance.CurrentCall.IsPrefNameRequested = true;
            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.PrefNameButton);
        }
    }
}
