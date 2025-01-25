using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class AddSurveyRecord : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private int CallNumber { get; }
        private bool IsPrompted { get; }
        private string Text { get; } 

        public AddSurveyRecord(int callNumber, bool isPrompted, string text) {
            CallNumber = callNumber;
            IsPrompted = isPrompted;
            Text = text;
        }

        public void Process() {
            LOG.Info("SURVEY: " + CallNumber + ": " + IsPrompted + (string.IsNullOrWhiteSpace(Text) ? string.Empty : " - " + Text));
            SurveyRecord record = new SurveyRecord(CallNumber, IsPrompted, Text);
            CallRecordCore.Instance.UIProperties.SurveyRecordList.Add(record);
        }
    }
}
