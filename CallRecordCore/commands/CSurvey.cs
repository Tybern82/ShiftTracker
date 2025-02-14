using com.tybern.CMDProcessor;
using static com.tybern.CallRecordCore.dialogs.SkipSurveyResult;

namespace com.tybern.CallRecordCore.commands {
    public class CSurvey : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public void Process() {
            LOG.Info("Record Survey");
            CallRecordCore.Instance.CurrentCall.IsSurveyRecorded = true;
            CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.SurveyButtons);
            CallRecordCore.Instance.ShiftCounter.CallNumber++;
            CallRecordCore.Instance.Messages.Enqueue(new AddSurveyRecord(CallRecordCore.Instance.ShiftCounter.CallNumber, true, OptionSkipSurvey.None, string.Empty));
        }
    }
}
