using Avalonia.Controls;
using com.tybern.CallRecordCore;
using com.tybern.CallRecordCore.dialogs;
using static com.tybern.CallRecordCore.dialogs.TransferRequestResult;

namespace CallRecordGUI.dialogs;

public partial class TransferRequest : Window {


    private OptionTransferRequest Result { get; set; }

    private string Text { get; set; } = string.Empty;

    public TransferRequest() {
        InitializeComponent();

        DataContext = CallRecordCore.Instance.UIProperties;

        setContent(rTransferBilling, OptionTransferRequest.Billing);
        setContent(rTransferFAST, OptionTransferRequest.FAST);
        setContent(rTransferCM, OptionTransferRequest.ConnectionManagement);
        setContent(rTransferPSTN, OptionTransferRequest.PSTN);
        setContent(rTransferBusiness, OptionTransferRequest.Business);
        setContent(rTransferFoxtel, OptionTransferRequest.Foxtel);
        setContent(rTransferNFS, OptionTransferRequest.NFS);
        setContent(rTransferCOAT, OptionTransferRequest.COAT);
        setContent(rTransferOther, OptionTransferRequest.Other);

        txtOther.TextChanged += (sender, args) => { Text = (txtOther != null && txtOther.Text != null) ? txtOther.Text : string.Empty; };

        btnSaveTransfer.Click += (sender, args) => {
            CallRecordCore.Instance.Messages.Enqueue(new TransferRequestResult(Result, Text));
            this.Close();
        };
    }

    private void setContent(RadioButton rControl, com.tybern.CallRecordCore.dialogs.TransferRequestResult.OptionTransferRequest option) {
        rControl.Content = com.tybern.CallRecordCore.dialogs.TransferRequestResult.GetText(option);
        ToolTip.SetTip(rControl, com.tybern.CallRecordCore.dialogs.TransferRequestResult.GetTooltip(option));
        rControl.IsCheckedChanged += (sender, args) => updateChecked(rControl, option);
    }

    private void updateChecked(RadioButton rControl, OptionTransferRequest value) {
        if (rControl.IsChecked == true) Result = value;
    }
}