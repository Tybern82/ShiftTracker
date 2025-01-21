using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Mozilla;

namespace CallRecordCore {
    public interface UICallbacks {

        public void setCallMode(CallRecordCore.CurrentCallDetails.CallMode mode);

        public void enableSurvey(bool enable);

        public Task<SkipSurvey.SkipSurveyResult> doSkipSurvey();
        public Task<SMERequest.SMERequestResult> doSMERequest();
        public Task<CallNotes.CallNotesResult> doCallNotes();
        public Task<OutboundCall.OutboundCallResult> doOutboundCall();
        public Task<MultipleTransfer.MultipleTransferResult> doMAERequest();
        public Task<TransferRequest.TransferRequestResult> doTransferRequest();

        public void appendNote(string note);
        public void prependNote(string note);
        public string getNotes();
        public void clearNotes();
    }

    public class DesignCallbacks : UICallbacks {
        public void setCallMode(CallRecordCore.CurrentCallDetails.CallMode callMode) { }

        public void enableSurvey(bool enable) { }

        public Task<SkipSurvey.SkipSurveyResult> doSkipSurvey() { return new Task<SkipSurvey.SkipSurveyResult>(() => new SkipSurvey.SkipSurveyResult(SkipSurvey.SkipSurveyOption.None)); }
        public Task<SMERequest.SMERequestResult> doSMERequest() { return new Task<SMERequest.SMERequestResult>(() => new SMERequest.SMERequestResult(SMERequest.SMERequestOption.Tools)); }
        public Task<CallNotes.CallNotesResult> doCallNotes() { return new Task<CallNotes.CallNotesResult>(() => new CallNotes.CallNotesResult(CallNotes.CallType.Other)); }
        public Task<OutboundCall.OutboundCallResult> doOutboundCall() { return new Task<OutboundCall.OutboundCallResult>(() => new OutboundCall.OutboundCallResult(OutboundCall.OutboundCallOption.Other)); }
        public Task<MultipleTransfer.MultipleTransferResult> doMAERequest() { return new Task<MultipleTransfer.MultipleTransferResult>(() => new MultipleTransfer.MultipleTransferResult(MultipleTransfer.MultipleTransferOption.Other)); }
        public Task<TransferRequest.TransferRequestResult> doTransferRequest() { return new Task<TransferRequest.TransferRequestResult>(() => new TransferRequest.TransferRequestResult(TransferRequest.TransferRequestOption.Other)); }

        public void appendNote(string note) { }
        public void prependNote(string note) { }
        public string getNotes() { return ""; }
        public void clearNotes() { }
    }
}
