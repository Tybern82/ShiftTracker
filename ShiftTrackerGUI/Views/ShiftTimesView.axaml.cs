using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using ShiftTrackerGUI.Views;

namespace ShiftTrackerGUI.Views;

public partial class ShiftTimesView : UserControl {

    public delegate void DateChangedEvent(DateTime oldDate, DateTime newDate);

    public int MinuteIncrement { get; set; } = 5;

    public ShiftTimesView() {
        InitializeComponent();

        DataContext = ActiveShift;

        fDateSelector.SelectedDateChanged += (sender, args) => onSelectDate?.Invoke(args.OldDate?.Date ?? DateTime.MinValue, args.NewDate?.Date ?? DateTime.Now);

        btnStandardShift.Click += (sender, args) => onTriggerStandardShift?.Invoke();
        btnExtendedShift.Click += (sender, args) => onTriggerExtendedShift?.Invoke();

        btnClearDay.Click += (sender, args) => onClearDay?.Invoke();
        btnEditWeek.Click += (sender, args) => onEditWeek?.Invoke();

        btnAddBreak.Click += (sender, args) => onAddBreak?.Invoke();
        btnRemoveBreak.Click += (sender, args) => onRemoveBreak?.Invoke();
        btnClearBreaks.Click += (sender, args) => onClearBreaks?.Invoke();
        btnAddStandardBreaks.Click += (sender, args) => onAddStandardBreaks?.Invoke();
        btnAllDayBreak.Click += (sender, args) => onAddAllDayBreak?.Invoke();
    }

    public void addDefaultHandlers() {
        // These handlers link to operations under the ActiveShift
        onTriggerStandardShift += () => ActiveShift.EndTime = ActiveShift.StartTime + TimeSpan.FromHours(7) + TimeSpan.FromMinutes(50);
        onTriggerExtendedShift += () => ActiveShift.EndTime = ActiveShift.StartTime + TimeSpan.FromHours(7) + TimeSpan.FromMinutes(55);

        onAddStandardBreaks += () => ActiveShift.doAddStandardBreaks();
        onClearBreaks += () => ActiveShift.Breaks.Clear();
        onAddBreak += () => ActiveShift.doAddBreak();
        onRemoveBreak += () => ActiveShift.doRemoveBreak((WorkBreak)dataBreakList.SelectedItem);
        onAddAllDayBreak += () => ActiveShift.doAddAllDayBreak();

        onClearDay += () => {
            onClearBreaks?.Invoke();
            ActiveShift.StartTime = TimeSpan.Zero;
            ActiveShift.EndTime = TimeSpan.Zero;
        };
    }

    public static readonly StyledProperty<WorkShift> ActiveShiftProperty = AvaloniaProperty.Register<ShiftTimesView, WorkShift>(name: "ActiveShift", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
    public static readonly StyledProperty<bool> EditDateProperty = AvaloniaProperty.Register<ShiftTimesView, bool>(name: "EditDate", defaultValue: true);
    public static readonly StyledProperty<bool> EditWeekVisibleProperty = AvaloniaProperty.Register<ShiftTimesView, bool>(name: "EditWeekVisible", defaultValue: true);

    public event DateChangedEvent? onSelectDate;

    public event CommandEvent? onTriggerStandardShift;
    public event CommandEvent? onTriggerExtendedShift;

    public event CommandEvent? onClearDay;
    public event CommandEvent? onEditWeek;

    public event CommandEvent? onAddBreak;
    public event CommandEvent? onRemoveBreak;
    public event CommandEvent? onClearBreaks;
    public event CommandEvent? onAddStandardBreaks;
    public event CommandEvent? onAddAllDayBreak;

    public WorkShift ActiveShift {
        get {
            WorkShift _result = GetValue(ActiveShiftProperty);
            if (_result == null) {
                _result = new WorkShift(DateTime.Now);
                SetValue(ActiveShiftProperty, _result);
                DataContext = _result;
            }
            return _result;
        }

        set {
            value.matchState(ActiveShift);
            SetValue(ActiveShiftProperty, value);
            DataContext = value;
        }
    }

    public bool EditDate {
        get => GetValue(EditDateProperty);
        set {
            SetValue(EditDateProperty, value);
            fDateSelector.IsEnabled = value;
        }
    }

    public bool EditWeekVisible {
        get => GetValue(EditWeekVisibleProperty);
        set {
            SetValue(EditWeekVisibleProperty, value);
            btnEditWeek.IsVisible = value;
        }
    }
}