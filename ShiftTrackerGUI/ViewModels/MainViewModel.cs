using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;
using com.tybern.ShiftTracker.enums;
using ReactiveUI;
using StateMachine;

namespace ShiftTrackerGUI.ViewModels;

public class MainViewModel : ViewModelBase {

    public ShiftSM ShiftState { get; private set; } = new ShiftSM();

    public CallSM CallState { get; private set; } = new CallSM();
    public IEnumerable<CallType> Models { get; } = CallRecord.MODELS;

    private string _pdfPassword = string.Empty;
    public string PDFPassword {
        get => TrackerSettings.Instance.PDFPassword;
        set {
            this.RaiseAndSetIfChanged(ref _pdfPassword, value);
            TrackerSettings.Instance.PDFPassword = _pdfPassword;
        }
    }

    private TimeSpan _MeetingTime = TimeSpan.Zero;
    public TimeSpan MeetingTime {
        get => TrackerSettings.Instance.MeetingTime;
        set {
            this.RaiseAndSetIfChanged(ref _MeetingTime, value);
            TrackerSettings.Instance.MeetingTime = _MeetingTime;
        }
    }

    public MainViewModel() {}
}
