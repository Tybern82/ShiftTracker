using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace com.tybern.ShiftTracker.data {
    public class WorkShift : INotifyPropertyChanged {

        public static readonly TimeSpan BREAK_LENGTH = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan LUNCH_LENGTH = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan MEET_LENGTH = TimeSpan.FromMinutes(15);

        private DateTime _CurrentDate;
        public DateTime CurrentDate { 
            get { return _CurrentDate; }
            set { _CurrentDate = value; onPropertyChanged(nameof(CurrentDate)); }
        }

        public DateTimeOffset CurrentDateOffset {
            get { return CurrentDate; }
            set { CurrentDate = value.Date; }
        }

        private TimeSpan _StartTime;
        public TimeSpan StartTime {
            get { return _StartTime; }
            set { _StartTime = value; onPropertyChanged(nameof(StartTime)); }
        }

        private TimeSpan _EndTime;
        public TimeSpan EndTime {
            get { return _EndTime; }
            set { _EndTime = value; onPropertyChanged(nameof(EndTime)); }
        }

        public ObservableCollection<WorkBreak> Breaks { get; } = new ObservableCollection<WorkBreak>();

        public WorkShift(DateTime dt) {
            CurrentDate = dt;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void onPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
