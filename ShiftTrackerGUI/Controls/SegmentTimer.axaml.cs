using System;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.enums;

namespace ShiftTrackerGUI.Controls;

public class SegmentTimer : TemplatedControl {

    public static readonly StyledProperty<TimeSpan> TimerTextProperty = AvaloniaProperty.Register<NotesControl, TimeSpan>(name:"TimerText", defaultValue:TimeSpan.Zero, defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);

    public TimeSpan TimerText {
        get { return GetValue(TimerTextProperty); }
        set { SetValue(TimerTextProperty, value); }
    }

    public SegmentTimer() {
        ClockTimer.GlobalTimer.ClockUpdate += GlobalTimer_ClockUpdate;
    }

    ~SegmentTimer() {
        ClockTimer.GlobalTimer.ClockUpdate -= GlobalTimer_ClockUpdate;
    }

    private DateTime? startTime = null;
    private DateTime? endTime = null;

    public void startTimer() => startTime = DateTime.Now;
    public void stopTimer() => ClockTimer.GlobalTimer.ClockUpdate -= GlobalTimer_ClockUpdate;

    private void GlobalTimer_ClockUpdate(DateTime currTime) {
        endTime = currTime;
        if (Dispatcher.UIThread.CheckAccess())
            TimerText = (startTime == null) ? TimeSpan.Zero : (currTime - startTime ?? TimeSpan.Zero);
        else
            Dispatcher.UIThread.Invoke(() => TimerText = (startTime == null) ? TimeSpan.Zero : (currTime - startTime ?? TimeSpan.Zero));
    }

    public WorkBreak createBreak(BreakType type) => new() { 
        Type = type, 
        StartTime = startTime?.TimeOfDay ?? DateTime.Now.TimeOfDay, 
        EndTime = endTime?.TimeOfDay ?? DateTime.Now.TimeOfDay, 
        CurrentDate = startTime?.Date ?? DateTime.Today 
    };

    public NoteRecord createNote(string details) => new(startTime ?? DateTime.Now) {
        NoteContent = details
    };
}