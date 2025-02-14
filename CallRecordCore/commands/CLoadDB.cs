using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CLoadDB : Command {
        public void Process() {
            CallRecordCore.Instance.CallRecordLog.LoadCurrentDay();
            CallRecordCore.Instance.SurveyRecordLog.LoadCurrentDay();
        }
    }
}
