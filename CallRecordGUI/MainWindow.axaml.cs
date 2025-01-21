using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CallRecordCore;

namespace CallRecordGUI {
    public partial class MainWindow : Window, UICallbacks {

        private const string lblStartCall = "Start Call";
        private const string lblStartWrap = "Start Wrap";
        private const string lblStartCallback = "Callback";

        private const string lblInboundCall = "Inbound Call";
        private const string lblWaiting = "Waiting";

        private Timer clockTimer;
        private CallUI callUI;

        public MainWindow() {
            InitializeComponent();

            callUI = new CallUI(this);
            clockTimer = new Timer(callUI.doClockUpdate, callUI, 2000, 1000);
            DataContext = callUI;

            btnStartCall.Content = lblStartCall;
            btnStartCall.Click += (sender, args) => {
                if (callUI.CurrentCallDetails.CurrentMode == CurrentCallDetails.CallMode.InCall) {
                    callUI.doStartWrap();
                } else {
                    callUI.doStartCall();
                }
            };

            btnEndCall.Click += (sender, args) => {
                if (callUI.CurrentCallDetails.isInCall) {
                    if (!callUI.CurrentCallDetails.isInWrap) callUI.doStartWrap();
                    callUI.doStopCall();
                    if (!callUI.CurrentCallDetails.isSurveyRegistered) {
                        callUI.doSkipSurvey();
                    } else {
                        callUI.doSurvey();
                    }
                }
            };

            btnSMECall.Click += (sender, args) => callUI.doStartSMECall();
            btnSurvey.Click += (sender, args) => callUI.doSurvey();
            btnSkipSurvey.Click += (sender, args) => callUI.doSkipSurvey();
            btnAddNote.Click += (sender, args) => callUI.doAddNote();
            btnMAECall.Click += (sender, args) => callUI.doMAECall();

            enableSurvey(false);
            /* 
             * clockTimer.Elapsed += async (sender, e) => await doClockUpdate();
            clockTimer.Elapsed += (sender, args) => { 
            };
            clockTimer.AutoReset = true;
            clockTimer.Start();
            */
        }

        public void setCallMode(CallRecordCore.CurrentCallDetails.CallMode mode) {
            if (Dispatcher.UIThread.CheckAccess()) {
                _setCallMode(mode);
            } else {
                Dispatcher.UIThread.Invoke(() => _setCallMode(mode));
            }
        }
        private void _setCallMode(CallRecordCore.CurrentCallDetails.CallMode mode) {
            switch (mode) {
                case CurrentCallDetails.CallMode.Disconnect:    
                    btnStartCall.Content = lblStartCall;
                    enableSurvey(false);
                    btnEndCall.IsEnabled = false;
                    btnSMECall.IsEnabled = false;
                    btnMAECall.IsEnabled = false;
                    fIsInbound.Text = lblWaiting;
                    fIsInCall.Background = Avalonia.Media.Brushes.AliceBlue;
                    fIsInWrap.Background = Avalonia.Media.Brushes.AliceBlue;
                    break;

                case CurrentCallDetails.CallMode.InCall:        
                    btnStartCall.Content = lblStartWrap;
                    enableSurvey(true);
                    btnEndCall.IsEnabled = true;
                    btnSMECall.IsEnabled = true;
                    btnMAECall.IsEnabled = true;
                    fIsInbound.Text = lblInboundCall;
                    fIsInCall.Background = Avalonia.Media.Brushes.ForestGreen;
                    fIsInWrap.Background = Avalonia.Media.Brushes.AliceBlue;
                    break;

                case CurrentCallDetails.CallMode.InWrap:        
                    btnStartCall.Content = lblStartCallback;
                    enableSurvey(true);
                    btnStartCall.IsEnabled = true;
                    btnEndCall.IsEnabled = true;
                    btnSMECall.IsEnabled = true;
                    btnMAECall.IsEnabled = true;
                    fIsInbound.Text = lblInboundCall;
                    fIsInCall.Background = Avalonia.Media.Brushes.ForestGreen;
                    fIsInWrap.Background = Avalonia.Media.Brushes.ForestGreen;
                    break;
            }
        }

        public void enableSurvey(bool enable) {
            if (Dispatcher.UIThread.CheckAccess()) {
                btnSurvey.IsEnabled = enable;
                btnSkipSurvey.IsEnabled = enable;
            } else {
                Dispatcher.UIThread.Invoke(() => {  btnSurvey.IsEnabled = enable; btnSkipSurvey.IsEnabled = enable; });
            }
        }

        public void appendNote(string note) {
            if (Dispatcher.UIThread.CheckAccess()) {
                _appendNote(note);
            } else {
                Dispatcher.UIThread.Invoke(() => { _appendNote(note); });
            }
        }
        private void _appendNote(string note) {
            string? notes = txtNotes.Text;
            if (string.IsNullOrEmpty(notes)) {
                txtNotes.Text = note;
            } else {
                txtNotes.Text += "\n" + note;
            }
        }

        public void prependNote(string note) {
            if (Dispatcher.UIThread.CheckAccess()) {
                _prependNote(note);
            } else {
                Dispatcher.UIThread.Invoke(() => { _prependNote(note); });
            }
        }
        private void _prependNote(string note) {
            string? notes = txtNotes.Text;
            if (string.IsNullOrEmpty(notes)) {
                txtNotes.Text = note;
            } else {
                txtNotes.Text = note + "\n" + notes;
            }
        }

        public string getNotes() {
            if (Dispatcher.UIThread.CheckAccess()) {
                return txtNotes.Text ?? string.Empty;
            } else {
                return Dispatcher.UIThread.Invoke(() => txtNotes.Text ?? string.Empty);
            }
        }

        public void clearNotes() {
            if (Dispatcher.UIThread.CheckAccess()) {
                txtNotes.Text = string.Empty;
            } else {
                Dispatcher.UIThread.Invoke(() =>  txtNotes.Text = string.Empty);
            }
        }

        public async Task<SkipSurvey.SkipSurveyResult> doSkipSurvey() {
            if (Dispatcher.UIThread.CheckAccess()) {
                return await _doSkipSurvey();
            } else {
                return await Dispatcher.UIThread.Invoke(async () => await _doSkipSurvey());
            }
        }
        private async Task<SkipSurvey.SkipSurveyResult> _doSkipSurvey() {
            dialogs.SkipSurvey dlgSkipSurvey = new dialogs.SkipSurvey();
            dlgSkipSurvey.Width = 350;
            dlgSkipSurvey.Height = 400;
            await dlgSkipSurvey.ShowDialog(this);
            return dlgSkipSurvey.Result;
        }

        public async Task<TransferRequest.TransferRequestResult> doTransferRequest() {
            if (Dispatcher.UIThread.CheckAccess()) { 
                return await _doTransferRequest();
            } else {
                return await Dispatcher.UIThread.Invoke(async () => await _doTransferRequest());
            }
        }
        private async Task<TransferRequest.TransferRequestResult> _doTransferRequest() {
            dialogs.TransferRequest dlgTransferRequest = new dialogs.TransferRequest(callUI);
            dlgTransferRequest.DataContext = callUI;
            dlgTransferRequest.Width = 350;
            dlgTransferRequest.Height = 520;
            await dlgTransferRequest.ShowDialog(this);
            return dlgTransferRequest.Result;
        }

        public async Task<SMERequest.SMERequestResult> doSMERequest() {
            if (Dispatcher.UIThread.CheckAccess()) {
                return await _doSMERequest();
            } else {
                return await Dispatcher.UIThread.Invoke(async () => await _doSMERequest());
            }
        }
        private async Task<SMERequest.SMERequestResult> _doSMERequest() {
            dialogs.SMERequest dlgSMERequest = new dialogs.SMERequest();
            dlgSMERequest.DataContext = callUI;
            dlgSMERequest.Width = 400;
            dlgSMERequest.Height = 550;
            await dlgSMERequest.ShowDialog(this);
            return dlgSMERequest.Result;
        }

        public async Task<MultipleTransfer.MultipleTransferResult> doMAERequest() {
            if (Dispatcher.UIThread.CheckAccess()) {
                return await _doMAERequest();
            } else {
                return await Dispatcher.UIThread.Invoke(async () => await _doMAERequest());
            }
        }
        private async Task<MultipleTransfer.MultipleTransferResult> _doMAERequest() {
            dialogs.MultipleTransfer dlgMAERequest = new dialogs.MultipleTransfer(callUI);
            dlgMAERequest.Width = 350;
            dlgMAERequest.Height = 450;
            await dlgMAERequest.ShowDialog(this);
            return dlgMAERequest.Result;
        }

        public async Task<OutboundCall.OutboundCallResult> doOutboundCall() {
            if (Dispatcher.UIThread.CheckAccess()) {
                return await _doOutboundCall();
            } else {
                return await Dispatcher.UIThread.Invoke(async () => await _doOutboundCall());
            }
        }
        private async Task<OutboundCall.OutboundCallResult> _doOutboundCall() {
            dialogs.OutboundCall dlgOutboundCall = new dialogs.OutboundCall();
            dlgOutboundCall.Width = 350;
            dlgOutboundCall.Height = 300;
            await dlgOutboundCall.ShowDialog(this);
            return dlgOutboundCall.Result;
        }

        public async Task<CallNotes.CallNotesResult> doCallNotes() {
            if (Dispatcher.UIThread.CheckAccess()) {
                return await _doCallNotes();
            } else {
                return await Dispatcher.UIThread.Invoke(async () => await _doCallNotes());
            }
        }
        private async Task<CallNotes.CallNotesResult> _doCallNotes() {
            dialogs.CallNotes dlgCallNotes = new dialogs.CallNotes(callUI);
            dlgCallNotes.Width = 350;
            dlgCallNotes.Height = 700;
            await dlgCallNotes.ShowDialog(this);
            return dlgCallNotes.Result;
        }
    }
}