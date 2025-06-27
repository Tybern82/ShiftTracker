using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using com.tybern.ShiftTracker;

namespace com.tybern.CallRecordGUI.panels {

    public partial class AutoNotesButtons : UserControl {

        public event CommandEvent? Generated;
        public event CommandEvent? Edited;
        public event CommandEvent? Saved;
        public event CommandEvent? Manual;

        public AutoNotesButtons() {
            InitializeComponent();

            btnIsANGenerated.Click += (sender, args) => Generated?.Invoke();
            btnIsANEdited.Click += (sender, args) => Edited?.Invoke();
            btnIsANSaved.Click += (sender, args) => Saved?.Invoke();
            btnIsANManualSave.Click += (sender, args) => Manual?.Invoke();
        }

        public bool EnableGenerated {
            set { doSetEnabled(btnIsANGenerated, value); }
        }

        public bool EnableEdited {
            set { doSetEnabled(btnIsANEdited, value); }
        }

        public bool EnableSaved {
            set { doSetEnabled(btnIsANSaved, value); }
        }

        public bool EnableManual {
            set { doSetEnabled(btnIsANManualSave, value); }
        }

        private void doSetEnabled(Button btn, bool isEnabled) {
            if (Dispatcher.UIThread.CheckAccess())
                btn.IsEnabled = isEnabled;
            else
                Dispatcher.UIThread.Invoke(() => btn.IsEnabled = isEnabled);
        }
    }
}