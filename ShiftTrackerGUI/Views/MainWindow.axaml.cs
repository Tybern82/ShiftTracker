using System;
using Avalonia.Controls;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;

namespace ShiftTrackerGUI.Views;

public partial class MainWindow : Window {

    protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();
    public MainWindow() {
        InitializeComponent();

        TrackerSettings.Instance.loadConfigFile();

        this.Position = new Avalonia.PixelPoint(TrackerSettings.Instance.MainWindowPosition.PositionX, TrackerSettings.Instance.MainWindowPosition.PositionY);
        this.Width = TrackerSettings.Instance.MainWindowPosition.Width;
        this.Height = TrackerSettings.Instance.MainWindowPosition.Height;

        pMainView.pShiftTimes.ActiveShift = DBShiftTracker.Instance.loadWorkShift(DateTime.Now.Date) ?? new WorkShift(DateTime.Now);
        pMainView.pShiftTimes.onSelectDate += (oldDate, newDate) => {
            // Save the current date to the DB and load the new date record
            WorkShift currShift = pMainView.pShiftTimes.ActiveShift;
            pMainView.pShiftTimes.ActiveShift = DBShiftTracker.Instance.loadWorkShift(newDate) ?? new WorkShift(newDate);
            currShift.CurrentDate = oldDate;
            DBShiftTracker.Instance.save(currShift);
        };

        pMainView.pShiftTimes.addDefaultHandlers(); // add the standard commands that operate with the ActiveShift

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
