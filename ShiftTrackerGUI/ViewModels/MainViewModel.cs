using System;
using System.Collections.Generic;
using System.ComponentModel;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;
using StateMachine;

namespace ShiftTrackerGUI.ViewModels;

public class MainViewModel : ViewModelBase {

    public ShiftSM ShiftState { get; private set; } = new ShiftSM();

    public CallSM CallState { get; private set; } = new CallSM();

    public MainViewModel() {}
}
