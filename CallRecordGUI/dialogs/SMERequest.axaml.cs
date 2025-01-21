using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CallRecordGUI.dialogs;

public partial class SMERequest : Window {

    public CallRecordCore.SMERequest.SMERequestResult Result { get; } = new CallRecordCore.SMERequest.SMERequestResult(CallRecordCore.SMERequest.SMERequestOption.Tools);

    public SMERequest() {
        InitializeComponent();

        setContent(rSMETools, CallRecordCore.SMERequest.SMERequestOption.Tools);
        setContent(rSMEQuery, CallRecordCore.SMERequest.SMERequestOption.Query);

        rSMETools.IsCheckedChanged += onChange_rSMETools;
        rSMEQuery.IsCheckedChanged += onChange_rSMEQuery;

        btnSaveSME.Click += onClick_btnSaveSME;
        txtDetails.TextChanged += onChange_txtDetails;
    }

    private void onChange_txtDetails(object? sender, TextChangedEventArgs e) {
        Result.Text = (txtDetails != null && txtDetails.Text != null) ? txtDetails.Text : string.Empty;
    }

    private void onClick_btnSaveSME(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        this.Close();
    }

    private void onChange_rSMEQuery(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rSMEQuery.IsChecked == true) Result.Type = CallRecordCore.SMERequest.SMERequestOption.Query;
    }

    private void onChange_rSMETools(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rSMETools.IsChecked == true) Result.Type = CallRecordCore.SMERequest.SMERequestOption.Tools;
    }

    private void setContent(RadioButton rControl, CallRecordCore.SMERequest.SMERequestOption option) {
        rControl.Content = CallRecordCore.SMERequest.getSMERequestOptionText(option);
        ToolTip.SetTip(rControl, CallRecordCore.SMERequest.getSMERequestOptionTooltip(option));
    }
}