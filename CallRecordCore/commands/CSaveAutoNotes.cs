using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands
{
    public class CSaveAutoNotes : Command
    {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public void Process()
        {
            LOG.Info("AutoNotes Saved");

            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.ANSavedButton);
            CallRecordCore.Instance.CurrentCall.IsANSaved = true;
        }
    }
}
