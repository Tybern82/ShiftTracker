using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;
using com.tybern.ShiftTracker.enums;
using StateMachine;

namespace ShiftTrackerGUI.ViewModels;

public class MainViewModel : ViewModelBase {

    public ShiftSM ShiftState { get; private set; } = new ShiftSM();

    public CallSM CallState { get; private set; } = new CallSM();

    private static IEnumerable<CallType> MODELS = Enum.GetValues(typeof(CallType)).Cast<CallType>();
    public IEnumerable<CallType> Models { get; } = MODELS;

    public MainViewModel() {}
}
