using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;

namespace ShiftTrackerGUI;

public partial class CallbackWindow : Window {
    public CallbackWindow() {
        InitializeComponent();

        WindowPosition? wPos = TrackerSettings.Instance.loadWindow(nameof(CallbackWindow));
        if (wPos != null) Utility.setPosition(this, wPos);

        this.Closing += (sender, args) => {
            vCallback.doClose();
            TrackerSettings.Instance.saveWindow(nameof(CallbackWindow), Utility.getPosition(this));
        };
    }
}