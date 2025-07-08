using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using static ShiftTrackerGUI.SMECallView;

namespace ShiftTrackerGUI;

public partial class SMECallWindow : Window {

    public SMECallWindow() : this(new BasicNoteStore()) { }
    public SMECallWindow(NoteStore notes) {
        InitializeComponent();

        WindowPosition? wPos = TrackerSettings.Instance.loadWindow(nameof(SMECallWindow));
        if (wPos != null) Utility.setPosition(this, wPos);

        this.Closing += (sender, args) => {
            vSMECall.doClose(); // make sure the close is properly processed (in case user clicks close, rather than "Save")
        };

        vSMECall.onSMEClose += (type, time, details) => {
            string typeText = com.tybern.ShiftTracker.EnumConverter.GetEnumDescription(type);
            string timeText = ClockTimer.toShortTimeString(time);

            notes.appendNote("SME Call <" + timeText + "> - " + typeText);
            notes.appendNote(details);

            TrackerSettings.Instance.saveWindow(nameof(SMECallWindow), Utility.getPosition(this));

            this.Close();
        };
    }
}