using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using static com.tybern.CallRecordCore.CallDetails;

namespace com.tybern.CallRecordCore {
    public interface UICallbacks {

        public enum CallRecordDialog { SkipSurveyDialog, TransferRequestDialog, SMERequestDialog, MAERequestDialog, OutboundCallDialog, CallNotesDialog, BreakTimesDialog }
        public enum UITriggerType { SurveyButtons, EndCallButton, SMECallButton, MAECallButton, StartCallButton, StartShiftButton, EndShiftButton, StartBreakButton, EndBreakButton }

        public void ShowDialog(CallRecordDialog dlgType);
        public void EnableButton(UITriggerType btnType);
        public void DisableButton(UITriggerType btnType);

        public void SetCallMode(CallMode callMode);

        public void SetClipboard(string? text);
    }

    public class DesignCallbacks : UICallbacks {
        public void DisableButton(UICallbacks.UITriggerType btnType) {}
        public void EnableButton(UICallbacks.UITriggerType btnType) {}
        public void ShowDialog(UICallbacks.CallRecordDialog dlgType) {}
        public void SetCallMode(CallMode callMode) { }
        public void SetClipboard(string? text) { }
    }
}