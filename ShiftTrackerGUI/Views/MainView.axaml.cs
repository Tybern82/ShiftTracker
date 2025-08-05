using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Controls;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;
using com.tybern.ShiftTracker.enums;
using com.tybern.ShiftTracker.reports;
using ShiftTrackerGUI.ViewModels;
using StateMachine;
using static ShiftTrackerGUI.Views.CallControlsView;
using static ShiftTrackerGUI.Views.ExtraControlsView;
using static ShiftTrackerGUI.Views.ShiftControlsView;

namespace ShiftTrackerGUI.Views;

public partial class MainView : UserControl {

    protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

    public MainViewModel ViewModel { get; private set; } = new MainViewModel(); 

    public MainView() {
        InitializeComponent();

        DataContext = ViewModel;
        vSettings.DataContext = ViewModel;
        vReports.DataContext = ViewModel;

        vCall.AttachModel(ViewModel);



        // pCurrentTime.DataContext = pCurrentTime;
        pShiftTimes.ActiveShift = DBShiftTracker.Instance.loadWorkShift(DateTime.Today) ?? new WorkShift(DateTime.Today);
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

        vCall.pShiftControls.onShiftStart += () => ViewModel.ShiftState.shiftState.gotoState(State.getState(ShiftSM.SHIFT_INCALLS));
        State.getState(ShiftSM.SHIFT_INCALLS).enterState += (s, param) => {
            pShiftTimes.ActiveShift.doStartShift();
            vCall.pShiftControls.EnableButtons(pShiftTimes.ActiveShift.NextBreak != null ? ShiftControlsView.ShiftControls.InCalls : ShiftControlsView.ShiftControls.ShiftEnd);
            vCall.pCallControls.EnableButtons(CallControlsView.CallControls.Waiting);
            vCall.pExtraControls.EnableButtons(ExtraControlsView.ExtraControls.Initial);
        };

        vCall.pShiftControls.onShiftEnd += () => ViewModel.ShiftState.shiftState.gotoState(State.getState(ShiftSM.SHIFT_OFFLINE));
        State.getState(ShiftSM.SHIFT_OFFLINE).enterState += (s, param) => {
            pShiftTimes.ActiveShift.doEndShift();
            vCall.pShiftControls.EnableButtons(ShiftControlsView.ShiftControls.Initial);
            vCall.pCallControls.EnableButtons(CallControlsView.CallControls.None);
            vCall.pExtraControls.EnableButtons(ExtraControlsView.ExtraControls.None);
        };

        vCall.pShiftControls.onBreakStart += () => ViewModel.ShiftState.shiftState.gotoState(State.getState(ShiftSM.SHIFT_INBREAK));
        vCall.pShiftControls.onBreakEnd += () => ViewModel.ShiftState.shiftState.gotoState(State.getState(ShiftSM.SHIFT_INCALLS));
        State.getState(ShiftSM.SHIFT_INBREAK).enterState += (s, param) => {
            ViewModel.ShiftState.CurrentBreak.Clear();
            ViewModel.ShiftState.BreakStartTime = DateTime.Now.TimeOfDay;
            SortedSet<WorkBreak> breaks = pShiftTimes.ActiveShift.doStartBreak();
            foreach (WorkBreak b in breaks) ViewModel.ShiftState.CurrentBreak.Add(b);
            vCall.pShiftControls.EnableButtons(ShiftControlsView.ShiftControls.InBreak);
        };
        State.getState(ShiftSM.SHIFT_INBREAK).leaveState += (s, param) => {
            pShiftTimes.ActiveShift.doEndBreak();
            vCall.pShiftControls.EnableButtons(pShiftTimes.ActiveShift.NextBreak != null ? ShiftControlsView.ShiftControls.InCalls : ShiftControlsView.ShiftControls.ShiftEnd);
        };
    }
}
