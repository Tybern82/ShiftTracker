using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace ShiftTrackerGUI;

public partial class SMECallView : UserControl {

    public delegate void SMECallback(SMECallType type, TimeSpan segmentTime, string txtDetails);

    public event SMECallback? onSMEClose;

    public enum SMECallType {
        [Description("Systems / Program Access")]
        Tools,

        [Description("General Query")]
        Query
    }

    private bool isClosing = false;

    public SMECallView() {
        InitializeComponent();

        rSMETools.Content = com.tybern.ShiftTracker.EnumConverter.GetEnumDescription(SMECallType.Tools);
        rSMEQuery.Content = com.tybern.ShiftTracker.EnumConverter.GetEnumDescription(SMECallType.Query);

        btnSaveSME.Click += (sender, args) => onSMEClose?.Invoke(((rSMETools.IsChecked == true) ? SMECallType.Tools : SMECallType.Query), tSMETime.TimerText, txtDetails.Text ?? string.Empty);

        onSMEClose += (type, time, details) => {
            isClosing = true;
            tSMETime.stopTimer();
        };

        tSMETime.startTimer();
    }

    public void doClose() {
        if (!isClosing) {
            if (Dispatcher.UIThread.CheckAccess())
                onSMEClose?.Invoke(((rSMETools.IsChecked == true) ? SMECallType.Tools : SMECallType.Query), tSMETime.TimerText, txtDetails.Text ?? string.Empty);
            else
                Dispatcher.UIThread.Invoke(() => onSMEClose?.Invoke(((rSMETools.IsChecked == true) ? SMECallType.Tools : SMECallType.Query), tSMETime.TimerText, txtDetails.Text ?? string.Empty));
        }
    }
}