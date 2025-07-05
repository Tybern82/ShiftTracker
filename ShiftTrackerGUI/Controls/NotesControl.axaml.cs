using Avalonia;
using Avalonia.Controls.Primitives;
using ShiftTrackerGUI.ViewModels;

namespace ShiftTrackerGUI.Controls;

public class NotesControl : TemplatedControl, NoteStore {

    public static readonly StyledProperty<string> NoteContentProperty = AvaloniaProperty.Register<NotesControl, string>(name:"Notes", defaultValue:"", defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);

    public string NoteContent {
        get { return GetValue(NoteContentProperty); }
        set { SetValue(NoteContentProperty, value); }
    }
}