using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;
using com.tybern.ShiftTracker.enums;
using ShiftTrackerGUI.ViewModels;
using StateMachine;
using static ShiftTrackerGUI.Views.CallControlsView;
using static ShiftTrackerGUI.Views.ExtraControlsView;
using static ShiftTrackerGUI.Views.NoteControlsView;
using static ShiftTrackerGUI.Views.ShiftControlsView;

namespace ShiftTrackerGUI.Views;

public partial class MVCallView : UserControl {

    protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

    public MainViewModel? ViewModel { get; private set; } = null;

    public MVCallView() {
        InitializeComponent();
    }

    public void AttachModel(MainViewModel model) {
        this.DataContext = model;
        this.ViewModel = model;


        pCallControls.EnableButtons(CallControlsView.CallControls.None);    // disable all call buttons to start (enabled when shift starts)
        pExtraControls.EnableButtons(ExtraControlsView.ExtraControls.None); // disable all buttons to start (enabled when shift starts)
        pNoteControls.EnableButtons(NoteControlsView.NoteControls.None);    // disable all auto-notes buttons to start (enabled when call starts)

        pCallControls.onCallStart += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_ACTIVE));
        pCallControls.onCallWrap += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_INWRAP));
        pCallControls.onCallback += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_ACTIVE));
        pCallControls.onCallEnd += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_WAITING));
        pCallControls.onCallSME += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_SME));
        pCallControls.onCallTransfer += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_TRANSFER));

        Transition? initialCall = ViewModel.CallState.callState.getTransition(State.getState(CallSM.CALL_WAITING), State.getState(CallSM.CALL_ACTIVE));
        if (initialCall != null) {
            initialCall.onTransition += (initial, final) => {
                pNoteControls.EnableButtons(NoteControlsView.NoteControls.Initial);
                pExtraControls.EnableButtons(ExtraControlsView.ExtraControls.InCall);
            };
        } else {
            LOG.Error("Missing Transition: <Waiting> -> <Active>");
        }

        pNoteControls.onAutoNotesGenerated += () => {
            pNoteControls.EnableButtons(NoteControlsView.NoteControls.HasGenerated);
            if (ViewModel.CallState.CurrentCall != null) {
                ViewModel.CallState.CurrentCall.AutoNotesStatus |= AutoNotesStatus.Generated;
                pNoteControls.DisableButton(ViewModel.CallState.CurrentCall.AutoNotesStatus);
            }
        };

        pNoteControls.onAutoNotesEdited += () => {
            if (ViewModel.CallState.CurrentCall != null) ViewModel.CallState.CurrentCall.AutoNotesStatus |= AutoNotesStatus.Edited;
            pNoteControls.DisableButton(NoteControlsView.NoteControls.Edited);
        };

        pNoteControls.onAutoNotesSaved += () => {
            if (ViewModel.CallState.CurrentCall != null) ViewModel.CallState.CurrentCall.AutoNotesStatus |= AutoNotesStatus.Saved;
            pNoteControls.DisableButton(NoteControlsView.NoteControls.Saved);
        };

        pNoteControls.onManualNotes += () => {
            if (ViewModel.CallState.CurrentCall != null) ViewModel.CallState.CurrentCall.AutoNotesStatus |= AutoNotesStatus.Manual;
            pNoteControls.DisableButton(NoteControlsView.NoteControls.Manual);
        };

        pExtraControls.onPreferredName += () => {
            if (ViewModel.CallState.CurrentCall != null) ViewModel.CallState.CurrentCall.IsPreferredNameRequested = true;
            pExtraControls.DisableButton(ExtraControlsView.ExtraControls.PreferredName);
        };

        pExtraControls.onSurveyRequest += () => {
            if (ViewModel.CallState.CurrentCall != null) ViewModel.CallState.CurrentCall.Survey = SurveyStatus.SurveyRequested;
            pExtraControls.DisableButton(ExtraControlsView.ExtraControls.SurveyControls);   // disable both Survey buttons once triggered
        };

        State.getState(CallSM.CALL_ACTIVE).enterState += (s, param) => {
            pCallControls.setMode(CallControlsView.CallStartButton.Wrap);
            pCallControls.EnableButtons(CallControlsView.CallControls.InCall);
        };

        State.getState(CallSM.CALL_INWRAP).enterState += (s, param) => {
            pCallControls.setMode(CallControlsView.CallStartButton.Callback);
            pCallControls.EnableButtons(CallControlsView.CallControls.InWrap);
        };

        State.getState(CallSM.CALL_WAITING).enterState += (s, param) => {
            pCallControls.setMode(CallControlsView.CallStartButton.Start);
            pCallControls.EnableButtons(CallControlsView.CallControls.Waiting);
            pExtraControls.EnableButtons(ExtraControlsView.ExtraControls.Initial);
        };
    }
}