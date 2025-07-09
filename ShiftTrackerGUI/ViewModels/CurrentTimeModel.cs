using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftTrackerGUI.ViewModels {

    public interface CurrentTimeModel {
        public DateTime CurrentTime { get; }
    }

    public class BasicCurrentTimeModel : CurrentTimeModel {
        public DateTime CurrentTime => DateTime.Now;
    }
}
