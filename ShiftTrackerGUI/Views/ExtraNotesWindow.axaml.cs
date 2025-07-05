using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using ShiftTrackerGUI.ViewModels;

namespace ShiftTrackerGUI;

public partial class ExtraNotesWindow : Window {

    public delegate void SaveNoteEvent(string note);

    public event SaveNoteEvent? onSave;
    public event CommandEvent? onCancel;

    private NoteStore _NoteStorage;
    public NoteStore NoteStorage {
        get { return _NoteStorage; }
        set { _NoteStorage = value; DataContext = value; }
    }

    public ExtraNotesWindow() : this(new BasicNoteStore()) { }

    public ExtraNotesWindow(NoteStore dataStore) {
        InitializeComponent();

        this._NoteStorage = dataStore;
        this.DataContext = dataStore;

        btnSave.Click += (sender, args) => onSave?.Invoke(dataStore.NoteContent);
        btnCancel.Click += (sender, args) => onCancel?.Invoke();
    }
}