using System;
using Avalonia.Controls;
using com.tybern.ShiftTracker.data;

namespace ShiftTrackerGUI.Views;

public partial class MainWindow : Window {

    protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();
    public MainWindow() {
        InitializeComponent();
    }
}
