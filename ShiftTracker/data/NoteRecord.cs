using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace com.tybern.ShiftTracker.data {
    public class NoteRecord : NoteStore, INotifyPropertyChanged, IComparable<NoteRecord> {

        public NoteRecord(DateTime startTime) {
            this.StartTime = startTime;
        }

        private DateTime _StartTime;
        public DateTime StartTime {
            get => _StartTime;
            set {
                _StartTime = value;
                onPropertyChanged(nameof(StartTime));
            }
        }

        private string _NoteContent = string.Empty;
        public string NoteContent {
            get => _NoteContent;
            set {
                _NoteContent = value;
                onPropertyChanged(nameof(NoteContent));
            }
        }

        public void appendNote(string note) => Utility.appendNote(this, note);
        public void prependNote(string note) => Utility.prependNote(this, note);

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void onPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public int CompareTo(NoteRecord other) => StartTime.CompareTo(other.StartTime);
    }
}
