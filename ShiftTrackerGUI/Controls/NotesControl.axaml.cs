using Avalonia;
using Avalonia.Controls.Primitives;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using ShiftTrackerGUI.ViewModels;

namespace ShiftTrackerGUI.Controls;

public class NotesControl : TemplatedControl, NoteStore {

    public static readonly StyledProperty<string> NoteContentProperty = AvaloniaProperty.Register<NotesControl, string>(name:"NoteContent", defaultValue:"", defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<NotesControl, string>(name:"Title", defaultValue:"Call Notes", defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);

    public string Title {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public string NoteContent {
        get { return GetValue(NoteContentProperty); }
        set { SetValue(NoteContentProperty, value); }
    }

    public void prependNote(string note) => com.tybern.ShiftTracker.Utility.prependNote(this, note);
    public void appendNote(string note) => com.tybern.ShiftTracker.Utility.appendNote(this, note);
}