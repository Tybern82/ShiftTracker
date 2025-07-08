using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using com.tybern.ShiftTracker;
using Org.BouncyCastle.Asn1.Mozilla;

namespace ShiftTrackerGUI;

public partial class CallControlsView : UserControl {

    private static readonly string BTN_START    = "Start";
    private static readonly string BTN_WRAP     = "Wrap";
    private static readonly string BTN_CALLBACK = "Callback";

    [Flags] public enum CallControls {
        CallStart = 1,
        CallEnd = 2,
        CallSME = 4,
        CallTransfer = 8,

        None = 0,
        Waiting = CallStart | CallSME,
        InCall = CallSME | CallTransfer | CallStart,
        InWrap = CallStart | CallEnd | CallSME,
    }

    public enum CallStartButton {
        Start,
        Wrap,
        Callback
    }

    public event CommandEvent? onCallStart;
    public event CommandEvent? onCallWrap;
    public event CommandEvent? onCallback;
    public event CommandEvent? onCallSME;
    public event CommandEvent? onCallTransfer;
    public event CommandEvent? onCallEnd;

    private CallStartButton buttonState = CallStartButton.Start;

    public CallControlsView() {
        InitializeComponent();

        btnStartCall.Click += (sender, args) => {
            switch (buttonState) {
                case CallStartButton.Start:
                    onCallStart?.Invoke();
                    break;

                case CallStartButton.Wrap:
                    onCallWrap?.Invoke();
                    break;

                case CallStartButton.Callback:
                    onCallback?.Invoke();
                    break;
            }
        };
        btnCallSME.Click += (sender, args) => onCallSME?.Invoke();
        btnCallTransfer.Click += (sender, args) => onCallTransfer?.Invoke();
        btnEndCall.Click += (sender, args) => onCallEnd?.Invoke();
    }

    public void setMode(CallStartButton state) { 
        switch (state) {
            case CallStartButton.Start:
                btnStartCall.Content = BTN_START;
                break;

            case CallStartButton.Wrap:
                btnStartCall.Content = BTN_WRAP;
                break;

            case CallStartButton.Callback:
                btnStartCall.Content = BTN_CALLBACK;
                break;
        }
        buttonState = state;
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