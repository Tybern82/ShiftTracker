using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CCopyState : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private bool isRecorded = false;
        private bool isPrefName = false;

        public CCopyState(bool isRecorded, bool isPrefName) { this.isRecorded = isRecorded; this.isPrefName = isPrefName; }

        public void Process() {
            LOG.Info("Restore Survey Status");
            CallRecordCore.Instance.CurrentCall.IsSurveyRecorded = isRecorded;
            if (isRecorded) CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.SurveyButtons);

            LOG.Info("Restore Preferred Name Status");
            CallRecordCore.Instance.CurrentCall.IsPrefNameRequested = isPrefName;
            if (isPrefName) CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.PrefNameButton);
        }
    }
}
