using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;

namespace ShiftTrackerGUI;

public partial class ImmediateBreakWindow : Window {
    public ImmediateBreakWindow() {
        InitializeComponent();


        WindowPosition? wPos = TrackerSettings.Instance.loadWindow(nameof(ImmediateBreakWindow));
        if (wPos != null) Utility.setPosition(this, wPos);

        this.Closing += (sender, args) => TrackerSettings.Instance.saveWindow(nameof(ImmediateBreakWindow), Utility.getPosition(this));
    }
}