using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace com.tybern.CallRecordGUI.panels {

    public partial class CallNotesPanel : UserControl {
        public CallNotesPanel() {
            InitializeComponent();
            // txtNotes.Text = Notes;
            // txtNotes.TextChanged += (sender, args) => { Notes = txtNotes.Text; };
        }
        
        public static readonly StyledProperty<string> NotesProperty = AvaloniaProperty.Register<CallNotesPanel, string>("Notes", "");

        public string Notes {
            get { return GetValue(NotesProperty); }
            set { SetValue(NotesProperty, value); }
        }
    }
}