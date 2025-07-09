using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using ShiftTrackerGUI.ViewModels;

namespace ShiftTrackerGUI.Views;

public partial class CurrentTimeView : UserControl, INotifyPropertyChanged, CurrentTimeModel {

    private DateTime _CurrentTime = DateTime.Now;
    public DateTime CurrentTime {
        get => _CurrentTime;
        private set {
            _CurrentTime = value;
            onPropertyChanged(nameof(CurrentTime));
        }
    }

    public CurrentTimeView() {
        InitializeComponent();

        DataContext = this;

        ClockTimer.GlobalTimer.ClockUpdate += (currTime) => CurrentTime = currTime;
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void onPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}