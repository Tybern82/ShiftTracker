using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.enums;

namespace ShiftTrackerGUI.Views;

public partial class CallTransferView : UserControl {

    public enum TransferType {
        [Description("Billing / Sales / Prepaid")]
        [TooltipDescription("Agent transfer to Billing / Sales departments")]
        Billing,

        [Description("FAST / Credit Management")]
        [TooltipDescription("Agent tranfer to Financial Assistance and Support Team (FAST)")]
        FAST,

        [Description("Connection Management")]
        [TooltipDescription("Agent transfer to Connection Management / Order Remediation team")]
        ConnectionManagement,

        [Description("Fixed / PSTN Services")]
        [TooltipDescription("Agent transfer to Fixed-Line / PSTN Faults team")]
        PSTN,

        [Description("Business Faults")]
        [TooltipDescription("Agent transfer to Business Faults team")]
        Business,

        [Description("Foxtel Faults")]
        [TooltipDescription("Agent transfer to Foxtel (transfers to Foxtel Agent)")]
        Foxtel,

        [Description("5G Home Internet")]
        [TooltipDescription("Agent transfer to 5G Home Internet team")]
        Upfront5G,

        [Description("Need for Speed / ADSL L2 Team")]
        [TooltipDescription("Agent transfer to Need for Speed (ADSL L2 Testers)")]
        NFS,

        [Description("COAT")]
        [TooltipDescription("Agent transfer to Change-of-access-technology (COAT) team")]
        COAT,

        [Description("Moves")]
        [TooltipDescription("Agent transfer to Moves team")]
        Moves,

        [Description("Dedicated Damages")]
        [TooltipDescription("Agent transfer to Dedicated Damages team for infrastructure damage")]
        Damages,

        [Description("Other / Unspecified")]
        [TooltipDescription("Agent transfer to other department; please specify")]
        Unspecified
    }

    public delegate void CallTransferEvent(TransferType type, TimeSpan segmentTime, string txtDetails);

    public event CallTransferEvent? onTransferCall;
    public event CallTransferEvent? onTransferClose;

    private TransferType transferType;

    private bool isClosing = false;

    public CallTransferView() {
        InitializeComponent();

        setContent(rBilling, TransferType.Billing);
        setContent(rFAST, TransferType.FAST);
        setContent(rConnectionManagement, TransferType.ConnectionManagement);
        setContent(rPSTN, TransferType.PSTN);
        setContent(rBusiness, TransferType.Business);
        setContent(rFoxtel, TransferType.Foxtel);
        setContent(rUpfront5G, TransferType.Upfront5G);
        setContent(rNFS, TransferType.NFS);
        setContent(rCOAT, TransferType.COAT);
        setContent(rMoves, TransferType.Moves);
        setContent(rDamages, TransferType.Damages);
        setContent(rUnspecified, TransferType.Unspecified);

        btnTransferCall.Click += (sender, args) => {
            onTransferCall?.Invoke(transferType, tTransferTime.TimerText, txtDetails.Text ?? string.Empty);
        };
        btnCloseTransfer.Click += (sender, args) => {
            onTransferClose?.Invoke(transferType, tTransferTime.TimerText, txtDetails.Text ?? string.Empty);
        };

        transferType = TransferType.Billing;
        rBilling.IsChecked = true;

        onTransferCall += (type, time, details) => {
            isClosing = true;
            tTransferTime.stopTimer();
        };
        onTransferClose += (type, time, details) => {
            isClosing = true;
            tTransferTime.stopTimer();
        };

        tTransferTime.startTimer();
    }

    public void doClose() {
        if (!isClosing) {
            if (Dispatcher.UIThread.CheckAccess())
                onTransferClose?.Invoke(transferType, tTransferTime.TimerText, txtDetails.Text ?? string.Empty);
            else
                Dispatcher.UIThread.Invoke(() => onTransferClose?.Invoke(transferType, tTransferTime.TimerText, txtDetails.Text ?? string.Empty));
        }
    }

    private void setContent(RadioButton rb, TransferType type) {
        rb.Content = com.tybern.ShiftTracker.EnumConverter.GetEnumDescription(type);
        ToolTip.SetTip(rb, com.tybern.ShiftTracker.EnumConverter.GetEnumTooltip(type));
        rb.IsCheckedChanged += (sender, args) => {
            if (rb.IsChecked == true) transferType = type;
        };
    }
}