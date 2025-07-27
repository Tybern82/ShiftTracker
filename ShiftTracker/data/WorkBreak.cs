using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using com.tybern.ShiftTracker.enums;
using SQLite.Net2;

namespace com.tybern.ShiftTracker.data {

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

                    default:
                        EndTime = StartTime; break;
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
                    case BreakType.UnpaidLeave:
                    case BreakType.PublicHoliday:
                    default:
                        return false;
                }
            }
        }

        private bool? isFine = null;
        public bool IsFine {
            get { 
                if (isFine == null) {
                    isFine = (
                        (StartTime.Minutes % 5 != 0)
                        || (EndTime.Minutes % 5 != 0)
                        || (Type == BreakType.Coaching)
                        || (Type == BreakType.SystemIssue));
                }
                return isFine ?? false;
            }
            set {
                isFine = value;
                onPropertyChanged(nameof(IsFine));
            }
        }

        public TimeSpan Length {
            get {
                if (EndTime < StartTime) {
                    DateTime sdt = fromCurrent(CurrentDate, StartTime);
                    DateTime edt = fromCurrent(CurrentDate + TimeSpan.FromDays(1), EndTime);
                    return edt - sdt;
                } else {
                    return EndTime - StartTime;
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

        public static DateTime fromCurrent(DateTime currDay, TimeSpan timeOffset) => new DateTime(currDay.Year, currDay.Month, currDay.Day, timeOffset.Hours, timeOffset.Minutes, timeOffset.Seconds);

        public static TimeSpan GetTotalLength(SortedSet<WorkBreak> breaks) {
            TimeSpan _result = TimeSpan.Zero;
            foreach (WorkBreak brk in breaks) _result += brk.Length;
            return _result;
        }
    }
}
