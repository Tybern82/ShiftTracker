using Avalonia.Controls;
using com.tybern.CallRecordCore;
using com.tybern.CallRecordCore.dialogs;
using static com.tybern.CallRecordCore.dialogs.MultipleTransferResult;

namespace CallRecordGUI.dialogs;

public partial class MultipleTransfer : Window {

    private OptionMultipleTransfer Result { get; set; }
    private string Text { get; set; } = string.Empty;

    public MultipleTransfer() {
        InitializeComponent();
        DataContext = CallRecordCore.Instance.UIProperties;

        setContent(rMisrouted, OptionMultipleTransfer.Misrouted);
        setContent(rDrop, OptionMultipleTransfer.CallDrop);
        setContent(rAgentDrop, OptionMultipleTransfer.AgentDrop);
        setContent(rIncorrect, OptionMultipleTransfer.Incorrect);
        setContent(rMAEOther, OptionMultipleTransfer.Other);

        txtOther.TextChanged += (sender, args) => { Text = (txtOther != null && txtOther.Text != null) ? txtOther.Text : string.Empty; };

        btnSaveMAE.Click += (sender, args) => {
            CallRecordCore.Instance.Messages.Enqueue(new MultipleTransferResult(Result, Text));
            this.Close();
        };
    }

    private void setContent(RadioButton rControl, com.tybern.CallRecordCore.dialogs.MultipleTransferResult.OptionMultipleTransfer option) {
        rControl.Content = com.tybern.CallRecordCore.dialogs.MultipleTransferResult.GetText(option);
        ToolTip.SetTip(rControl, com.tybern.CallRecordCore.dialogs.MultipleTransferResult.GetToolTip(option));
        rControl.IsCheckedChanged += (sender, args) => updateChecked(rControl, option);
    }

    private void updateChecked(RadioButton rControl, OptionMultipleTransfer value) {
        if (rControl.IsChecked == true) Result = value;
    }
}