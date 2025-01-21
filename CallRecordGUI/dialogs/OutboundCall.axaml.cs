using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using static CallRecordCore.OutboundCall;
using static SQLite.SQLite3;

namespace CallRecordGUI.dialogs;

public partial class OutboundCall : Window {

    public OutboundCallResult Result { get; set; } = new OutboundCallResult(OutboundCallOption.Disconnect);

    public OutboundCall() {
        InitializeComponent();

        setContent(rDisconnect, OutboundCallOption.Disconnect);
        setContent(rChangeNumber, OutboundCallOption.ChangeNumber);
        setContent(rOutboundOther, OutboundCallOption.Other);

        rDisconnect.IsCheckedChanged += (sender, args) => { if (rDisconnect.IsChecked == true) Result.Reason = OutboundCallOption.Disconnect; };
        rChangeNumber.IsCheckedChanged += (sender, args) => { if (rChangeNumber.IsChecked == true) Result.Reason = OutboundCallOption.ChangeNumber; };
        rOutboundOther.IsCheckedChanged += (sender, args) => { if (rOutboundOther.IsChecked == true) Result.Reason = OutboundCallOption.Other; };

        btnSaveOutbound.Click += (sender, args) => { this.Close(); };
        txtOther.TextChanged += onChange_txtOther;
    }

    private void setContent(RadioButton rControl, CallRecordCore.OutboundCall.OutboundCallOption option) {
        rControl.Content = CallRecordCore.OutboundCall.getOutboundCallOptionText(option);
        ToolTip.SetTip(rControl, CallRecordCore.OutboundCall.getOutboundCallOptionTooltip(option));
    }

    private void onChange_txtOther(object? sender, TextChangedEventArgs e) {
        Result.Text = (txtOther != null && txtOther.Text != null) ? txtOther.Text : string.Empty;
    }
}