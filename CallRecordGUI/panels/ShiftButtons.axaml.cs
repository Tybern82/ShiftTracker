using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace com.tybern.CallRecordGUI.panels;

public partial class ShiftButtons : UserControl {

    public delegate void ShiftEvent();

    public event ShiftEvent? ShiftStart;
    public event ShiftEvent? ShiftEnd;
    public event ShiftEvent? BreakStart;
    public event ShiftEvent? BreakEnd;

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
}