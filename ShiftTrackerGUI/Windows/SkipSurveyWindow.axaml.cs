using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;

namespace ShiftTrackerGUI;

public partial class SkipSurveyWindow : Window {
    public SkipSurveyWindow() {
        InitializeComponent();

        WindowPosition? wPos = TrackerSettings.Instance.loadWindow(nameof(SkipSurveyWindow));
        if (wPos != null) Utility.setPosition(this, wPos);

        this.Closing += (sender, args) => TrackerSettings.Instance.saveWindow(nameof(SkipSurveyWindow), Utility.getPosition(this));

    }
}