using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.CallRecordCore;
using com.tybern.CallRecordCore.commands;

namespace CallRecordGUI;

public partial class BreakTimes : Window {
    public BreakTimes() {
        InitializeComponent();
        DataContext = CallRecordCore.Instance.UIProperties;

        btnUpdateBreakTimes.Click += (sender, args) => {
            CallRecordCore.Instance.Messages.Enqueue(new CUpdateBreaks(new BreakTimeRecord(CallRecordCore.Instance.UIProperties.SelectedDate.Date, CallRecordCore.Instance.UIProperties.SelectedBreakTimes)));
            this.Close();
        };
        btnCancelBreakTimes.Click += (sender, args) => this.Close();
        btnResetBreakTimes.Click += (sender, args) => CallRecordCore.Instance.UIProperties.SelectedBreakTimes.Update(new BreakTimeRecord());
    }
}