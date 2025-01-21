using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CallRecordCore;

namespace CallRecordGUI.dialogs;

public partial class TransferRequest : Window {

    public CallRecordCore.TransferRequest.TransferRequestResult Result { get; } = new CallRecordCore.TransferRequest.TransferRequestResult(CallRecordCore.TransferRequest.TransferRequestOption.Billing);
    private CallUI callUI { get; }

    public TransferRequest() : this(new CallUI()) { }
    public TransferRequest(CallUI callUI) {
        this.callUI = callUI;
        InitializeComponent();

        setContent(rTransferBilling, CallRecordCore.TransferRequest.TransferRequestOption.Billing);
        setContent(rTransferFAST, CallRecordCore.TransferRequest.TransferRequestOption.FAST);
        setContent(rTransferCM, CallRecordCore.TransferRequest.TransferRequestOption.ConnectionManagement);
        setContent(rTransferPSTN, CallRecordCore.TransferRequest.TransferRequestOption.PSTN);
        setContent(rTransferBusiness, CallRecordCore.TransferRequest.TransferRequestOption.Business);
        setContent(rTransferFoxtel, CallRecordCore.TransferRequest.TransferRequestOption.Foxtel);
        setContent(rTransferNFS, CallRecordCore.TransferRequest.TransferRequestOption.NFS);
        setContent(rTransferOther, CallRecordCore.TransferRequest.TransferRequestOption.Other);

        rTransferBilling.IsCheckedChanged += onChange_rTransferBilling;
        rTransferFAST.IsCheckedChanged += onChange_rTransferFAST;
        rTransferCM.IsCheckedChanged += onChange_rTransferCM;
        rTransferPSTN.IsCheckedChanged += onChange_rTransferPSTN;
        rTransferBusiness.IsCheckedChanged += onChange_rTransferBusiness;
        rTransferFoxtel.IsCheckedChanged += onChange_rTransferFoxtel;
        rTransferNFS.IsCheckedChanged += onChange_rTransferNFS;
        rTransferOther.IsCheckedChanged += onChange_rTransferOther;

        btnSaveTransfer.Click += onClick_btnSaveTransfer;
        txtOther.TextChanged += onChange_txtOther;
    }

    private void setContent(RadioButton rControl, CallRecordCore.TransferRequest.TransferRequestOption option) {
        rControl.Content = CallRecordCore.TransferRequest.getTransferRequestOptionText(option);
        ToolTip.SetTip(rControl, CallRecordCore.TransferRequest.getTransferRequestOptionTooltip(option));
    }

    private void onChange_rTransferOther(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rTransferOther.IsChecked == true) Result.Destination = CallRecordCore.TransferRequest.TransferRequestOption.Other;
    }

    private void onChange_rTransferNFS(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rTransferNFS.IsChecked == true) Result.Destination = CallRecordCore.TransferRequest.TransferRequestOption.NFS;
    }

    private void onChange_rTransferFoxtel(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rTransferFoxtel.IsChecked == true) Result.Destination = CallRecordCore.TransferRequest.TransferRequestOption.Foxtel;
    }

    private void onChange_rTransferBusiness(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rTransferBusiness.IsChecked == true) Result.Destination = CallRecordCore.TransferRequest.TransferRequestOption.Business;
    }

    private void onChange_rTransferPSTN(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rTransferPSTN.IsChecked == true) Result.Destination = CallRecordCore.TransferRequest.TransferRequestOption.PSTN;
    }

    private void onChange_rTransferCM(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rTransferCM.IsChecked == true) Result.Destination = CallRecordCore.TransferRequest.TransferRequestOption.ConnectionManagement;
    }

    private void onChange_rTransferFAST(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rTransferFAST.IsChecked == true) Result.Destination = CallRecordCore.TransferRequest.TransferRequestOption.FAST;
    }

    private void onChange_rTransferBilling(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rTransferBilling.IsChecked == true) Result.Destination = CallRecordCore.TransferRequest.TransferRequestOption.Billing;
    }

    private void onClick_btnSaveTransfer(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        this.Close();
        callUI.doCloseTransfer(Result.Destination, Result.Text);
    }

    private void onChange_txtOther(object? sender, TextChangedEventArgs e) {
        Result.Text = (txtOther != null && txtOther.Text != null) ? txtOther.Text : string.Empty;
    }
}