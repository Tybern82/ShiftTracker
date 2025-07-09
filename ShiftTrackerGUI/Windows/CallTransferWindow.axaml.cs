using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using ShiftTrackerGUI.Views;

namespace ShiftTrackerGUI;

public partial class CallTransferWindow : Window {

    public CallTransferWindow() : this(new BasicNoteStore()) { }
    public CallTransferWindow(NoteStore notes) {
        InitializeComponent();

        WindowPosition? wPos = TrackerSettings.Instance.loadWindow(nameof(CallTransferWindow));
        if (wPos != null) Utility.setPosition(this, wPos);

        this.Closing += (sender, args) => {
            vCallTransfer.doClose(); // make sure the close is properly processed (in case user clicks close, rather than "Save")
        };

        vCallTransfer.onTransferCall += (type, time, details) => addNotes(notes, type, time, details);
        vCallTransfer.onTransferClose += (type, time, details) => addNotes(notes, type, time, details);
    }

    private void addNotes(NoteStore notes, CallTransferView.TransferType type, TimeSpan time, string details) {
        string typeText = com.tybern.ShiftTracker.EnumConverter.GetEnumDescription(type);
        string timeText = ClockTimer.toShortTimeString(time);

        notes.appendNote("Transfer <" + timeText + "> - " + typeText);
        notes.appendNote(details);

        TrackerSettings.Instance.saveWindow(nameof(CallTransferWindow), Utility.getPosition(this));

        this.Close();
    }
}