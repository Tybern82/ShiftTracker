using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;

namespace ShiftTrackerGUI.Views;

public partial class ShiftWeekWindow : Window {

    public ShiftWeekWindow() : this(DateTime.Now.Date) {}
    public ShiftWeekWindow(DateTime dt) {
        InitializeComponent();

        WindowPosition? wPos = TrackerSettings.Instance.loadWindow(nameof(ShiftWeekWindow));
        if (wPos != null) Utility.setPosition(this, wPos);

        vShiftWeek.dtSelectWeek.SelectedDate = dt;

        vShiftWeek.onClose += () => this.Close();   // close window when button clicked (save is handled in View)

        this.Closing += (sender, args) => {
            vShiftWeek.saveAll(); // ensure changes are saved in case we close from window
            TrackerSettings.Instance.saveWindow(nameof(ShiftWeekWindow), Utility.getPosition(this));
        };
    }
}