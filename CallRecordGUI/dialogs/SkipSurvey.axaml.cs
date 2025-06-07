using Avalonia.Controls;
using com.tybern.CallRecordCore;
using com.tybern.CallRecordCore.dialogs;

namespace CallRecordGUI.dialogs;

public partial class SkipSurvey : Window {

    private SkipSurveyResult.OptionSkipSurvey SelectedOption { get; set; } = SkipSurveyResult.OptionSkipSurvey.NonFaults;
    private string Text { get; set; } = string.Empty;

    public SkipSurvey() {
        InitializeComponent();

        setContent(rSurveyCallback, com.tybern.CallRecordCore.dialogs.SkipSurveyResult.OptionSkipSurvey.Callback);
        setContent(rSurveyAgent, com.tybern.CallRecordCore.dialogs.SkipSurveyResult.OptionSkipSurvey.Agent);
        setContent(rSurveyTransfer, com.tybern.CallRecordCore.dialogs.SkipSurveyResult.OptionSkipSurvey.Transfer);
        setContent(rSurveyNonTelstra, com.tybern.CallRecordCore.dialogs.SkipSurveyResult.OptionSkipSurvey.NonTelstra);
        setContent(rSurveyNonFaults, com.tybern.CallRecordCore.dialogs.SkipSurveyResult.OptionSkipSurvey.NonFaults);
        setContent(rSurveyPrompted, com.tybern.CallRecordCore.dialogs.SkipSurveyResult.OptionSkipSurvey.None);
        setContent(rSurveyOther, com.tybern.CallRecordCore.dialogs.SkipSurveyResult.OptionSkipSurvey.Other);

        txtOther.TextChanged += (sender, args) => { Text = (txtOther != null && txtOther.Text != null) ? txtOther.Text : string.Empty; };

        rSurveyCallback.IsCheckedChanged += (sender, args) => { if (rSurveyCallback.IsChecked == true) SelectedOption = SkipSurveyResult.OptionSkipSurvey.Callback; };
        rSurveyAgent.IsCheckedChanged += (sender, args) => { if (rSurveyAgent.IsChecked == true) SelectedOption = SkipSurveyResult.OptionSkipSurvey.Agent; };
        rSurveyTransfer.IsCheckedChanged += (sender, args) => { if (rSurveyTransfer.IsChecked == true) SelectedOption = SkipSurveyResult.OptionSkipSurvey.Transfer; };
        rSurveyNonTelstra.IsCheckedChanged += (sender, args) => { if (rSurveyNonTelstra.IsChecked == true) SelectedOption = SkipSurveyResult.OptionSkipSurvey.NonTelstra; };
        rSurveyNonFaults.IsCheckedChanged += (sender, args) => { if (rSurveyNonFaults.IsChecked == true) SelectedOption = SkipSurveyResult.OptionSkipSurvey.NonFaults; };
        rSurveyPrompted.IsCheckedChanged += (sender, args) => { if (rSurveyPrompted.IsChecked == true) SelectedOption = SkipSurveyResult.OptionSkipSurvey.None; };
        rSurveyOther.IsCheckedChanged += (sender, args) => { if (rSurveyOther.IsChecked == true) SelectedOption = SkipSurveyResult.OptionSkipSurvey.Other; };

        btnSaveSurvey.Click += (sender, args) => {
            SkipSurveyResult cmdResult = new SkipSurveyResult(SelectedOption, Text);
            CallRecordCore.Instance.Messages.Enqueue(cmdResult);    // Enqueue processing of the result
            this.Close();                                           // and close the dialog window.
        };
    }

    private void setContent(RadioButton rControl, com.tybern.CallRecordCore.dialogs.SkipSurveyResult.OptionSkipSurvey reason) {
        rControl.Content = com.tybern.CallRecordCore.dialogs.SkipSurveyResult.GetText(reason);
        ToolTip.SetTip(rControl, com.tybern.CallRecordCore.dialogs.SkipSurveyResult.GetTooltip(reason));
    }
}