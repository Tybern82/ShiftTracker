using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CallRecordGUI.dialogs;
using com.tybern.CallRecordCore;
using com.tybern.CallRecordCore.commands;
using Newtonsoft.Json.Linq;
using static com.tybern.CallRecordCore.UICallbacks;

namespace CallRecordGUI {
    public class EnumConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            com.tybern.CallRecordCore.dialogs.CallNotesResult.CallType option = (com.tybern.CallRecordCore.dialogs.CallNotesResult.CallType)value;
            string _result = com.tybern.CallRecordCore.dialogs.CallNotesResult.GetText(option);
            return _result;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }

    public partial class MainWindow : Window, UICallbacks {
        
        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private static string ConfigFile = "CallRecordGUI.config";

        public MainWindow() {
            InitializeComponent();
            cmbCallType.ItemsSource = ItemCollection.GetOrCreate<com.tybern.CallRecordCore.dialogs.CallNotesResult.CallType>(Enum.GetValues(typeof(com.tybern.CallRecordCore.dialogs.CallNotesResult.CallType)).Cast<com.tybern.CallRecordCore.dialogs.CallNotesResult.CallType>());

            CallRecordCore.Instance.UICallbacks = this; // link this instance to the UICallbacks
            DataContext = CallRecordCore.Instance.UIProperties;

            FileStream configFile = File.Open(ConfigFile, FileMode.OpenOrCreate);
            StreamReader configReader = new StreamReader(configFile);
            string configJSON = configReader.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(configJSON)) {
                JObject configData = JObject.Parse(configJSON);
                decodeJSON(configData);
            }
            CallRecordCore.Instance.Messages.Enqueue(new CLoadDB());    // trigger loading existing call records from today

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

            cmbCallType.SelectionChanged += (sender, args) =>
            {
                if (cmbCallType.SelectedItem != null)
                {
                    CallRecordCore.Instance.UIProperties.CurrentCallType = (com.tybern.CallRecordCore.dialogs.CallNotesResult.CallType)cmbCallType.SelectedItem;
                }
            };

            spinSMTPPort.Spin += (sender, args) => {
                SpinEventArgs spinArgs = args as SpinEventArgs;
                switch (spinArgs.Direction) {
                    case SpinDirection.Increase:
                        CallRecordCore.Instance.UIProperties.SMTPPort++;
                        break;

                    case SpinDirection.Decrease:
                        CallRecordCore.Instance.UIProperties.SMTPPort--;
                        break;
                }
                if (CallRecordCore.Instance.UIProperties.SMTPPort == 1) spinSMTPPort.ValidSpinDirection = ValidSpinDirections.Increase;
                else if (CallRecordCore.Instance.UIProperties.SMTPPort == 65535) spinSMTPPort.ValidSpinDirection = ValidSpinDirections.Decrease;
                else spinSMTPPort.ValidSpinDirection = ValidSpinDirections.Increase | ValidSpinDirections.Decrease;
            };

            btnSMECall.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CSMERequest());
            btnSurvey.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CSurvey());
            btnSkipSurvey.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CSkipSurvey());
            btnAddNote.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CCallNotes());
            btnMAECall.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CMAERequest());
            btnStartShift.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CShiftStart());
            btnStartBreak.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CBreakStart());
            btnEndBreak.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CBreakEnd());
            btnEndShift.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CEndOfShift());

            btnIsANGenerated.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CGenerateAutoNotes());
            btnIsANEdited.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CEditAutoNotes());
            btnIsANSaved.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CSaveAutoNotes());
            btnIsANManualSave.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CManualNotes());

            btnSetFile.Click += async (sender, args) => {
                // Get top level from the current control. Alternatively, you can use Window reference instead.
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel != null) {
                    // Start async operation to open the dialog.
                    var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
                        Title = "Save Database File"
                    });

                    if (file is not null) {
                        CallRecordCore.Instance.UIProperties.DBFile = file.Path.LocalPath;
                    }
                }
            };
            btnUpdateBreaks.Click += (sender, args) => CallRecordCore.Instance.Messages.Enqueue(new CCreateBreaks());

            CallRecordCore.Instance.CurrentCall.CurrentMode = CallDetails.CallMode.Disconnect;  // should set button states correctly

            this.Closing += (sender, args) => {
                CallRecordCore.Instance.Messages.Terminate();     // make sure to terminate the processing thread when the main window closes
                FileStream configFile = File.Open(ConfigFile, FileMode.Create);
                StreamWriter configWriter = new StreamWriter(configFile);
                configWriter.Write(encodeJSON().ToString());
                configWriter.Flush();
                configWriter.Close();
            };
        }

        private void setContent(ComboBoxItem rControl, com.tybern.CallRecordCore.dialogs.CallNotesResult.CallType option)
        {
            rControl.Content = com.tybern.CallRecordCore.dialogs.CallNotesResult.GetText(option);
            ToolTip.SetTip(rControl, com.tybern.CallRecordCore.dialogs.CallNotesResult.GetToolTip(option));
        }

        private void FSMTPPort_TextChanged(object? sender, TextChangedEventArgs e) {
            try {
                if (sender == null) return;
                TextBox fSMTPPort = (TextBox)sender;
                if (fSMTPPort.Text != null) {
                    int val = int.Parse(fSMTPPort.Text);
                    CallRecordCore.Instance.UIProperties.SMTPPort = val;
                }
            } catch (Exception) { }
        }

        private void decodeJSON(JObject configData) {
            int posx = configData.ContainsKey("PosX") ? configData.Value<int>("PosX") : this.Position.X;
            int posy = configData.ContainsKey("PosY") ? configData.Value<int>("PosY") : this.Position.Y;
            double width = configData.ContainsKey("Width") ? configData.Value<double>("Width") : this.Width;
            double height = configData.ContainsKey("Height") ? configData.Value<double>("Height") : this.Height;

            this.Position = new Avalonia.PixelPoint(posx, posy);
            this.Width = width;
            this.Height = height;

            string? _DBFile = configData.ContainsKey("DBFile") ? configData.Value<string>("DBFile") : "CallRecordGUI.db";
            if (_DBFile != null) CallRecordCore.Instance.UIProperties.DBFile = _DBFile;

            if (configData.ContainsKey("SMTP")) {
                JObject? smtpDetails = configData.Value<JObject>("SMTP");
                if (smtpDetails != null) {
                    CallRecordCore.Instance.UIProperties.SMTPHost = smtpDetails.Value<string>("Host") ?? CallRecordCore.Instance.UIProperties.SMTPHost;
                    CallRecordCore.Instance.UIProperties.SMTPPort = smtpDetails.Value<int>("Port");
                    CallRecordCore.Instance.UIProperties.SMTPUsername = smtpDetails.Value<string>("Username") ?? CallRecordCore.Instance.UIProperties.SMTPUsername;
                    CallRecordCore.Instance.UIProperties.SMTPPassword = smtpDetails.Value<string>("Password") ?? CallRecordCore.Instance.UIProperties.SMTPPassword;
                    CallRecordCore.Instance.UIProperties.SenderAddress = smtpDetails.Value<string>("FromAddress") ?? CallRecordCore.Instance.UIProperties.SenderAddress;
                    CallRecordCore.Instance.UIProperties.DestinationAddress = smtpDetails.Value<string>("ToAddress") ?? string.Empty;
                }
            }

            if (configData.ContainsKey("Breaks")) {
                JObject? breakDetails = configData.Value<JObject>("Breaks");
                if (breakDetails != null) {
                    if (breakDetails.ContainsKey("ShiftStart")) CallRecordCore.Instance.UIProperties.BreakTimes.ShiftStart = TimeSpan.FromTicks(breakDetails.Value<long>("ShiftStart"));
                    if (breakDetails.ContainsKey("ShiftEnd")) CallRecordCore.Instance.UIProperties.BreakTimes.ShiftEnd = TimeSpan.FromTicks(breakDetails.Value<long>("ShiftEnd"));
                    if (breakDetails.ContainsKey("FirstBreak")) CallRecordCore.Instance.UIProperties.BreakTimes.FirstBreak = TimeSpan.FromTicks(breakDetails.Value<long>("FirstBreak"));
                    if (breakDetails.ContainsKey("LunchBreak")) CallRecordCore.Instance.UIProperties.BreakTimes.LunchBreak = TimeSpan.FromTicks(breakDetails.Value<long>("LunchBreak"));
                    if (breakDetails.ContainsKey("LastBreak")) CallRecordCore.Instance.UIProperties.BreakTimes.LastBreak = TimeSpan.FromTicks(breakDetails.Value<long>("LastBreak"));
                    if (breakDetails.ContainsKey("MeetingTime")) CallRecordCore.Instance.UIProperties.BreakTimes.MeetingBreak = TimeSpan.FromTicks(breakDetails.Value<long>("MeetingTime"));
                    if (breakDetails.ContainsKey("TrainingTime")) CallRecordCore.Instance.UIProperties.BreakTimes.TrainingBreak = TimeSpan.FromTicks(breakDetails.Value<long>("TrainingTime"));
                }
            }
            CallRecordCore.Instance.BreakTimesDB.LoadBreakTimes(DateTime.Now.Date, CallRecordCore.Instance.UIProperties.BreakTimes);
        }

        private JObject encodeJSON() {
            JObject _result = new JObject();
            _result.Add("PosX", this.Position.X);
            _result.Add("PosY", this.Position.Y);
            _result.Add("Width", this.Width);
            _result.Add("Height", this.Height);

            _result.Add("DBFile", CallRecordCore.Instance.UIProperties.DBFile);

            JObject smtpDetails = new JObject();
            smtpDetails.Add("Host", CallRecordCore.Instance.UIProperties.SMTPHost);
            smtpDetails.Add("Port", CallRecordCore.Instance.UIProperties.SMTPPort);
            smtpDetails.Add("Username", CallRecordCore.Instance.UIProperties.SMTPUsername);
            smtpDetails.Add("Password", CallRecordCore.Instance.UIProperties.SMTPPassword);
            smtpDetails.Add("FromAddress", CallRecordCore.Instance.UIProperties.SenderAddress);
            smtpDetails.Add("ToAddress", CallRecordCore.Instance.UIProperties.DestinationAddress);
            _result.Add("SMTP", smtpDetails);

            JObject breakDetails = new JObject();
            breakDetails.Add("ShiftStart", CallRecordCore.Instance.UIProperties.BreakTimes.ShiftStart.Ticks);
            breakDetails.Add("ShiftEnd", CallRecordCore.Instance.UIProperties.BreakTimes.ShiftEnd.Ticks);
            breakDetails.Add("FirstBreak", CallRecordCore.Instance.UIProperties.BreakTimes.FirstBreak.Ticks);
            breakDetails.Add("LunchBreak", CallRecordCore.Instance.UIProperties.BreakTimes.LunchBreak.Ticks);
            breakDetails.Add("LastBreak", CallRecordCore.Instance.UIProperties.BreakTimes.LastBreak.Ticks);
            breakDetails.Add("MeetingTime", CallRecordCore.Instance.UIProperties.BreakTimes.MeetingBreak.Ticks);
            breakDetails.Add("TrainingTime", CallRecordCore.Instance.UIProperties.BreakTimes.TrainingBreak.Ticks);
            _result.Add("Breaks", breakDetails);

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

                case CallRecordDialog.BreakTimesDialog:
                    BreakTimes dlgBreakTimes = new BreakTimes();
                    dlgBreakTimes.ShowDialog(this);
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

                case UITriggerType.StartShiftButton:
                    btnStartShift.IsEnabled = enabled;
                    break;

                case UITriggerType.EndShiftButton:
                    btnEndShift.IsEnabled = enabled;
                    break;

                case UITriggerType.StartBreakButton:
                    btnStartBreak.IsEnabled = enabled;
                    break;

                case UITriggerType.EndBreakButton:
                    btnEndBreak.IsEnabled = enabled;
                    break;

                case UITriggerType.ANGeneratedButton:
                    btnIsANGenerated.IsEnabled = enabled;
                    break;

                case UITriggerType.ANEditedButton:
                    btnIsANEdited.IsEnabled = enabled;
                    break;

                case UITriggerType.ANSavedButton:
                    btnIsANSaved.IsEnabled = enabled;
                    break;

                case UITriggerType.ANManualButton:
                    btnIsANManualSave.IsEnabled = enabled;
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