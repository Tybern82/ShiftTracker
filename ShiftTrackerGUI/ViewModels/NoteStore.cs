using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftTrackerGUI.ViewModels {
    public interface NoteStore {

        public string NoteContent { get; set; }
    }

    public class BasicNoteStore : NoteStore {
        public string NoteContent { get; set; } = string.Empty;
    }
}
