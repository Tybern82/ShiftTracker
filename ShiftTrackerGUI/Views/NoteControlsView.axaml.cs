using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.enums;
using static ShiftTrackerGUI.CallControlsView;

namespace ShiftTrackerGUI;

public partial class NoteControlsView : UserControl {

    [Flags] public enum NoteControls {
        Generated = 1,
        Edited = 2,
        Saved = 4,
        Manual = 8,

        None = 0,
        Initial = Generated | Manual,
        HasGenerated = Edited | Saved | Manual
    }

    public event CommandEvent? onAutoNotesGenerated;
    public event CommandEvent? onAutoNotesEdited;
    public event CommandEvent? onAutoNotesSaved;
    public event CommandEvent? onManualNotes;

    public NoteControlsView() {
        InitializeComponent();

        btnGenerated.Click += (sender, args) => onAutoNotesGenerated?.Invoke();
        btnEdited.Click += (sender, args) => onAutoNotesEdited?.Invoke();
        btnSaved.Click += (sender, args) => onAutoNotesSaved?.Invoke();
        btnManual.Click += (sender, args) => onManualNotes?.Invoke();
    }

    public void EnableButtons(NoteControls noteControls) {
        doSetEnabled(btnGenerated, noteControls.HasFlag(NoteControls.Generated));
        doSetEnabled(btnEdited, noteControls.HasFlag(NoteControls.Edited));
        doSetEnabled(btnSaved, noteControls.HasFlag(NoteControls.Saved));
        doSetEnabled(btnManual, noteControls.HasFlag(NoteControls.Manual));
    }

    // Helper method to just disable buttons - usually called with single Button value to disable
    public void DisableButton(NoteControls noteControls) {
        if (noteControls.HasFlag(NoteControls.Generated)) doSetEnabled(btnGenerated, false);
        if (noteControls.HasFlag(NoteControls.Edited)) doSetEnabled(btnEdited, false);
        if (noteControls.HasFlag(NoteControls.Saved)) doSetEnabled(btnSaved, false);
        if (noteControls.HasFlag(NoteControls.Manual)) doSetEnabled(btnManual, false);
    }

    public void DisableButton(AutoNotesStatus anStatus) {
        if (anStatus.HasFlag(AutoNotesStatus.Generated)) doSetEnabled(btnGenerated, false);
        if (anStatus.HasFlag(AutoNotesStatus.Edited)) doSetEnabled(btnEdited, false);
        if (anStatus.HasFlag(AutoNotesStatus.Saved)) doSetEnabled(btnSaved, false);
        if (anStatus.HasFlag(AutoNotesStatus.Manual)) doSetEnabled(btnManual, false);
    }

    private void doSetEnabled(Button btn, bool isEnabled) {
        if (Dispatcher.UIThread.CheckAccess())
            btn.IsEnabled = isEnabled;
        else
            Dispatcher.UIThread.Invoke(() => btn.IsEnabled = isEnabled);
    }
}