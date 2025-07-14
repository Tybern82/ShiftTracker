using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.tybern.ShiftTracker;
using ReactiveUI;

namespace ShiftTrackerGUI.ViewModels {
    internal class StatusBarModel : ViewModelBase {

        private const int DECAY = 10;

        private int countdown = 0;
        private string _StatusText = string.Empty;
        public string StatusText {
            get => _StatusText;
            set {
                lock (this) {
                    this.countdown = DECAY;
                    this.RaiseAndSetIfChanged(ref _StatusText, value);
                }
            }
        }

        private string _StatusVersion = string.Empty;
        public string StatusVersion {
            get => _StatusVersion;
            set => this.RaiseAndSetIfChanged(ref _StatusVersion, value);
        }

        public StatusBarModel() {
            ClockTimer.GlobalTimer.ClockUpdate += statusDecay;
        }

        ~StatusBarModel() {
            ClockTimer.GlobalTimer.ClockUpdate -= statusDecay;
        }

        private void statusDecay(DateTime currTIme) {
            lock (this) {   // lock to ensure we can't change countdown while we're processing
                if (this.countdown < 0) {
                    // NOP
                } else if (this.countdown == 0) {
                    StatusText = string.Empty;
                    this.countdown--;   // will move countdown to -1 so we don't repeatedly update StatusText
                } else {
                    this.countdown--;
                }
            }
        }
    }
}
