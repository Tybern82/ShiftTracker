using Avalonia;
using Avalonia.Controls.Primitives;
using com.tybern.CallRecordGUI.panels;

namespace com.tybern.CallRecordGUI.panels;

public class NotesControl : TemplatedControl {

    public static readonly StyledProperty<string> NotesProperty = AvaloniaProperty.Register<CallNotesPanel, string>(name:"Notes", defaultValue:"", defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);

    public string Notes {
        get { return GetValue(NotesProperty); }
        set { SetValue(NotesProperty, value); }
    }
}