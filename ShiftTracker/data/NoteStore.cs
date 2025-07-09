using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tybern.ShiftTracker.data {
    public interface NoteStore {

        public string NoteContent { get; set; }
        public void prependNote(string note);
        public void appendNote(string note);
    }

    public class BasicNoteStore : NoteStore {
        public string NoteContent { get; set; } = string.Empty;

        public void prependNote(string note) => Utility.prependNote(this, note);
        public void appendNote(string note) => Utility.appendNote(this, note);
    }
}
