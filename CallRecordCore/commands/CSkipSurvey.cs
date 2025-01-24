using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CallRecordCore.old;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CSkipSurvey : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public void Process() {
            LOG.Info("DIALOG <SkipSurvey>");
            CallRecordCore.Instance.CurrentCall.IsSurveyRecorded = true;
            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.SurveyButtons);
            CallRecordCore.Instance.UICallbacks?.ShowDialog(UICallbacks.CallRecordDialog.SkipSurveyDialog);
        }
    }
}
