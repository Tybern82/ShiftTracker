using System;
using System.Collections.Generic;
using System.Text;
using static com.tybern.CallRecordCore.UICallbacks;

namespace com.tybern.CallRecordCore {
    public class CallDetails {

        public enum CallMode { Disconnect, InCall, InWrap };

        private CallMode _CurrentMode = CallMode.InWrap;
        public CallMode CurrentMode {
            get { return _CurrentMode; }
            set {
                if (_CurrentMode != value) {
                    _CurrentMode = value;
                    switch (value) {     // Update Buttons based on the call mode
                        case CallMode.Disconnect:
                            CallRecordCore.Instance.UIProperties.StartCallButtonText = UIProperties.LabelStartCall;
                            CallRecordCore.Instance.UIProperties.IsInboundText = UIProperties.LabelWaiting;
                            CallRecordCore.Instance.UICallbacks?.DisableButton(UITriggerType.SurveyButtons);
                            CallRecordCore.Instance.UICallbacks?.DisableButton(UITriggerType.PrefNameButton);
                            CallRecordCore.Instance.UICallbacks?.DisableButton(UITriggerType.EndCallButton);
                            CallRecordCore.Instance.UICallbacks?.DisableButton(UITriggerType.SMECallButton);
                            CallRecordCore.Instance.UICallbacks?.DisableButton(UITriggerType.MAECallButton);

                            CallRecordCore.Instance.UICallbacks?.DisableButton(UITriggerType.ANGeneratedButton);
                            CallRecordCore.Instance.UICallbacks?.DisableButton(UITriggerType.ANEditedButton);
                            CallRecordCore.Instance.UICallbacks?.DisableButton(UITriggerType.ANSavedButton);
                            CallRecordCore.Instance.UICallbacks?.DisableButton(UITriggerType.ANManualButton);
                            break;

                        case CallMode.InCall:
                            CallRecordCore.Instance.UIProperties.StartCallButtonText = UIProperties.LabelStartWrap;
                            CallRecordCore.Instance.UIProperties.IsInboundText = UIProperties.LabelInboundCall;
                            CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.SurveyButtons);
                            CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.PrefNameButton);
                            CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.EndCallButton);
                            CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.SMECallButton);
                            CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.MAECallButton);

                            CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.ANGeneratedButton);
                            CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.ANManualButton);
                            break;

                        case CallMode.InWrap:
                            CallRecordCore.Instance.UIProperties.StartCallButtonText = UIProperties.LabelStartCallback;
                            CallRecordCore.Instance.UIProperties.IsInboundText = UIProperties.LabelInboundCall;
                            if (CallRecordCore.Instance.CurrentCall.IsSurveyRecorded) {
                                CallRecordCore.Instance.UICallbacks?.DisableButton(UITriggerType.SurveyButtons);
                            } else {
                                CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.SurveyButtons);
                            }
                            if (CallRecordCore.Instance.CurrentCall.IsPrefNameRequested)
                            {
                                CallRecordCore.Instance.UICallbacks?.DisableButton(UITriggerType.PrefNameButton);
                            } else
                            {
                                CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.PrefNameButton);
                            }
                            CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.EndCallButton);
                            CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.SMECallButton);
                            CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.MAECallButton);
                            CallRecordCore.Instance.UICallbacks?.EnableButton(UITriggerType.StartCallButton);
                            break;
                    }
                    CallRecordCore.Instance.UICallbacks?.SetCallMode(_CurrentMode);
                }
            }
        }

        public TimeSpan CallStartTime { get; set; } = TimeSpan.Zero;
        public TimeSpan WrapStartTime { get; set; } = TimeSpan.Zero;
        public TimeSpan SMEStartTime { get; set; } = TimeSpan.Zero;
        public TimeSpan TransferStartTime { get; set; } = TimeSpan.Zero;

        public bool IsInCall { get; set; } = false;
        public bool IsInWrap { get; set; } = false;
        public bool IsCallback { get; set; } = false;
        public bool IsInSMECall { get; set; } = false;
        public bool IsInTransferCall { get; set; } = false;
        public bool IsSurveyRecorded { get; set; } = false;

        // Auto-Notes
        public bool IsANGenerated { get; set; } = false;
        public bool IsANEdited { get; set; } = false;
        public bool IsANSaved { get; set; } = false;
        public bool HasManualNotes { get; set; } = false;

        // Preferred Name
        public bool IsPrefNameRequested { get; set; } = false;
    }
}
