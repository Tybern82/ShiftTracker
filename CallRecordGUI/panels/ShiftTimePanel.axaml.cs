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


        public WorkShift View = new WorkShift();

        public ShiftTimePanel() {
            InitializeComponent();

            this.View = WorkShift.LoadToday();

            DataContext = this.View;

            btnStandardShift.Click += (sender, args) => View.doStandardShift();
            btnExtendedShift.Click += (sender, args) => View.doExtendedShift();
            btnAddBreak.Click += (sender, args) => View.doAddBreak();
            btnAddStandardBreaks.Click += (sender, args) => View.doAddStandardBreaks();
            btnClearBreaks.Click += (sender, args) => View.doClearBreaks();
            btnRemoveBreak.Click += (sender, args) => View.doRemoveBreak(dataBreakList.SelectedItem);
        }

        public static readonly StyledProperty<bool> IsDateEnabledProperty = AvaloniaProperty.Register<UserControl, bool>("IsDateEnabled", true);

        public bool IsDateEnabled {
            get { return GetValue(IsDateEnabledProperty); }
            set { SetValue(IsDateEnabledProperty, value); fDateSelector.IsEnabled = value; }    // Property controls visibility of the embeded DateSelector; update when value changed
        }
    }
}