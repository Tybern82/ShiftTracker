using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CSetCallback : Command {
        public void Process() {
            CallRecordCore.Instance.CurrentCall.IsCallback = true;
        }
    }
}
