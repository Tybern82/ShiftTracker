using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using com.tybern.ShiftTracker;

namespace ShiftTrackerGUI;

public partial class CallControlsView : UserControl {

    [Flags] public enum CallControls {
        CallStart = 1,
        CallEnd = 2,
        CallSME = 4,
        CallTransfer = 8,

        None = 0,
        Waiting = CallStart | CallSME,
        InCall = CallEnd | CallSME | CallTransfer,
    }

    public event CommandEvent? onCallStart;
    public event CommandEvent? onCallSME;
    public event CommandEvent? onCallTransfer;
    public event CommandEvent? onCallEnd;

    public CallControlsView() {
        InitializeComponent();

        btnStartCall.Click += (sender, args) => onCallStart?.Invoke();
        btnCallSME.Click += (sender, args) => onCallSME?.Invoke();
        btnCallTransfer.Click += (sender, args) => onCallTransfer?.Invoke();
        btnEndCall.Click += (sender, args) => onCallEnd?.Invoke();
    }

    public void EnableButtons(CallControls callControls) {
        doSetEnabled(btnStartCall, callControls.HasFlag(CallControls.CallStart));
        doSetEnabled(btnCallSME, callControls.HasFlag(CallControls.CallSME));
        doSetEnabled(btnCallTransfer, callControls.HasFlag(CallControls.CallTransfer));
        doSetEnabled(btnEndCall, callControls.HasFlag(CallControls.CallEnd));
    }

    private void doSetEnabled(Button btn, bool isEnabled) {
        if (Dispatcher.UIThread.CheckAccess())
            btn.IsEnabled = isEnabled;
        else
            Dispatcher.UIThread.Invoke(() => btn.IsEnabled = isEnabled);
    }
}