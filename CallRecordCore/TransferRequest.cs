using System;
using System.Collections.Generic;
using System.Text;

namespace CallRecordCore {
    public class TransferRequest {

        public enum TransferRequestOption {
            Billing, FAST, ConnectionManagement, PSTN, Business, Foxtel, NFS, Other
        }

        public static string getTransferRequestOptionText(TransferRequestOption option) {
            switch (option) {
                case TransferRequestOption.Billing: return "Billing / Sales / Prepaid";
                case TransferRequestOption.FAST: return "FAST / Credit Management";
                case TransferRequestOption.ConnectionManagement: return "Connection Management";
                case TransferRequestOption.PSTN: return "Fixed / PSTN Services";
                case TransferRequestOption.Business: return "Business Faults";
                case TransferRequestOption.Foxtel: return "Foxtel Faults";
                case TransferRequestOption.NFS: return "Need for Speed";

                default: return "Other";
            }
        }

        public static string getTransferRequestOptionTooltip(TransferRequestOption option) {
            switch (option) {
                case TransferRequestOption.Billing: return "Agent transfer to Billing / Sales departments";
                case TransferRequestOption.FAST: return "Agent transfer to Financial Assistance and Support Team (FAST)";
                case TransferRequestOption.ConnectionManagement: return "Agent transfer to Connection Management team";
                case TransferRequestOption.PSTN: return "Agent transfer to Fixed-line / PSTN Faults team";
                case TransferRequestOption.Business: return "Agent transfer to Business Faults team";
                case TransferRequestOption.Foxtel: return "Agent transfer to Foxtel Faults team";
                case TransferRequestOption.NFS: return "Agent transfer to Need for Speed (ADSL L2 Testers)";

                default: return "Other reason - please specify: ";
            }
        }

        public class TransferRequestResult {
            public TransferRequestOption Destination { get; set; }
            public string Text { get; set; }

            public TransferRequestResult(TransferRequestOption option, string text = "") {
                Destination = option;
                Text = text;
            }
        }
    }
}
