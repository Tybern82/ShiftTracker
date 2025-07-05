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
        ViewModel.ActiveShift = pShiftTimes.ActiveShift;

        pShiftTimes.onSelectDate += (oldDate, newDate) => {
            // Save the current date to the DB and load the new date record
            WorkShift currShift = pShiftTimes.ActiveShift;
            pShiftTimes.ActiveShift = DBShiftTracker.Instance.loadWorkShift(newDate) ?? new WorkShift(newDate);
            ViewModel.ActiveShift = pShiftTimes.ActiveShift;
            currShift.CurrentDate = oldDate;
            DBShiftTracker.Instance.save(currShift);
        };

        pShiftTimes.addDefaultHandlers(); // add the standard commands that operate with the ActiveShift

        pShiftControls.onShiftStart += () => ViewModel.shiftState.gotoState(State.getState(MainViewModel.SHIFT_INCALLS));
        State.getState(MainViewModel.SHIFT_INCALLS).enterState += (s, param) => {
            pShiftTimes.ActiveShift.doStartShift();
            pShiftControls.EnableButtons(pShiftTimes.ActiveShift.NextBreak != null ? ShiftControlsView.ShiftControls.InCalls : ShiftControlsView.ShiftControls.ShiftEnd);
        };

        pShiftControls.onShiftEnd += () => ViewModel.shiftState.gotoState(State.getState(MainViewModel.SHIFT_OFFLINE));
        State.getState(MainViewModel.SHIFT_OFFLINE).enterState += (s, param) => {
            pShiftTimes.ActiveShift.doEndShift();
            pShiftControls.EnableButtons(ShiftControlsView.ShiftControls.Initial);
        };

        pShiftControls.onBreakStart += () => ViewModel.shiftState.gotoState(State.getState(MainViewModel.SHIFT_INBREAK));
        State.getState(MainViewModel.SHIFT_INBREAK).enterState += (s, param) => {
            ViewModel.CurrentBreak.Clear();
            ViewModel.BreakStartTime = DateTime.Now.TimeOfDay;
            SortedSet<WorkBreak> breaks = pShiftTimes.ActiveShift.doStartBreak();
            foreach (WorkBreak b in breaks) ViewModel.CurrentBreak.Add(b);
            pShiftControls.EnableButtons(ShiftControlsView.ShiftControls.InBreak);
        };

        pShiftControls.onBreakEnd += () => {
            ViewModel.shiftState.gotoState(State.getState(MainViewModel.SHIFT_INCALLS));
            pShiftTimes.ActiveShift.doEndBreak();
            pShiftControls.EnableButtons(pShiftTimes.ActiveShift.NextBreak != null ? ShiftControlsView.ShiftControls.InCalls : ShiftControlsView.ShiftControls.ShiftEnd);
        };
    }
}
