using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CSetSurvey : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private bool isRecorded = false;

        public CSetSurvey(bool isRecorded) { this.isRecorded = isRecorded; }

        public void Process() {
            LOG.Info("Restore Survey Status");
            CallRecordCore.Instance.CurrentCall.IsSurveyRecorded = isRecorded;
            if (isRecorded) CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.SurveyButtons);
        }
    }
}
