using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands
{
    public class CGenerateAutoNotes : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public void Process()
        {
            LOG.Info("AutoNotes Generated");
            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.ANGeneratedButton);
            CallRecordCore.Instance.UICallbacks?.EnableButton(UICallbacks.UITriggerType.ANEditedButton);
            CallRecordCore.Instance.UICallbacks?.EnableButton(UICallbacks.UITriggerType.ANSavedButton);

            CallRecordCore.Instance.CurrentCall.IsANGenerated = true;
        }
    }
}
