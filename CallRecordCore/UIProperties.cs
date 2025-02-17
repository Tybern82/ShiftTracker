using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace com.tybern.CallRecordCore {

    /// <summary>
    /// Contains all the UI/GUI properties. UI elements will link to values on this class, which can then be 
    /// updated by application code to change the UI values.
    /// </summary>
    public class UIProperties : INotifyPropertyChanged {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Trigger PropertyChanged event when values change. This method is called when any UI/GUI exposed property is updated
        /// to trigger the notification to the UI of the change.
        /// </summary>
        /// <remarks>This method should be called outside any thread lock (ie release lock before calling this method).</remarks>
        /// <param name="propertyName">name of the property which has been modified</param>
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<CallRecord> CallRecordsList { get; } = new ObservableCollection<CallRecord>();
        public ObservableCollection<SurveyRecord> SurveyRecordList { get; } = new ObservableCollection<SurveyRecord>();

        private static string NULL_TIME = "00:00:00";

        public static string LabelStartCall { get; } = "Start Call";
        public static string LabelStartWrap { get; } = "Start Wrap";
        public static string LabelStartCallback { get; } = "Callback";
        private string _startCallButtonText = LabelStartCall;
        public string StartCallButtonText {
            get { lock (_startCallButtonText) { return _startCallButtonText; } }
            set {
                lock (_startCallButtonText) { _startCallButtonText = value; }
                OnPropertyChanged(nameof(StartCallButtonText));
            }
        }

        public static string LabelInboundCall { get; } = "Inbound Call";
        public static string LabelWaiting { get; } = "Waiting";
        private string _isInboundText = LabelWaiting;
        public string IsInboundText {
            get { lock (_isInboundText) { return _isInboundText; } }
            set {
                lock (_isInboundText) { _isInboundText = value; }
                OnPropertyChanged(nameof(IsInboundText));
            }
        }

        private string _notes = "";
        public string Notes {
            get { lock (_notes) { return _notes; } }
            set {
                lock (_notes) { _notes = value; }
                OnPropertyChanged(nameof(Notes));
            }
        }

        private DateTime _currentTime = DateTime.Now;
        public DateTime CurrentTime {
            get { lock (this) return _currentTime; }
            set {
                lock (this) { _currentTime = value; }
                OnPropertyChanged(nameof(CurrentTime));
            }
        }

        private TimeSpan _transferTime = TimeSpan.Zero;
        public TimeSpan TransferTime {
            get { lock (this) return _transferTime; }
            set {
                lock (this) { _transferTime = value; }
                OnPropertyChanged(nameof(TransferTime));
            }
        }

        private int _totalMAE = 0;
        public int TotalMAE {
            get { lock (this) { return _totalMAE; } }
            set {
                lock (this) { _totalMAE = value; }
                OnPropertyChanged(nameof(TotalMAE));
            }
        }

        private int _totalCalls = 0;
        public int TotalCalls {
            get { lock (this) { return _totalCalls; } }
            set {
                lock (this) { _totalCalls = value; }
                OnPropertyChanged(nameof(TotalCalls));
            }
        }

        private int _callSME = 0;
        public int CallSME {
            get { lock (this) { return _callSME; } }
            set {
                lock (this) { _callSME = value; }
                OnPropertyChanged(nameof(CallSME));
            }
        }

        private TimeSpan _smeTime = TimeSpan.Zero;
        public TimeSpan SMETime {
            get { lock (this) { return _smeTime; } }
            set {
                lock (this) { _smeTime = value; }
                OnPropertyChanged(nameof(SMETime));
            }
        }

        private int _callMAE = 0;
        public int CallMAE {
            get { lock (this) { return _callMAE; } }
            set {
                lock (this) { _callMAE = value; }
                OnPropertyChanged(nameof(CallMAE));
            }
        }

        private TimeSpan _breakTimer = TimeSpan.Zero;
        public TimeSpan BreakTimer {
            get { lock (this) { return _breakTimer; } }
            set {
                if (_breakTimer != value) {
                    lock (this) { _breakTimer = value; }
                    OnPropertyChanged(nameof(BreakTimer));
                }
            }
        }

        private string _breakTimerText = NULL_TIME;
        public string BreakTimerText {
            get { lock (_breakTimerText) { return _breakTimerText; } }
            set {
                lock (_breakTimerText) { _breakTimerText = value; }
                OnPropertyChanged(nameof(BreakTimerText));
            }
        }

        private string _currBreakText = string.Empty;
        public string CurrentBreakText {
            get { lock (_currBreakText) { return _currBreakText; } }
            set {
                lock (_currBreakText) { _currBreakText = value; }
                OnPropertyChanged(nameof(CurrentBreakText));
            }
        }

        private string _eosTimerText = NULL_TIME;
        public string EOSTimerText {
            get { lock (_eosTimerText) { return _eosTimerText; } }
            set {
                lock (_eosTimerText) { _eosTimerText = value; }
                OnPropertyChanged(nameof(EOSTimerText));
            }
        }

        private TimeSpan _callTime = TimeSpan.Zero;
        public TimeSpan CallTime {
            get { lock (this) { return _callTime; } }
            set {
                lock (this) { _callTime = value; }
                OnPropertyChanged(nameof(CallTime));
            }
        }

        private TimeSpan _totalWrap = TimeSpan.Zero;
        public TimeSpan TotalWrap {
            get { lock (this) { return _totalWrap; } }
            set {
                lock (this) { _totalWrap = value; WrapPercent = (_totalWrap.Ticks / (double)CallRecordCore.Instance.ShiftCounter.TotalDuration.Ticks); }
                OnPropertyChanged(nameof(TotalWrap));
            }
        }

        private double _wrapPercent = 0.0;
        public double WrapPercent {
            get { lock (this) { return _wrapPercent; } }
            private set {
                lock (this) { _wrapPercent = value; }
                OnPropertyChanged(nameof(WrapPercent));
            }
        }

        private string _destinationAddress = string.Empty;
        public string DestinationAddress {
            get { lock (_destinationAddress) { return _destinationAddress; } }
            set {
                lock (_destinationAddress) { _destinationAddress = value; }
                OnPropertyChanged(nameof(DestinationAddress));
            }
        }

        private string _senderAddress = string.Empty;
        public string SenderAddress {
            get { lock (_senderAddress) { return _senderAddress; } }
            set {
                lock (_senderAddress) { _senderAddress = value; }
                OnPropertyChanged(nameof(SenderAddress));
            }
        }

        private string _smtpHost = string.Empty;
        public string SMTPHost {
            get { lock (_smtpHost) { return _smtpHost; } }
            set {
                lock (_smtpHost) { _smtpHost = value; }
                OnPropertyChanged(nameof(SMTPHost));
            }
        }

        private int _smtpPort = 465;
        public int SMTPPort {
            get { lock (this) { return _smtpPort; } }
            set {
                int val = value;
                if (val < 1) val = 1;
                if (val > 65535) val = 65535;
                lock (this) { _smtpPort = val; }
                OnPropertyChanged(nameof(SMTPPort));
            }
        }

        private string _smtpUsername = string.Empty;
        public string SMTPUsername {
            get { lock (_smtpUsername) { return _smtpUsername; } }
            set {
                lock (_smtpUsername) { _smtpUsername = value; }
                OnPropertyChanged(nameof(SMTPUsername));
            }
        }

        private string _smtpPassword = string.Empty;
        public string SMTPPassword {
            get { lock (_smtpPassword) { return _smtpPassword; } }
            set {
                lock (_smtpPassword) { _smtpPassword = value; }
                OnPropertyChanged(nameof(SMTPPassword));
            }
        }

        public BreakTimes BreakTimes { get; } = new BreakTimes(true);



        private string _DBFile = "CallRecordGUI.db";
        public string DBFile {
            get { lock (_DBFile) { return _DBFile; } }
            set {
                lock (_DBFile) { _DBFile = value; }
                OnPropertyChanged(nameof(DBFile));
            }
        }

        private DateTimeOffset _SelectedDate = DateTime.Now.Date;
        public DateTimeOffset SelectedDate {
            get { lock (this) { return _SelectedDate; } }
            set {
                lock (this) {
                    LOG.Info("Updating SelectedDate");
                    if (_SelectedDate != value) {
                        LOG.Info("Date Changed");
                        _SelectedDate = value;
                        CallRecordCore.Instance.BreakTimesDB.LoadBreakTimes(_SelectedDate.Date, SelectedBreakTimes);
                    }
                }
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        public BreakTimes SelectedBreakTimes { get; } = new BreakTimes();
    }
}
