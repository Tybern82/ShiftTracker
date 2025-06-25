using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using com.tybern.ShiftTracker.db;
using com.tybern.ShiftTracker.shifts;

namespace com.tybern.CallRecordGUI.panels {

    public partial class ShiftTimePanel : UserControl {

        public ShiftTimePanel() {
            InitializeComponent();

            DataContext = View; // preload current view
            // this.View = WorkShift.LoadToday();

            btnStandardShift.Click += (sender, args) => View.doStandardShift();
            btnExtendedShift.Click += (sender, args) => View.doExtendedShift();
            btnAddBreak.Click += (sender, args) => View.doAddBreak();
            btnAddStandardBreaks.Click += (sender, args) => View.doAddStandardBreaks();
            btnClearBreaks.Click += (sender, args) => View.doClearBreaks();
            btnRemoveBreak.Click += (sender, args) => View.doRemoveBreak(dataBreakList.SelectedItem);
        }

        public static readonly StyledProperty<bool> IsDateEnabledProperty = AvaloniaProperty.Register<ShiftTimePanel, bool>("IsDateEnabled", true);

        public bool IsDateEnabled {
            get { return GetValue(IsDateEnabledProperty); }
            set { SetValue(IsDateEnabledProperty, value); fDateSelector.IsEnabled = value; }    // Property controls visibility of the embeded DateSelector; update when value changed
        }

        public static readonly StyledProperty<WorkShift> ViewProperty = AvaloniaProperty.Register<ShiftTimePanel, WorkShift>("View");

        public WorkShift View {
            get {
                WorkShift _result = GetValue(ViewProperty);
                if (_result == null) {
                    _result = WorkShift.LoadToday();    // if unspecified, load current date from DB; set here rather than in Register to ensure that current date is loaded when used, and separate instances load separate records
                    SetValue(ViewProperty, _result);
                    DataContext = _result;
                }
                return _result;
            }

            set {
                SetValue(ViewProperty, value);
                DataContext = value;
            }
        }
    }
}