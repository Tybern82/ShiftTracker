using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;

namespace ShiftTrackerGUI.Views;

public partial class ImmediateBreakView : UserControl {

    public event CommandEvent? onClose;
    public ImmediateBreakView() {
        InitializeComponent();

        pTimer.startTimer();
        btnClose.Click += (sender, args) => onClose?.Invoke();

        onClose += () => pTimer.stopTimer();
    }
}