using System;
using System.Collections.Generic;
using Avalonia.Controls;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;
using ShiftTrackerGUI.ViewModels;
using StateMachine;

namespace ShiftTrackerGUI.Views;

public partial class MainView : UserControl {

    public MainViewModel ViewModel { get; private set; } = new MainViewModel();

    public MainView() {
        InitializeComponent();

        DataContext = ViewModel;

        pShiftTimes.ActiveShift = DBShiftTracker.Instance.loadWorkShift(DateTime.Now.Date) ?? new WorkShift(DateTime.Now);
        ViewModel.ShiftState.ActiveShift = pShiftTimes.ActiveShift;

        pShiftTimes.onSelectDate += (oldDate, newDate) => {
            // Save the current date to the DB and load the new date record
            WorkShift currShift = pShiftTimes.ActiveShift;
            pShiftTimes.ActiveShift = DBShiftTracker.Instance.loadWorkShift(newDate) ?? new WorkShift(newDate);
            ViewModel.ShiftState.ActiveShift = pShiftTimes.ActiveShift;
            currShift.CurrentDate = oldDate;
            DBShiftTracker.Instance.save(currShift);
        };

        pShiftTimes.addDefaultHandlers(); // add the standard commands that operate with the ActiveShift
        pCallControls.EnableButtons(CallControlsView.CallControls.None);    // disable all call buttons to start (enabled when shift starts)

        pShiftControls.onShiftStart += () => ViewModel.ShiftState.shiftState.gotoState(State.getState(ShiftSM.SHIFT_INCALLS));
        State.getState(ShiftSM.SHIFT_INCALLS).enterState += (s, param) => {
            pShiftTimes.ActiveShift.doStartShift();
            pShiftControls.EnableButtons(pShiftTimes.ActiveShift.NextBreak != null ? ShiftControlsView.ShiftControls.InCalls : ShiftControlsView.ShiftControls.ShiftEnd);
            pCallControls.EnableButtons(CallControlsView.CallControls.Waiting);
        };

        pShiftControls.onShiftEnd += () => ViewModel.ShiftState.shiftState.gotoState(State.getState(ShiftSM.SHIFT_OFFLINE));
        State.getState(ShiftSM.SHIFT_OFFLINE).enterState += (s, param) => {
            pShiftTimes.ActiveShift.doEndShift();
            pShiftControls.EnableButtons(ShiftControlsView.ShiftControls.Initial);
            pCallControls.EnableButtons(CallControlsView.CallControls.None);
        };

        pShiftControls.onBreakStart += () => ViewModel.ShiftState.shiftState.gotoState(State.getState(ShiftSM.SHIFT_INBREAK));
        pShiftControls.onBreakEnd += () => ViewModel.ShiftState.shiftState.gotoState(State.getState(ShiftSM.SHIFT_INCALLS));
        State.getState(ShiftSM.SHIFT_INBREAK).enterState += (s, param) => {
            ViewModel.ShiftState.CurrentBreak.Clear();
            ViewModel.ShiftState.BreakStartTime = DateTime.Now.TimeOfDay;
            SortedSet<WorkBreak> breaks = pShiftTimes.ActiveShift.doStartBreak();
            foreach (WorkBreak b in breaks) ViewModel.ShiftState.CurrentBreak.Add(b);
            pShiftControls.EnableButtons(ShiftControlsView.ShiftControls.InBreak);
        };
        State.getState(ShiftSM.SHIFT_INBREAK).leaveState += (s, param) => {
            pShiftTimes.ActiveShift.doEndBreak();
            pShiftControls.EnableButtons(pShiftTimes.ActiveShift.NextBreak != null ? ShiftControlsView.ShiftControls.InCalls : ShiftControlsView.ShiftControls.ShiftEnd);
        };

        pCallControls.onCallStart += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_ACTIVE));
        pCallControls.onCallWrap += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_INWRAP));
        pCallControls.onCallback += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_ACTIVE));
        pCallControls.onCallEnd += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_WAITING));
        pCallControls.onCallSME += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_SME));
        pCallControls.onCallTransfer += () => ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_TRANSFER));

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

            // TODO: Save new call record


        };
    }
}
