using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using com.tybern.ShiftTracker;

namespace ShiftTrackerGUI.Views;

public partial class ShiftControlsView : UserControl {

    [Flags] public enum ShiftControls { 
        // Individual Buttons
        ShiftStart = 1,
        ShiftEnd = 2,
        BreakStart = 4,
        BreakEnd = 8,

        // Standard Button Groups
        None = 0,
        All = ShiftStart | ShiftEnd | BreakStart | BreakEnd,
        Initial = ShiftStart,
        InCalls = BreakStart | ShiftEnd,
        InBreak = BreakEnd | ShiftEnd
    }

    public event CommandEvent? onShiftStart;
    public event CommandEvent? onShiftEnd;
    public event CommandEvent? onBreakStart;
    public event CommandEvent? onBreakEnd;

    public ShiftControlsView() {
        InitializeComponent();

        btnStartShift.Click += (sender, args) => onShiftStart?.Invoke();
        btnEndShift.Click += (sender, args) => onShiftEnd?.Invoke();
        btnStartBreak.Click += (sender, args) => onBreakStart?.Invoke();
        btnEndBreak.Click += (sender, args) => onBreakEnd?.Invoke();
    }

    public void EnableButtons(ShiftControls buttons) {
        doSetEnabled(btnStartShift, buttons.HasFlag(ShiftControls.ShiftStart));
        doSetEnabled(btnEndShift, buttons.HasFlag(ShiftControls.ShiftEnd));
        doSetEnabled(btnStartBreak, buttons.HasFlag(ShiftControls.BreakStart));
        doSetEnabled(btnEndBreak, buttons.HasFlag(ShiftControls.BreakEnd));
    }

    private void doSetEnabled(Button btn, bool isEnabled) {
        if (Dispatcher.UIThread.CheckAccess())
            btn.IsEnabled = isEnabled;
        else
            Dispatcher.UIThread.Invoke(() => btn.IsEnabled = isEnabled);
    }
}