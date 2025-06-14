using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CallRecordCore.commands;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.dialogs {
    public class TransferRequestResult : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private OptionTransferRequest Result { get; }
        private string Text { get; }

        public TransferRequestResult(OptionTransferRequest result, string text = "") { Result = result; Text = text; }

        public void Process() {
            LOG.Info("DIALOG <TransferRequest>: Completed");
            CallRecordCore.Instance.Messages.Enqueue(new AppendNote(GetText(Result)));
            if (!string.IsNullOrWhiteSpace(Text)) CallRecordCore.Instance.Messages.Enqueue(new AppendNote(Text));
            TimeSpan transferTime = DateTime.Now.TimeOfDay - CallRecordCore.Instance.CurrentCall.TransferStartTime;
            CallRecordCore.Instance.ShiftCounter.TotalTransferTime += transferTime;
            CallRecordCore.Instance.CurrentCall.IsInTransferCall = false;
            CallRecordCore.Instance.UIProperties.TransferTime = TimeSpan.Zero;
            CallRecordCore.Instance.Messages.Enqueue(new AppendNote("Transfer Time: " + CallRecordCore.toShortTimeString(transferTime)));
        }

        public enum OptionTransferRequest {
            Billing, FAST, ConnectionManagement, PSTN, Business, Foxtel, NFS, COAT, Other
        }

        public static string GetText(OptionTransferRequest option) {
            switch (option) {
                case OptionTransferRequest.Billing: return "Billing / Sales / Prepaid";
                case OptionTransferRequest.FAST: return "FAST / Credit Management";
                case OptionTransferRequest.ConnectionManagement: return "Connection Management";
                case OptionTransferRequest.PSTN: return "Fixed / PSTN Services";
                case OptionTransferRequest.Business: return "Business Faults";
                case OptionTransferRequest.Foxtel: return "Foxtel Faults";
                case OptionTransferRequest.NFS: return "Need for Speed";
                case OptionTransferRequest.COAT: return "COAT";

                default: return "Other";
            }
        }

        public static string GetTooltip(OptionTransferRequest option) {
            switch (option) {
                case OptionTransferRequest.Billing: return "Agent transfer to Billing / Sales departments";
                case OptionTransferRequest.FAST: return "Agent transfer to Financial Assistance and Support Team (FAST)";
                case OptionTransferRequest.ConnectionManagement: return "Agent transfer to Connection Management team";
                case OptionTransferRequest.PSTN: return "Agent transfer to Fixed-line / PSTN Faults team";
                case OptionTransferRequest.Business: return "Agent transfer to Business Faults team";
                case OptionTransferRequest.Foxtel: return "Agent transfer to Foxtel Faults team";
                case OptionTransferRequest.NFS: return "Agent transfer to Need for Speed (ADSL L2 Testers)";
                case OptionTransferRequest.COAT: return "Agent transfer to Change-of-access-technology (COAT) team";

                default: return "Other reason - please specify: ";
            }
        }
    }
}
