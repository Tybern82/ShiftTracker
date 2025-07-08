using System;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using com.tybern.ShiftTracker;

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

    public void startTimer() => startTime = DateTime.Now;
    public void stopTimer() => ClockTimer.GlobalTimer.ClockUpdate -= GlobalTimer_ClockUpdate;

    private void GlobalTimer_ClockUpdate(DateTime currTime) {
        if (Dispatcher.UIThread.CheckAccess())
            TimerText = (startTime == null) ? TimeSpan.Zero : (currTime - startTime ?? TimeSpan.Zero);
        else
            Dispatcher.UIThread.Invoke(() => TimerText = (startTime == null) ? TimeSpan.Zero : (currTime - startTime ?? TimeSpan.Zero));
    }
}