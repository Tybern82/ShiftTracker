using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;
using static com.tybern.CallRecordCore.dialogs.SkipSurveyResult;

namespace com.tybern.CallRecordCore.commands {
    public class AddSurveyRecord : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private SurveyRecord Record { get; set; }
        private bool DBLoaded { get; set; } = false;


        public AddSurveyRecord(int callNumber, bool isPrompted, OptionSkipSurvey type, string text) : this(new SurveyRecord(DateTime.Now, callNumber, isPrompted, type, text), false) {}

        public AddSurveyRecord(SurveyRecord record, bool isPreloaded) {
            Record = record; 
            DBLoaded = isPreloaded;
        }

        public void Process() {
            LOG.Info("SURVEY: " + Record.AsString);
            if (!DBLoaded) CallRecordCore.Instance.SurveyRecordLog.AddRecord(Record);
            CallRecordCore.Instance.UIProperties.SurveyRecordList.Add(Record);
        }
    }
}
