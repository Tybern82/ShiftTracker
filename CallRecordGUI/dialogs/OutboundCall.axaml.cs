using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.CallRecordCore;
using com.tybern.CallRecordCore.dialogs;
using static com.tybern.CallRecordCore.dialogs.OutboundCallResult;
using static com.tybern.CallRecordCore.dialogs.TransferRequestResult;
using static SQLite.SQLite3;

namespace CallRecordGUI.dialogs;

public partial class OutboundCall : Window {

    private OptionOutboundCall Result { get; set; } = OptionOutboundCall.NetworkReset;
    private string Text { get; set; } = string.Empty;

    public OutboundCall() {
        InitializeComponent();

        setContent(rDisconnect, OptionOutboundCall.Disconnect);
        setContent(rChangeNumber, OptionOutboundCall.ChangeNumber);
        setContent(rNetworkReset, OptionOutboundCall.NetworkReset);
        setContent(rOutboundOther, OptionOutboundCall.Other);

        txtOther.TextChanged += (sender, args) => { Text = (txtOther != null && txtOther.Text != null) ? txtOther.Text : string.Empty; };

        btnSaveOutbound.Click += (sender, args) => {
            CallRecordCore.Instance.Messages.Enqueue(new OutboundCallResult(Result, Text));
            this.Close(); 
        };
    }

    private void setContent(RadioButton rControl, com.tybern.CallRecordCore.dialogs.OutboundCallResult.OptionOutboundCall option) {
        rControl.Content = com.tybern.CallRecordCore.dialogs.OutboundCallResult.GetText(option);
        ToolTip.SetTip(rControl, com.tybern.CallRecordCore.dialogs.OutboundCallResult.GetToolTip(option));
        rControl.IsCheckedChanged += (sender, args) => updateChecked(rControl, option);
    }

    private void updateChecked(RadioButton rControl, OptionOutboundCall value) {
        if (rControl.IsChecked == true) Result = value;
    }
}