using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.db;

namespace com.tybern.CallRecordGUI.panels {

    public partial class SMTPPanel : UserControl {

        public event DataUpdate? UpdateHost;
        public event DataUpdate? UpdatePort;
        public event DataUpdate? UpdateUsername;
        public event DataUpdate? UpdatePassword;
        public event DataUpdate? UpdateSender;
        public event DataUpdate? UpdateDestination;

        private SMTPRecord _SMTP = new SMTPRecord();
        public SMTPRecord SMTP { 
            get { return _SMTP; }
            set { _SMTP = value; DataContext = _SMTP; }
        }

        public SMTPPanel() {
            InitializeComponent();
            DataContext = SMTP;

            spinSMTPPort.Spin += (sender, args) => {
                SpinEventArgs spinArgs = args as SpinEventArgs;
                switch (spinArgs.Direction) {
                    case SpinDirection.Increase:
                        SMTP.Port++;
                        UpdatePort?.Invoke();
                        break;

                    case SpinDirection.Decrease:
                        SMTP.Port--;
                        UpdatePort?.Invoke();
                        break;
                }
                if (SMTP.Port == 1) spinSMTPPort.ValidSpinDirection = ValidSpinDirections.Increase;
                else if (SMTP.Port == 65535) spinSMTPPort.ValidSpinDirection = ValidSpinDirections.Decrease;
                else spinSMTPPort.ValidSpinDirection = ValidSpinDirections.Increase | ValidSpinDirections.Decrease;
            };

            txtHost.TextChanged += (sender, args) => UpdateHost?.Invoke();
            txtUsername.TextChanged += (sender, args) => UpdateUsername?.Invoke();
            txtPassword.TextChanged += (sender, args) => UpdatePassword?.Invoke();
            txtSender.TextChanged += (sender, args) => UpdateSender?.Invoke();
            txtDestination.TextChanged += (sender, args) => UpdateDestination?.Invoke();
        }

        private void FSMTPPort_TextChanged(object? sender, TextChangedEventArgs e) {
            try {
                if (sender == null) return;
                TextBox fSMTPPort = (TextBox)sender;
                if (fSMTPPort.Text != null) {
                    int val = int.Parse(fSMTPPort.Text);
                    SMTP.Port = val;
                    UpdatePort?.Invoke();
                }
            } catch (Exception) { }
        }
    }
}