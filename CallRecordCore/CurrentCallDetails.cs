using System;
using System.Collections.Generic;
using System.Text;

namespace CallRecordCore {
    public class CurrentCallDetails {

        public enum CallMode { Disconnect, InCall, InWrap };

        private UICallbacks UICallbacks { get; }
        public CurrentCallDetails(UICallbacks callbacks) { UICallbacks = callbacks; }

        private CallMode _CurrentMode = CallMode.Disconnect;
        public CallMode CurrentMode { 
            get {  return _CurrentMode; }
            set { _CurrentMode = value; UICallbacks.setCallMode(_CurrentMode); }
        }
        public bool isInCall { get; set; } = false;
        public bool isInWrap { get; set; } = false;
        public bool isCallback { get; set; } = false;
        public bool isInSMECall { get; set; } = false;
        public bool isInTransferCall { get; set; } = false;
        public bool isSurveyRegistered { get; set; } = false;
        public bool isSurveyRecorded { get; set; } = false;

        public TimeSpan callStartTime { get; set; } = TimeSpan.Zero;
        public TimeSpan wrapStartTime { get; set; } = TimeSpan.Zero;
        public TimeSpan smeStartTime { get; set; } = TimeSpan.Zero;
        public TimeSpan transferStartTime { get; set; } = TimeSpan.Zero;
    }
}
