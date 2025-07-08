using System;
using System.Collections.Generic;
using Avalonia.Controls;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;
using ShiftTrackerGUI.ViewModels;
using StateMachine;

namespace ShiftTrackerGUI.Views;

public partial class MainWindow : Window {

    protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();
    public MainWindow() {
        InitializeComponent();

        TrackerSettings.Instance.loadConfigFile();

        Utility.setPosition(this, TrackerSettings.Instance.MainWindowPosition);

        statusBarText.Text = TrackerSettings.Instance.VersionString;

        pMainView.pShiftTimes.onEditWeek += () => {
            DateTime currentDate = pMainView.pShiftTimes.fDateSelector.SelectedDate.HasValue ? pMainView.pShiftTimes.fDateSelector.SelectedDate.Value.Date : DateTime.Now.Date;
            DBShiftTracker.Instance.save(pMainView.pShiftTimes.ActiveShift);    // save any current edits before trying to load
            ShiftWeekWindow dlgEditWeek = new(currentDate);
            // Reload the date from the current page once the dialog is closed
            dlgEditWeek.Closed += (sender, args) => {
                pMainView.pShiftTimes.ActiveShift = DBShiftTracker.Instance.loadWorkShift(currentDate) ?? new WorkShift(currentDate);
                pMainView.ViewModel.ShiftState.ActiveShift = pMainView.pShiftTimes.ActiveShift;
            };
            dlgEditWeek.ShowDialog(this);
        };

        State.getState(CallSM.CALL_SME).enterState += (s, param) => {
            SMECallWindow wndSMECall = new SMECallWindow(pMainView.ViewModel.CallState);    // link to common notes

            State nextState = ((param != null) && (param is State)) ? (State)param : State.getState(CallSM.CALL_WAITING);   // should always be non-null, but default return to Waiting

            wndSMECall.Closed += (sender, args) => {
                pMainView.ViewModel.CallState.callState.gotoState(nextState);   // change state when closing SME call window
            };

            wndSMECall.ShowDialog(this);
        };

        this.Closing += (sender, args) => {
            // Save the current date to the DB before we close
            WorkShift currentShift = pMainView.pShiftTimes.ActiveShift;
            DBShiftTracker.Instance.save(currentShift);

            // Update the current window position and save the current config settings
            TrackerSettings.Instance.MainWindowPosition.PositionX = this.Position.X;
            TrackerSettings.Instance.MainWindowPosition.PositionY = this.Position.Y;
            TrackerSettings.Instance.MainWindowPosition.Width = this.Width;
            TrackerSettings.Instance.MainWindowPosition.Height = this.Height;

            TrackerSettings.Instance.saveConfigFile();
        };
    }
}
