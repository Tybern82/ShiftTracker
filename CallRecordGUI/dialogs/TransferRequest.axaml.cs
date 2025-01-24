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
        setContent(rTransferOther, OptionTransferRequest.Other);

        txtOther.TextChanged += (sender, args) => { Text = (txtOther != null && txtOther.Text != null) ? txtOther.Text : string.Empty; };

        rTransferBilling.IsCheckedChanged += (sender, args) => updateChecked(rTransferBilling, OptionTransferRequest.Billing);
        rTransferFAST.IsCheckedChanged += (sender, args) => updateChecked(rTransferFAST, OptionTransferRequest.FAST);
        rTransferCM.IsCheckedChanged += (sender, args) => updateChecked(rTransferCM, OptionTransferRequest.ConnectionManagement);
        rTransferPSTN.IsCheckedChanged += (sender, args) => updateChecked(rTransferPSTN, OptionTransferRequest.PSTN);
        rTransferBusiness.IsCheckedChanged += (sender, args) => updateChecked(rTransferBusiness, OptionTransferRequest.Business);
        rTransferFoxtel.IsCheckedChanged += (sender, args) => updateChecked(rTransferFoxtel, OptionTransferRequest.Foxtel);
        rTransferNFS.IsCheckedChanged += (sender, args) => updateChecked(rTransferNFS, OptionTransferRequest.NFS);
        rTransferOther.IsCheckedChanged += (sender, args) => updateChecked(rTransferOther, OptionTransferRequest.Other);

        btnSaveTransfer.Click += (sender, args) => {
            CallRecordCore.Instance.Messages.Enqueue(new TransferRequestResult(Result, Text));
            this.Close();
        };
    }

    private void setContent(RadioButton rControl, com.tybern.CallRecordCore.dialogs.TransferRequestResult.OptionTransferRequest option) {
        rControl.Content = com.tybern.CallRecordCore.dialogs.TransferRequestResult.GetText(option);
        ToolTip.SetTip(rControl, com.tybern.CallRecordCore.dialogs.TransferRequestResult.GetTooltip(option));
    }

    private void updateChecked(RadioButton rControl, OptionTransferRequest value) {
        if (rControl.IsChecked == true) Result = value;
    }
}