using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands
{
    public class CEditAutoNotes : Command
    {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public void Process()
        {
            LOG.Info("AutoNotes Edited");
            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.ANEditedButton);
            CallRecordCore.Instance.CurrentCall.IsANEdited = true;
        }
    }
}
