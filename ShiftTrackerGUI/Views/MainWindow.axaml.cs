using System;
using Avalonia.Controls;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;

namespace ShiftTrackerGUI.Views;

public partial class MainWindow : Window {

    protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();
    public MainWindow() {
        InitializeComponent();

        TrackerSettings.Instance.loadConfigFile();

        this.Position = new Avalonia.PixelPoint(TrackerSettings.Instance.MainWindowPosition.PositionX, TrackerSettings.Instance.MainWindowPosition.PositionY);
        this.Width = TrackerSettings.Instance.MainWindowPosition.Width;
        this.Height = TrackerSettings.Instance.MainWindowPosition.Height;

        this.Closing += (sender, args) => {
            TrackerSettings.Instance.MainWindowPosition.PositionX = this.Position.X;
            TrackerSettings.Instance.MainWindowPosition.PositionY = this.Position.Y;
            TrackerSettings.Instance.MainWindowPosition.Width = this.Width;
            TrackerSettings.Instance.MainWindowPosition.Height = this.Height;

            TrackerSettings.Instance.saveConfigFile();
        };
    }
}
