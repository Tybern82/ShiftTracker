using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShiftTrackerGUI.Views;

public partial class ShiftWeekWindow : Window {

    public ShiftWeekWindow() : this(DateTime.Now.Date) {}
    public ShiftWeekWindow(DateTime dt) {
        InitializeComponent();

        vShiftWeek.dtSelectWeek.SelectedDate = dt;

        vShiftWeek.onClose += () => this.Close();   // close window when button clicked (save is handled in View)

        this.Closing += (sender, args) => vShiftWeek.saveAll(); // ensure changes are saved in case we close from window
    }
}