using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CallRecordCore;
using static CallRecordCore.MultipleTransfer;

namespace CallRecordGUI.dialogs;

public partial class MultipleTransfer : Window {

    public MultipleTransferResult Result { get; set; } = new MultipleTransferResult(MultipleTransferOption.Misrouted);
    private CallUI callUI { get; }

    public MultipleTransfer(CallUI callUI) {
        this.callUI = callUI;
        InitializeComponent();

        setContent(rMisrouted, MultipleTransferOption.Misrouted);
        setContent(rDrop, MultipleTransferOption.CallDrop);
        setContent(rAgentDrop, MultipleTransferOption.AgentDrop);
        setContent(rIncorrect, MultipleTransferOption.Incorrect);
        setContent(rMAEOther, MultipleTransferOption.Other);

        rMisrouted.IsCheckedChanged += (sender, args) => { if (rMisrouted.IsChecked == true) Result.Reason = MultipleTransferOption.Misrouted; };
        rDrop.IsCheckedChanged      += (sender, args) => { if (rDrop.IsChecked == true) Result.Reason = MultipleTransferOption.CallDrop; };
        rAgentDrop.IsCheckedChanged += (sender, args) => { if (rAgentDrop.IsChecked == true) Result.Reason = MultipleTransferOption.AgentDrop; };
        rIncorrect.IsCheckedChanged += (sender, args) => { if (rIncorrect.IsChecked == true) Result.Reason = MultipleTransferOption.Incorrect; };
        rMAEOther.IsCheckedChanged  += (sender, args) => { if (rMAEOther.IsChecked == true) Result.Reason = MultipleTransferOption.Other; };

        btnSaveMAE.Click += (sender, args) => { 
            this.Close();
            callUI.doSaveMAE(Result.Reason, Result.Text);
        };
        txtOther.TextChanged += onChange_txtOther;
    }

    private void setContent(RadioButton rControl, CallRecordCore.MultipleTransfer.MultipleTransferOption option) {
        rControl.Content = CallRecordCore.MultipleTransfer.getMultipleTransferOptionsText(option);
        ToolTip.SetTip(rControl, CallRecordCore.MultipleTransfer.getMultipleTransferOptionsTooltip(option));
    }

    private void onChange_txtOther(object? sender, TextChangedEventArgs e) {
        Result.Text = (txtOther != null && txtOther.Text != null) ? txtOther.Text : string.Empty;
    }
}