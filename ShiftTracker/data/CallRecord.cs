using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using com.tybern.ShiftTracker.enums;

namespace com.tybern.ShiftTracker.data {
    public class CallRecord : NoteRecord, AutoNoteStore {


        public static IEnumerable<CallType> MODELS = Enum.GetValues(typeof(CallType)).Cast<CallType>();
        public IEnumerable<CallType> Models { get { return MODELS; } }

        public CallRecord(DateTime startTime) : base(startTime) { }

        private DateTime? _EndTime = null;
        public DateTime EndTime {
            get => _EndTime ?? StartTime;
            set {
                _EndTime = value;
                onPropertyChanged(nameof(EndTime));
            }
        }

        public TimeSpan CallTime => EndTime - StartTime;

        private TimeSpan _SMETIme = TimeSpan.Zero;
        public TimeSpan SMETime {
            get => _SMETIme;
            set {
                _SMETIme = value;
                onPropertyChanged(nameof(SMETime));
            }
        }

        private TimeSpan _WrapTime = TimeSpan.Zero;
        public TimeSpan WrapTime {
            get => _WrapTime;
            set {
                _WrapTime = value;
                onPropertyChanged(nameof(WrapTime));
            }
        }

        private TimeSpan _TransferTime = TimeSpan.Zero;
        public TimeSpan TransferTime {
            get => _TransferTime;
            set {
                _TransferTime = value;
                onPropertyChanged(nameof(TransferTime));
            }
        }

        private int _TransferCount = 0;
        public int TransferCount {
            get => _TransferCount;
            set {
                _TransferCount = value;
                onPropertyChanged(nameof(TransferCount));
            }
        }

        private int _CallbackCount = 0;
        public int CallbackCount {
            get => _CallbackCount;
            set {
                _CallbackCount = value;
                onPropertyChanged(nameof(CallbackCount));
            }
        }

        private bool _IsPreferredNameRequested = false;
        public bool IsPreferredNameRequested {
            get => _IsPreferredNameRequested;
            set {
                _IsPreferredNameRequested = value;
                onPropertyChanged(nameof(IsPreferredNameRequested));
            }
        }

        private SurveyStatus _Survey = SurveyStatus.Missing;
        public SurveyStatus Survey {
            get => _Survey;
            set {
                _Survey = value;
                onPropertyChanged(nameof(Survey));
            }
        }

        private CallType _Type = CallType.NBN;
        public CallType Type {
            get => _Type;
            set {
                _Type = value;
                onPropertyChanged(nameof(Type));
            }
        }

        private AutoNotesStatus _AutoNotesStatus = AutoNotesStatus.None;
        public AutoNotesStatus AutoNotesStatus {
            get => _AutoNotesStatus;
            set {
                _AutoNotesStatus = value;
                onPropertyChanged(nameof(AutoNotesStatus));
            }
        }
    }
}
