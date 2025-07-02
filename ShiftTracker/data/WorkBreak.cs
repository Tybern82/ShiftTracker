using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.data {
    public enum BreakType {
        [Description("Break")]
        ShiftBreak,

        [Description("Lunch Break")]
        LunchBreak,

        [Description("Meeting")]
        Meeting,

        [Description("Training")]
        Training,

        [Description("Coaching Session")]
        Coaching,

        [Description("Fault / System Issue")]
        SystemIssue,

        [Description("Personal / Sick Leave")]
        PersonalLeave,

        [Description("Public Holiday")]
        PublicHoliday,

        [Description("Annual Leave")]
        AnnualLeave
    };

    public class WorkBreak : INotifyPropertyChanged, IComparable<WorkBreak> {

        private static IEnumerable<BreakType> MODELS = Enum.GetValues(typeof(BreakType)).Cast<BreakType>();
        public IEnumerable<BreakType> Models => MODELS;

        private BreakType _Type;
        public BreakType Type {
            get { return _Type; }
            set { _Type = value; onPropertyChanged(nameof(BreakType)); }
        }

        private DateTime _CurrentDate = DateTime.Now;
        public DateTime CurrentDate {
            get { return _CurrentDate; }
            set { _CurrentDate = value; onPropertyChanged(nameof(CurrentDate)); }
        }

        private TimeSpan _StartTime;
        public TimeSpan StartTime {
            get { return _StartTime; }
            set { 
                _StartTime = value; onPropertyChanged(nameof(StartTime));
                // Auto-Update end time for known items
                switch (Type) {
                    case BreakType.ShiftBreak:
                        EndTime = StartTime + TimeSpan.FromMinutes(15); break;

                    case BreakType.LunchBreak:
                        EndTime = StartTime + TimeSpan.FromMinutes(30); break;

                    case BreakType.Meeting:
                        EndTime = StartTime + TimeSpan.FromMinutes(15); break;

                }
            }
        }

        private TimeSpan _EndTime;
        public TimeSpan EndTime {
            get { return _EndTime; }
            set { _EndTime = value; onPropertyChanged(nameof(EndTime)); }
        }

        public bool IsActiveBreak { // IsActiveBreak identifies whether this type of break should be considered for calculating the next break
            get {
                switch (Type) {
                    case BreakType.ShiftBreak:
                    case BreakType.LunchBreak:
                    case BreakType.Meeting:
                    case BreakType.Training:
                    case BreakType.Coaching:
                        return true;

                    case BreakType.SystemIssue:
                    case BreakType.PersonalLeave:
                    case BreakType.PublicHoliday:
                    default:
                        return false;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void onPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public int CompareTo(WorkBreak other) {
            int _result = _CurrentDate.CompareTo(other._CurrentDate);
            return (_result == 0) ? _StartTime.CompareTo(other._StartTime) : _result;
        }
    }
}
