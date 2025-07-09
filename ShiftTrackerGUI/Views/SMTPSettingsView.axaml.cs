using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;

namespace ShiftTrackerGUI.Views;

public partial class SMTPSettingsView : UserControl {

    public event DataUpdate? onUpdateHost;
    public event DataUpdate? onUpdatePort;
    public event DataUpdate? onUpdateUsername;
    public event DataUpdate? onUpdatePassword;
    public event DataUpdate? onUpdateSender;
    public event DataUpdate? onUpdateDestination;

    private SMTPRecord _SMTP = new SMTPRecord();
    public SMTPRecord SMTP {
        get { return _SMTP; }
        set { _SMTP = value; DataContext = _SMTP; }
    }

    public SMTPSettingsView() {
        InitializeComponent();
        DataContext = SMTP;

        spinSMTPPort.Spin += (sender, args) => {
            SpinEventArgs spinArgs = args as SpinEventArgs;
            switch (spinArgs.Direction) {
                case SpinDirection.Increase:
                    SMTP.Port++;
                    onUpdatePort?.Invoke();
                    break;

                case SpinDirection.Decrease:
                    SMTP.Port--;
                    onUpdatePort?.Invoke();
                    break;
            }
            if (SMTP.Port == 1) spinSMTPPort.ValidSpinDirection = ValidSpinDirections.Increase;
            else if (SMTP.Port == 65535) spinSMTPPort.ValidSpinDirection = ValidSpinDirections.Decrease;
            else spinSMTPPort.ValidSpinDirection = ValidSpinDirections.Increase | ValidSpinDirections.Decrease;
        };

        txtHost.TextChanged += (sender, args) => onUpdateHost?.Invoke();
        txtUsername.TextChanged += (sender, args) => onUpdateUsername?.Invoke();
        txtPassword.TextChanged += (sender, args) => onUpdatePassword?.Invoke();
        txtSender.TextChanged += (sender, args) => onUpdateSender?.Invoke();
        txtDestination.TextChanged += (sender, args) => onUpdateDestination?.Invoke();
    }

    private void FSMTPPort_TextChanged(object? sender, TextChangedEventArgs e) {
        try {
            if (sender == null) return;
            TextBox fSMTPPort = (TextBox)sender;
            if (fSMTPPort.Text != null) {
                int val = int.Parse(fSMTPPort.Text);
                SMTP.Port = val;
                onUpdatePort?.Invoke();
            }
        } catch (Exception) { }
    }
}