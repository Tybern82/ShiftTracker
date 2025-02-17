using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CUpdateBreaks : Command {

        private BreakTimeRecord Record { get; }

        public CUpdateBreaks(BreakTimeRecord record) {
            Record = record;
        }

        public void Process() {
            CallRecordCore.Instance.BreakTimesDB.AddRecord(Record);
            // If updating the current date, update the active break times as well
            if (Record.Date == DateTime.Now.Date) CallRecordCore.Instance.UIProperties.BreakTimes.Update(Record);
        }
    }
}
