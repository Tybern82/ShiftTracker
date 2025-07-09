using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.enums;

namespace ShiftTrackerGUI.Views;

public partial class SkipSurveyView : UserControl {

    public delegate void SkipSurveyEvent(SurveyStatus status);

    public event SkipSurveyEvent? onSkipSurvey;
    public event CommandEvent? onSave;

    public SkipSurveyView() {
        InitializeComponent();

        setContent(rSurveyRequested, SurveyStatus.SurveyRequested);
        setContent(rCallback, SurveyStatus.Callback);
        setContent(rAgent, SurveyStatus.Agent);
        setContent(rNonTelstra, SurveyStatus.NonTelstra);
        setContent(rNonFaults, SurveyStatus.NonFaults);
        setContent(rTransfer, SurveyStatus.Transfer);
        setContent(rUnspecified, SurveyStatus.Unspecified);

        btnSaveSurvey.Click += (sender, args) => onSave?.Invoke();

        rNonFaults.IsChecked = true;
    }

    private void setContent(RadioButton rb, SurveyStatus status) {
        rb.Content = com.tybern.ShiftTracker.EnumConverter.GetEnumDescription(status);
        ToolTip.SetTip(rb, com.tybern.ShiftTracker.EnumConverter.GetEnumTooltip(status));
        rb.IsCheckedChanged += (sender, args) => {
            if (rb.IsChecked == true) onSkipSurvey?.Invoke(status);
        };
    }
}