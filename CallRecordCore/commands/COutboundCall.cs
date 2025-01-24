using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class COutboundCall : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public void Process() {
            LOG.Info("DIALOG <OutboundCall>");
            CallRecordCore.Instance.Messages.Enqueue(new AppendNote("Callback / Outbound"));
            CallRecordCore.Instance.UICallbacks?.ShowDialog(UICallbacks.CallRecordDialog.OutboundCallDialog);
        }
    }
}
