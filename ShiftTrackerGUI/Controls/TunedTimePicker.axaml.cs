using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace ShiftTrackerGUI.Controls;

[TemplatePart("PART_FineButton", typeof(ToggleButton))]
public class TunedTimePicker : TemplatedControl {

    public static readonly StyledProperty<TimeSpan> ActiveTimeProperty = AvaloniaProperty.Register<TunedTimePicker, TimeSpan>(name: "ActiveTime", defaultValue: TimeSpan.Zero, defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);
    public static readonly StyledProperty<bool> IsFineProperty = AvaloniaProperty.Register<TunedTimePicker, bool>(name: "IsFine", defaultValue: false, defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);
    public static readonly DirectProperty<TunedTimePicker, int> IncrementValueProperty = AvaloniaProperty.RegisterDirect<TunedTimePicker, int>(name: "IncrementValue", (ttp) => (ttp.IsFine ? 1 : 5), defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);

    public TimeSpan ActiveTime {
        get => GetValue(ActiveTimeProperty);
        set => SetValue(ActiveTimeProperty, value); 
    }

    public bool IsFine {
        get => GetValue(IsFineProperty);
        set {
            SetValue(IsFineProperty, value);
            SetAndRaise(IncrementValueProperty, ref _IncrementValue, (value ? 1 : 5)); 
        }
    }

    private int _IncrementValue = 5;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        var btnFineControl = e.NameScope.Find<ToggleButton>("PART_FineButton");
        if (btnFineControl != null) {
            btnFineControl.IsCheckedChanged += (sender, args) => IsFine = btnFineControl.IsChecked ?? false;
        }
    }
}