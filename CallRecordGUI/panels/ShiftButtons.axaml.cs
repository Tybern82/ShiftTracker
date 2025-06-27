using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using com.tybern.ShiftTracker;

namespace com.tybern.CallRecordGUI.panels;

public partial class ShiftButtons : UserControl, IShiftControls {

    public event CommandEvent? ShiftStart;
    public event CommandEvent? ShiftEnd;
    public event CommandEvent? BreakStart;
    public event CommandEvent? BreakEnd;

    private void onShiftStart() => ShiftStart?.Invoke();
    private void onShiftEnd() => ShiftEnd?.Invoke();
    private void onBreakStart() => BreakStart?.Invoke();
    private void onBreakEnd() => BreakEnd?.Invoke();

    public ShiftButtons() {
        InitializeComponent();

        btnStartShift.Click += (sender, args) => onShiftStart();
        btnEndShift.Click += (sender, args) => onShiftEnd();
        btnStartBreak.Click += (sender, args) => onBreakStart();
        btnEndBreak.Click += (sender, args) => onBreakEnd();
    }

    public bool EnableShiftStart {
        set { doSetEnabled(btnStartShift, value); }
    }

    public bool EnableShiftEnd {
        set { doSetEnabled(btnEndShift, value); }
    }

    public bool EnableBreakStart {
        set { doSetEnabled(btnStartBreak, value); }
    }

    public bool EnableBreakEnd {
        set { doSetEnabled(btnEndBreak, value); }
    }

    private void doSetEnabled(Button btn, bool isEnabled) {
        if (Dispatcher.UIThread.CheckAccess())
            btn.IsEnabled = isEnabled;
        else
            Dispatcher.UIThread.Invoke(() => btn.IsEnabled = isEnabled);
    }
}