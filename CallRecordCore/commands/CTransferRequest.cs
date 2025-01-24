using System;
using System.Collections.Generic;
using System.Text;

namespace com.tybern.CallRecordCore.commands {
    public class CTransferRequest : CMDProcessor.Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public void Process() {
            LOG.Info("DIALOG <TransferRequest>");
            CallRecordCore.Instance.UICallbacks?.ShowDialog(UICallbacks.CallRecordDialog.TransferRequestDialog);
        }
    }
}
