using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using com.tybern.ShiftTracker;
using static ShiftTrackerGUI.CallControlsView;

namespace ShiftTrackerGUI;

public partial class ExtraControlsView : UserControl {

    [Flags] public enum ExtraControls {
        PreferredName = 1,
        SurveyRequest = 2,
        SkipSurvey = 4,
        AddNote = 8,

        None = 0,
        Initial = AddNote,
        InCall = PreferredName | SurveyRequest | SkipSurvey | AddNote,
        SurveyControls = SurveyRequest | SkipSurvey
    }

    public event CommandEvent? onPreferredName;
    public event CommandEvent? onSurveyRequest;
    public event CommandEvent? onSkipSurvey;
    public event CommandEvent? onAddNote;

    public ExtraControlsView() {
        InitializeComponent();

        btnPrefName.Click += (sender, args) => onPreferredName?.Invoke();
        btnTriggerSurvey.Click += (sender, args) => onSurveyRequest?.Invoke();
        btnSkipSurvey.Click += (sender, args) => onSkipSurvey?.Invoke();
        btnAddNote.Click += (sender, args) => onAddNote?.Invoke();
    }

    public void EnableButtons(ExtraControls extraControls) {
        doSetEnabled(btnPrefName, extraControls.HasFlag(ExtraControls.PreferredName));
        doSetEnabled(btnTriggerSurvey, extraControls.HasFlag(ExtraControls.SurveyRequest));
        doSetEnabled(btnSkipSurvey, extraControls.HasFlag(ExtraControls.SkipSurvey));
        doSetEnabled(btnAddNote, extraControls.HasFlag(ExtraControls.AddNote));
    }

    public void DisableButton(ExtraControls ecButton) {
        if (ecButton.HasFlag(ExtraControls.PreferredName)) doSetEnabled(btnPrefName, false);
        if (ecButton.HasFlag(ExtraControls.SurveyRequest)) doSetEnabled(btnTriggerSurvey, false);
        if (ecButton.HasFlag(ExtraControls.SkipSurvey)) doSetEnabled(btnSkipSurvey, false);
        if (ecButton.HasFlag(ExtraControls.AddNote)) doSetEnabled(btnAddNote, false);
    }

    private void doSetEnabled(Button btn, bool isEnabled) {
        if (Dispatcher.UIThread.CheckAccess())
            btn.IsEnabled = isEnabled;
        else
            Dispatcher.UIThread.Invoke(() => btn.IsEnabled = isEnabled);
    }
}