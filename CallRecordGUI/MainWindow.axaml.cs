using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CallRecordGUI.dialogs;
using com.tybern.CallRecordCore;
using com.tybern.CallRecordCore.commands;
using Newtonsoft.Json.Linq;
using static com.tybern.CallRecordCore.UICallbacks;

namespace CallRecordGUI {
    public partial class MainWindow : Window, UICallbacks {

        private static string ConfigFile = "CallRecordGUI.config";

        public MainWindow() {
            InitializeComponent();

            FileStream configFile = File.Open(ConfigFile, FileMode.OpenOrCreate);
            StreamReader configReader = new StreamReader(configFile);
            string configJSON = configReader.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(configJSON)) {
                JObject configData = JObject.Parse(configJSON);
                decodeJSON(configData);
            }

            CallRecordCore.Instance.UICallbacks = this; // link this instance to the UICallbacks
            DataContext = CallRecordCore.Instance.UIProperties;

            btnStartCall.Click += (sender, args) => {
                if (CallRecordCore.Instance.CurrentCall.CurrentMode == CallDetails.CallMode.InCall) {
                    CallRecordCore.Instance.Messages.Enqueue(new CStartWrap());
                } else {
                    CallRecordCore.Instance.Messages.Enqueue(new CStartCall());
                }
            };

            btnEndCall.Click += (sender, args) => {
                if (CallRecordCore.Instance.CurrentCall.CurrentMode != CallDetails.CallMode.Disconnect) {
                    if (!CallRecordCore.Instance.CurrentCall.IsInWrap) CallRecordCore.Instance.Messages.Enqueue(new CStartWrap());
                    if (!CallRecordCore.Instance.CurrentCall.IsSurveyRecorded) CallRecordCore.Instance.Messages.Enqueue(new CSkipSurvey());
                    CallRecordCore.Instance.Messages.Enqueue(new CStopCall());
                }
            };

            btnSMECall.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CSMERequest());
            btnSurvey.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CSurvey());
            btnSkipSurvey.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CSkipSurvey());
            btnAddNote.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CCallNotes());
            btnMAECall.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CMAERequest());
            btnEOS.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CEndOfShift());

            CallRecordCore.Instance.CurrentCall.CurrentMode = CallDetails.CallMode.Disconnect;  // should set button states correctly

            this.Closing += (sender, args) => {
                CallRecordCore.Instance.Messages.Terminate();     // make sure to terminate the processing thread when the main window closes
                FileStream configFile = File.OpenWrite(ConfigFile);
                StreamWriter configWriter = new StreamWriter(configFile);
                configWriter.Write(encodeJSON().ToString());
                configWriter.Flush();
                configWriter.Close();
            };
        }

        private void decodeJSON(JObject configData) {
            int posx = configData.ContainsKey("PosX") ? configData.Value<int>("PosX") : this.Position.X;
            int posy = configData.ContainsKey("PosY") ? configData.Value<int>("PosY") : this.Position.Y;
            double width = configData.ContainsKey("Width") ? configData.Value<double>("Width") : this.Width;
            double height = configData.ContainsKey("Height") ? configData.Value<double>("Height") : this.Height;

            this.Position = new Avalonia.PixelPoint(posx, posy);
            this.Width = width;
            this.Height = height;
        }

        private JObject encodeJSON() {
            JObject _result = new JObject();
            _result.Add("PosX", this.Position.X);
            _result.Add("PosY", this.Position.Y);
            _result.Add("Width", this.Width);
            _result.Add("Height", this.Height);
            return _result;
        }

        public void ShowDialog(CallRecordDialog dlgType) {
            if (Dispatcher.UIThread.CheckAccess()) {
                _showDialog(dlgType);
            } else {
                Dispatcher.UIThread.Invoke(() => _showDialog(dlgType));
            }
        }

        private void _showDialog(CallRecordDialog dlgType) {
            switch (dlgType) {
                case CallRecordDialog.SkipSurveyDialog:
                    SkipSurvey dlgSkipSurvey = new();
                    dlgSkipSurvey.ShowDialog(this);
                    break;

                case CallRecordDialog.TransferRequestDialog:
                    TransferRequest dlgTransferRequest = new TransferRequest();
                    dlgTransferRequest.ShowDialog(this);
                    break;

                case CallRecordDialog.SMERequestDialog:
                    SMERequest dlgSMERequest = new SMERequest();
                    dlgSMERequest.ShowDialog(this);
                    break;

                case CallRecordDialog.MAERequestDialog:
                    MultipleTransfer dlgMAERequest = new MultipleTransfer();
                    dlgMAERequest.ShowDialog(this);
                    break;

                case CallRecordDialog.OutboundCallDialog:
                    OutboundCall dlgOutboundCall = new OutboundCall();
                    dlgOutboundCall.ShowDialog(this);
                    break;

                case CallRecordDialog.CallNotesDialog:
                    CallNotes dlgNotes = new CallNotes();
                    dlgNotes.ShowDialog(this);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public void EnableButton(UITriggerType btnType) { if (Dispatcher.UIThread.CheckAccess()) setButtons(btnType, true); else Dispatcher.UIThread.Invoke(() => setButtons(btnType, true)); }
        public void DisableButton(UITriggerType btnType) { if (Dispatcher.UIThread.CheckAccess()) setButtons(btnType, false); else Dispatcher.UIThread.Invoke(() => setButtons(btnType, false)); }
        public void SetCallMode(CallDetails.CallMode mode) { if (Dispatcher.UIThread.CheckAccess()) setCallMode(mode); else Dispatcher.UIThread.Invoke(() => setCallMode(mode)); }

        public void SetClipboard(string? text) { if (Dispatcher.UIThread.CheckAccess()) Clipboard?.SetTextAsync(text); else Dispatcher.UIThread.Invoke(() => Clipboard?.SetTextAsync(text)); }

        private void setButtons(UITriggerType btnType, bool enabled) {
            switch (btnType) {
                case UITriggerType.SurveyButtons:
                    btnSurvey.IsEnabled = enabled;
                    btnSkipSurvey.IsEnabled = enabled;
                    break;

                case UITriggerType.StartCallButton:
                    btnStartCall.IsEnabled = enabled;
                    break;

                case UITriggerType.EndCallButton:
                    btnEndCall.IsEnabled = enabled;
                    break;

                case UITriggerType.SMECallButton:
                    btnSMECall.IsEnabled = enabled;
                    break;

                case UITriggerType.MAECallButton:
                    btnMAECall.IsEnabled = enabled;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void setCallMode(CallDetails.CallMode mode) {
            switch (mode) {
                case CallDetails.CallMode.Disconnect:
                    fIsInCall.Background = Avalonia.Media.Brushes.AliceBlue;
                    fIsInWrap.Background = Avalonia.Media.Brushes.AliceBlue;
                    break;

                case CallDetails.CallMode.InCall:    
                    fIsInCall.Background = Avalonia.Media.Brushes.ForestGreen;
                    fIsInWrap.Background = Avalonia.Media.Brushes.AliceBlue;
                    break;

                case CallDetails.CallMode.InWrap:    
                    fIsInCall.Background = Avalonia.Media.Brushes.ForestGreen;
                    fIsInWrap.Background = Avalonia.Media.Brushes.ForestGreen;
                    break;
            }
        }
    }
}