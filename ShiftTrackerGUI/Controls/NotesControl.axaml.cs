using Avalonia;
using Avalonia.Controls.Primitives;
using com.tybern.ShiftTracker.data;
using ShiftTrackerGUI.ViewModels;

namespace ShiftTrackerGUI.Controls;

public class NotesControl : TemplatedControl, NoteStore {

    public static readonly StyledProperty<string> NoteContentProperty = AvaloniaProperty.Register<NotesControl, string>(name:"NoteContent", defaultValue:"", defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);

    public string NoteContent {
        get { return GetValue(NoteContentProperty); }
        set { SetValue(NoteContentProperty, value); }
    }

    public void prependNote(string note) {
        string sep = (string.IsNullOrEmpty(NoteContent)) ? string.Empty : "\n";
        NoteContent = note + sep + NoteContent;
    }

    public void appendNote(string note) {
        string sep = (string.IsNullOrEmpty(NoteContent)) ? string.Empty : "\n";
        NoteContent += sep + note;
    }
}