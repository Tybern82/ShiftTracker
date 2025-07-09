using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.ShiftTracker.enums;

namespace com.tybern.ShiftTracker.data {
    public interface AutoNoteStore {

        public AutoNotesStatus AutoNotesStatus { get; set; }
    }
}
