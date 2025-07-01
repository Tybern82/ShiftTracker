using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using ShiftTrackerGUI.Views;

namespace ShiftTrackerGUI;

public partial class ShiftTimesView : UserControl {

    public delegate void DateChangedEvent(DateTime oldDate, DateTime newDate);

    public ShiftTimesView() {
        InitializeComponent();

        DataContext = ActiveShift;

        fDateSelector.SelectedDateChanged += (sender, args) => onSelectDate?.Invoke(args.OldDate?.Date ?? DateTime.MinValue, args.NewDate?.Date ?? DateTime.Now);

        btnStandardShift.Click += (sender, args) => onTriggerStandardShift?.Invoke();
        btnExtendedShift.Click += (sender, args) => onTriggerExtendedShift?.Invoke();

        btnAddBreak.Click += (sender, args) => onAddBreak?.Invoke();
        btnRemoveBreak.Click += (sender, args) => onRemoveBreak?.Invoke();
        btnClearBreaks.Click += (sender, args) => onClearBreaks?.Invoke();
        btnAddStandardBreaks.Click += (sender, args) => onAddStandardBreaks?.Invoke();
    }

    public void addDefaultHandlers() {
        // These handlers link to operations under the ActiveShift
        onTriggerStandardShift += () => ActiveShift.EndTime = ActiveShift.StartTime + TimeSpan.FromHours(7) + TimeSpan.FromMinutes(50);
        onTriggerExtendedShift += () => ActiveShift.EndTime = ActiveShift.StartTime + TimeSpan.FromHours(7) + TimeSpan.FromMinutes(55);

        onAddStandardBreaks += () => ActiveShift.doAddStandardBreaks();
        onClearBreaks += () => ActiveShift.Breaks.Clear();
        onAddBreak += () => ActiveShift.doAddBreak();
        onRemoveBreak += () => ActiveShift.doRemoveBreak((WorkBreak)dataBreakList.SelectedItem);
    }

    public static readonly StyledProperty<WorkShift> ActiveShiftProperty = AvaloniaProperty.Register<ShiftTimesView, WorkShift>(name: "ActiveShift", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
    public static readonly StyledProperty<bool> EditDateProperty = AvaloniaProperty.Register<ShiftTimesView, bool>(name: "EditDate", defaultValue: true);

    public event DateChangedEvent? onSelectDate;

    public event CommandEvent? onTriggerStandardShift;
    public event CommandEvent? onTriggerExtendedShift;

    public event CommandEvent? onAddBreak;
    public event CommandEvent? onRemoveBreak;
    public event CommandEvent? onClearBreaks;
    public event CommandEvent? onAddStandardBreaks;

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
}