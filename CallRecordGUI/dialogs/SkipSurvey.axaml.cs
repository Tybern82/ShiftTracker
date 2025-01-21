using Avalonia.Controls;

namespace CallRecordGUI.dialogs;

public partial class SkipSurvey : Window {

    public CallRecordCore.SkipSurvey.SkipSurveyResult Result { get; } = new CallRecordCore.SkipSurvey.SkipSurveyResult(CallRecordCore.SkipSurvey.SkipSurveyOption.None);

    public SkipSurvey() {
        InitializeComponent();

        setContent(rSurveyCallback, CallRecordCore.SkipSurvey.SkipSurveyOption.Callback);
        setContent(rSurveyAgent, CallRecordCore.SkipSurvey.SkipSurveyOption.Agent);
        setContent(rSurveyTransfer, CallRecordCore.SkipSurvey.SkipSurveyOption.Transfer);
        setContent(rSurveyNonTelstra, CallRecordCore.SkipSurvey.SkipSurveyOption.NonTelstra);
        setContent(rSurveyPrompted, CallRecordCore.SkipSurvey.SkipSurveyOption.None);
        setContent(rSurveyOther, CallRecordCore.SkipSurvey.SkipSurveyOption.Other);

        rSurveyCallback.IsCheckedChanged += onChange_rSurveyCallback;
        rSurveyAgent.IsCheckedChanged += onChange_rSurveyAgent;
        rSurveyTransfer.IsCheckedChanged += onChange_rSurveyTransfer;
        rSurveyNonTelstra.IsCheckedChanged += onChange_rSurveyNonTelstra;
        rSurveyPrompted.IsCheckedChanged += onChange_rSurveyPrompted;
        rSurveyOther.IsCheckedChanged += onChange_rSurveyOther;

        btnSaveSurvey.Click += onClick_btnSaveSurvey;
        txtOther.TextChanged += onChange_txtOther;
    }

    private void setContent(RadioButton rControl, CallRecordCore.SkipSurvey.SkipSurveyOption reason) {
        rControl.Content = CallRecordCore.SkipSurvey.getSkipSurveyOptionText(reason);
        ToolTip.SetTip(rControl, CallRecordCore.SkipSurvey.getSkipSurveyOptionTooltip(reason));
    }

    private void onChange_rSurveyOther(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rSurveyOther.IsChecked == true) Result.Reason = CallRecordCore.SkipSurvey.SkipSurveyOption.Other;
    }

    private void onChange_rSurveyPrompted(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rSurveyPrompted.IsChecked == true) Result.Reason = CallRecordCore.SkipSurvey.SkipSurveyOption.None;
    }

    private void onChange_rSurveyNonTelstra(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rSurveyNonTelstra.IsChecked == true) Result.Reason = CallRecordCore.SkipSurvey.SkipSurveyOption.NonTelstra;
    }

    private void onChange_rSurveyTransfer(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rSurveyTransfer.IsChecked == true) Result.Reason = CallRecordCore.SkipSurvey.SkipSurveyOption.Transfer;
    }

    private void onChange_rSurveyCallback(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rSurveyCallback.IsChecked == true) Result.Reason = CallRecordCore.SkipSurvey.SkipSurveyOption.Callback;
    }

    private void onChange_rSurveyAgent(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (rSurveyAgent.IsChecked == true) Result.Reason = CallRecordCore.SkipSurvey.SkipSurveyOption.Agent;
    }

    private void onChange_txtOther(object? sender, TextChangedEventArgs e) {
        Result.Text = (txtOther != null && txtOther.Text != null) ? txtOther.Text : string.Empty;
    }

    private void onClick_btnSaveSurvey(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        // Nothing to do here, just close the window
        this.Close();
    }
}