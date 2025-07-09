using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.enums;
using static ShiftTrackerGUI.Views.CallTransferView;

namespace ShiftTrackerGUI.Views;

public partial class CallbackView : UserControl {

    public enum CallbackType {
        [Description("Caller Disconnected")]
        [TooltipDescription("Inbound call has dropped; calling customer back")]
        Disconnect,

        [Description("Change of Number")]
        [TooltipDescription("Callback to alternate number to allow testing")]
        ChangeNumber,

        [Description("Network Reset")]
        [TooltipDescription("Network reset on device; calling customer back after restart")]
        NetworkReset,

        [Description("UTCC - No Response")]
        [TooltipDescription("UTCC - unable to reach customer, retrying")]
        UTCC,

        [Description("Outbound Authentication")]
        [TooltipDescription("Calling back to complete outbound authentication")]
        OutboundAuth,

        [Description("Other / Unspecified")]
        [TooltipDescription("Other / Unspecified callback")]
        Unspecified
    }

    public delegate void CallbackEvent(CallbackType type, string details);

    public event CallbackEvent? onCallback;

    private CallbackType type;
    private bool isClosing = false;

    public CallbackView() {
        InitializeComponent();

        setContent(rDisconnect, CallbackType.Disconnect);
        setContent(rChangeNumber, CallbackType.ChangeNumber);
        setContent(rNetworkReset, CallbackType.NetworkReset);
        setContent(rUTCC, CallbackType.UTCC);
        setContent(rOutboundAuth, CallbackType.OutboundAuth);
        setContent(rUnspecified, CallbackType.Unspecified);

        rNetworkReset.IsChecked = true;

        btnCloseCallback.Click += (sender, args) => onCallback?.Invoke(type, txtDetails.Text ?? string.Empty);

        onCallback += (type, details) => {
            this.isClosing = true;
        };
    }

    public void doClose() {
        if (!isClosing) {
            if (Dispatcher.UIThread.CheckAccess())
                onCallback?.Invoke(type, txtDetails.Text ?? string.Empty);
            else
                Dispatcher.UIThread.Invoke(() => onCallback?.Invoke(type, txtDetails.Text ?? string.Empty));
        }
    }

    private void setContent(RadioButton rb, CallbackType type) {
        rb.Content = com.tybern.ShiftTracker.EnumConverter.GetEnumDescription(type);
        ToolTip.SetTip(rb, com.tybern.ShiftTracker.EnumConverter.GetEnumTooltip(type));
        rb.IsCheckedChanged += (sender, args) => {
            if (rb.IsChecked == true) this.type = type;
        };
    }
}