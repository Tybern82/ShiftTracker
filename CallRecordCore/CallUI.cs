using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CallRecordCore {
    public class CallUI : INotifyPropertyChanged {

        private UICallbacks UICallbacks { get; }

        public CallUI() : this(new DesignCallbacks()) { }

        public CallUI(UICallbacks callbacks) { 
            UICallbacks = callbacks; 
            CurrentCallDetails = new CurrentCallDetails(callbacks);
        }


        public ObservableCollection<CallRecord> CallRecordsList { get; } = new ObservableCollection<CallRecord>();

        private DateTime _CurrentTime = DateTime.Now;
        public DateTime CurrentTime {
            get { return _CurrentTime; }
            set {
                _CurrentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }

        private TimeSpan _BreakTimer = TimeSpan.Zero;
        public TimeSpan BreakTimer {
            get { return _BreakTimer; }
            set {
                _BreakTimer = value;
                OnPropertyChanged(nameof(BreakTimer));
            }
        }

        private string _breakTimerText = "BREAK";
        public string BreakTimerText {
            get { return _breakTimerText; }
            set {
                _breakTimerText = value;
                OnPropertyChanged(nameof(BreakTimerText));
            }
        }

        private TimeSpan _EOSTimer = TimeSpan.Zero;
        public TimeSpan EOSTimer {
            get { return _EOSTimer; }
            set {
                _EOSTimer = value;
                OnPropertyChanged(nameof(EOSTimer));
            }
        }

        private string _eosTimerText = "END-OF-SHIFT";
        public string EOSTimerText {
            get { return _eosTimerText; }
            set {
                _eosTimerText = value;
                OnPropertyChanged(nameof(EOSTimerText));
            }
        }

        private TimeSpan fCallTime = TimeSpan.Zero;
        public TimeSpan CallTime {
            get { return fCallTime; }
            set {
                fCallTime = value;
                OnPropertyChanged(nameof(CallTime));
            }
        }

        private TimeSpan fSMETime = TimeSpan.Zero;
        public TimeSpan SMETime {
            get { return fSMETime; }
            set {
                fSMETime = value;
                OnPropertyChanged(nameof(SMETime));
            }
        }

        private TimeSpan fTransferTime = TimeSpan.Zero;
        public TimeSpan TransferTime {
            get { return fTransferTime; }
            set {
                fTransferTime = value;
                OnPropertyChanged(nameof(TransferTime));
            }
        }

        private int fCallMAE = 0;
        public int CallMAE {
            get { return fCallMAE; }
            set {
                fCallMAE = value;
                OnPropertyChanged(nameof(CallMAE));
            }
        }

        private int fCallSME = 0;
        public int CallSME {
            get { return fCallSME; }
            set {
                fCallSME = value;
                OnPropertyChanged(nameof(CallSME));
            }
        }

        private int fTotalCalls = 0;
        public int TotalCalls {
            get { return fTotalCalls; }
            set {
                fTotalCalls = value;
                OnPropertyChanged(nameof(TotalCalls));
            }
        }

        private int fTotalMAE = 0;
        public int TotalMAE {
            get { return fTotalMAE; }
            set {
                fTotalMAE = value;
                OnPropertyChanged(nameof(TotalMAE));
            }
        }

        private TimeSpan fTotalWrap = TimeSpan.Zero;
        public TimeSpan TotalWrap {
            get { return fTotalWrap; }
            set {
                fTotalWrap = value;
                OnPropertyChanged(nameof(TotalWrap));
                WrapPercent = (fTotalWrap.Ticks / fTotalDuration.Ticks);
            }
        }

        private TimeSpan fTotalDuration = TimeSpan.FromSeconds(0);
        public TimeSpan TotalDuration {
            get { return fTotalDuration; }
            set {
                fTotalDuration = value;
                OnPropertyChanged(nameof(TotalDuration));
                WrapPercent = (fTotalWrap.Ticks / fTotalDuration.Ticks);
            }
        }

        private double fWrapPercent = 0.0;
        public double WrapPercent {
            get { return fWrapPercent; }
            set {
                fWrapPercent = value;
                OnPropertyChanged(nameof(WrapPercent));
            }
        }

        private const int longCallMinutes = 40;
        private const int highMAE = 0;

        private CallTypeCounter callTypeCounter = new CallTypeCounter();
        private ShiftCallCounter shiftCounter = new ShiftCallCounter();
        public CurrentCallDetails CurrentCallDetails { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void doClockUpdate(object? ui) {
            if (ui != null) {
                CallUI callUI = (CallUI) ui;
                DateTime currTime = DateTime.Now;
                callUI.CurrentTime = currTime;

                DateTime breakTime = fromCurrent(currTime, BreakTimer);
                BreakTimerText = (breakTime <= currTime) ? "BREAK" : toShortTimeString(breakTime - currTime);

                DateTime eosTime = fromCurrent(currTime, EOSTimer);
                EOSTimerText = (eosTime <= currTime) ? "END-OF-SHIFT" : ((EOSTimer == BreakTimer) ? "EOS" : toShortTimeString(eosTime - currTime));

                if (CurrentCallDetails.isInCall && (CurrentCallDetails.callStartTime != TimeSpan.Zero)) {
                    CallTime = currTime.TimeOfDay - CurrentCallDetails.callStartTime;
                } else {
                    CallTime = TimeSpan.Zero;
                }
                if (CurrentCallDetails.isInSMECall) SMETime = currTime.TimeOfDay - CurrentCallDetails.smeStartTime;
                if (CurrentCallDetails.isInTransferCall) TransferTime = currTime.TimeOfDay - CurrentCallDetails.transferStartTime; 
            }
        }

        public void doStartCall() {
            DateTime timeStamp = DateTime.Now;
            if (CurrentCallDetails.isInCall) {
                CurrentCallDetails.isCallback = true;
                doStopCall();
                UICallbacks.appendNote("Callback / Outbound");
                Task<OutboundCall.OutboundCallResult> uiResult = UICallbacks.doOutboundCall();
                new Thread(() => {
                    uiResult.Wait();
                    string reason = OutboundCall.getOutboundCallOptionText(uiResult.Result.Reason);
                    if (!string.IsNullOrWhiteSpace(uiResult.Result.Text)) reason += " - " + uiResult.Result.Text;
                    UICallbacks.appendNote(reason);
                });
            } else {
                CurrentCallDetails.isSurveyRegistered = false;
                CurrentCallDetails.isSurveyRecorded = false;
            }

            shiftCounter.TotalCalls++;
            CurrentCallDetails.callStartTime = timeStamp.TimeOfDay;
            CurrentCallDetails.wrapStartTime = timeStamp.TimeOfDay;
            CallMAE = 0;
            CallSME = 0;
            CurrentCallDetails.isInCall = true;
            CurrentCallDetails.isInWrap = false;

            CurrentCallDetails.CurrentMode = CurrentCallDetails.CallMode.InCall;    // Update the UI state to IN-CALL
        }

        public void doStartWrap() {
            DateTime timeStamp = DateTime.Now;
            if (!CurrentCallDetails.isInCall) doStartCall();

            CurrentCallDetails.wrapStartTime = timeStamp.TimeOfDay;
            CurrentCallDetails.isInWrap = true;

            CurrentCallDetails.CurrentMode = CurrentCallDetails.CallMode.InWrap;    // Update the UI state to IN-WRAP
        }

        public void doStopCall() {
            DateTime timeStamp = DateTime.Now;

            CurrentCallDetails.isInCall = false;
            CurrentCallDetails.isInWrap = false;

            if (CallSME > 0) UICallbacks.appendNote("SME Calls: " + CallSME);
            addCallRecord(timeStamp);
            CallMAE = 0;
            CallSME = 0;
            UICallbacks.clearNotes();
            CurrentCallDetails.isCallback = false;
            CurrentCallDetails.CurrentMode = CurrentCallDetails.CallMode.Disconnect;
        }

        public void doStartSMECall() {
            if (CurrentCallDetails.isInCall) {
                UICallbacks.appendNote("SME Outbound Call");
                shiftCounter.TotalSME++;
                CallSME++;
                CurrentCallDetails.isInSMECall = true;
                CurrentCallDetails.smeStartTime = DateTime.Now.TimeOfDay;
                Task<SMERequest.SMERequestResult> uiResult = UICallbacks.doSMERequest();
                new Thread(() => {
                    uiResult.Wait();
                    SMERequest.SMERequestResult result = uiResult.Result;

                    UICallbacks.appendNote(SMERequest.getSMERequestOptionText(result.Type));
                    if (!string.IsNullOrWhiteSpace(result.Text)) UICallbacks.appendNote(result.Text);

                    TimeSpan smeTime = (DateTime.Now.TimeOfDay - CurrentCallDetails.smeStartTime);
                    shiftCounter.TotalSMETime += smeTime;
                    CurrentCallDetails.isInSMECall = false;
                    SMETime = TimeSpan.Zero;
                    UICallbacks.appendNote(toShortTimeString(smeTime));
                }).Start();
            }
        }

        public void doAddNote() {
            Task<CallNotes.CallNotesResult> uiResult = UICallbacks.doCallNotes();
            Thread t = new Thread(() => {
                uiResult.Wait();
                DateTime currTime = DateTime.Now;
                CallRecord callRecord = new CallRecord(currTime, currTime, TimeSpan.Zero, TimeSpan.Zero, 0, uiResult.Result.CallType, uiResult.Result.Text);
                CallRecordsList.Add(callRecord);
            });
            t.Start();
        }

        public void doSkipSurvey() {
            if (!CurrentCallDetails.isSurveyRecorded) {
                CurrentCallDetails.isSurveyRecorded = true;
                UICallbacks.enableSurvey(false);
                Task<SkipSurvey.SkipSurveyResult> uiResult = UICallbacks.doSkipSurvey();
                // Push to separate thread to release the UIThread to allow interaction with popup
                Thread t = new Thread(() => { uiResult.Wait(); appendSurvey(uiResult.Result); });
                t.Start();
            }
        }

        public void doSurvey() {
            if (!CurrentCallDetails.isSurveyRecorded) {
                CurrentCallDetails.isSurveyRecorded = true;
                UICallbacks.enableSurvey(false);
                appendSurvey(new SkipSurvey.SkipSurveyResult(SkipSurvey.SkipSurveyOption.None, string.Empty));
            }
        }

        public void doMAECall() {
            CallMAE++;
            TotalMAE++;
            CurrentCallDetails.transferStartTime = DateTime.Now.TimeOfDay;
            CurrentCallDetails.isInTransferCall = true;
            if (CallMAE > 1) {
                new Thread(() => {
                    UICallbacks.doMAERequest().Wait();
                    UICallbacks.doTransferRequest().Wait();
                }).Start();
            } else {
                new Thread(() => UICallbacks.doTransferRequest().Wait()).Start();
            }
        }

        public void doSaveMAE(MultipleTransfer.MultipleTransferOption reason, string notes) {
            UICallbacks.appendNote(MultipleTransfer.getMultipleTransferOptionsText(reason));
            if (!string.IsNullOrWhiteSpace(notes)) UICallbacks.appendNote(notes);
        }

        public void doCloseTransfer(TransferRequest.TransferRequestOption destination, string notes) {
            UICallbacks.appendNote(TransferRequest.getTransferRequestOptionText(destination));
            if (!string.IsNullOrWhiteSpace(notes)) UICallbacks.appendNote(notes);
            TimeSpan transferTime = DateTime.Now.TimeOfDay - CurrentCallDetails.transferStartTime;
            shiftCounter.TotalTransferTime += transferTime;
            CurrentCallDetails.isInTransferCall = false;
            TransferTime = TimeSpan.Zero;
            UICallbacks.appendNote("Transfer Time: " + toShortTimeString(transferTime));
        }

        public void updateCallType(CallNotes.CallType type) {
            switch (type) {
                case CallNotes.CallType.Mobile:         callTypeCounter.Mobile++; break;
                case CallNotes.CallType.NBN:            callTypeCounter.NBN++; break;
                case CallNotes.CallType.ADSL:           callTypeCounter.ADSL++; break;
                case CallNotes.CallType.eMail:          callTypeCounter.eMail++; break;
                case CallNotes.CallType.Billing:        callTypeCounter.Billing++; break;
                case CallNotes.CallType.PA:             callTypeCounter.PriorityAssist++; break;
                case CallNotes.CallType.Prepaid:        callTypeCounter.Prepaid++; break;
                case CallNotes.CallType.PSTN:           callTypeCounter.PSTN++; break;
                case CallNotes.CallType.Opticomm:       callTypeCounter.Opticomm++; break;
                case CallNotes.CallType.FetchTV:        callTypeCounter.FetchTV++; break;
                case CallNotes.CallType.HomeWireless:   callTypeCounter.HomeWireless++; break;
                case CallNotes.CallType.Platinum:       callTypeCounter.Platinum++; break;
                case CallNotes.CallType.Misrouted:      callTypeCounter.Misrouted++; break;
                case CallNotes.CallType.Helpdesk:       callTypeCounter.Helpdesk++; break;

                default:                                callTypeCounter.Other++; break;
            }
            UICallbacks.prependNote(CallNotes.getCallNotesOptionText(type));
        }

        public void addCallRecord(DateTime endTime) {
            TimeSpan duration = CurrentCallDetails.wrapStartTime - CurrentCallDetails.callStartTime;
            TotalDuration += duration;

            TimeSpan wrap = endTime.TimeOfDay - CurrentCallDetails.wrapStartTime;
            TotalWrap += wrap;

            if (CurrentCallDetails.isCallback) {
                shiftCounter.TotalDropped++;
                UICallbacks.prependNote("Dropped / Callback");
            }
            string notes = UICallbacks.getNotes();

            CallRecord callRecord = new CallRecord(fromCurrent(endTime, CurrentCallDetails.callStartTime), endTime, duration, wrap, CallMAE);
            if (!CurrentCallDetails.isCallback) {
                Task<CallNotes.CallNotesResult> uiResult = UICallbacks.doCallNotes();
                Thread t = new Thread(() => {
                    uiResult.Wait();
                    callRecord.CallType = uiResult.Result.CallType;
                    callRecord.notes = notes + "\n" + uiResult.Result.Text;
                    CallRecordsList.Add(callRecord);
                });
            } else {
                callRecord.notes = notes;
                CallRecordsList.Add(callRecord);
            }
        }

        public void appendSurvey(SkipSurvey.SkipSurveyResult result) {
            shiftCounter.CallNumber++;
            bool isPrompted = (result.Reason == SkipSurvey.SkipSurveyOption.None);
            // TODO: Update to actually record survey record
            UICallbacks.appendNote(shiftCounter.CallNumber + ": " + isPrompted + (string.IsNullOrWhiteSpace(result.Text) ? string.Empty : (" - " + result.Text)));
        }

        private string toShortTimeString(TimeSpan timeSpan) {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        private DateTime fromCurrent(DateTime current, TimeSpan offset) {
            return new DateTime(current.Year, current.Month, current.Day, offset.Hours, offset.Minutes, offset.Seconds);
        }

        private string formatNotes(string notes) {
            // Replaces newline with semicolon
            return reg.Replace(notes, "; ");
        }

        private Regex reg = new Regex("(\r\n|\n|\r)");
    }
}
