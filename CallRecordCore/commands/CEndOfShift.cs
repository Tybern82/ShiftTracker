using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {
    public class CEndOfShift : Command {
        public void Process() {
			if (CallRecordCore.Instance.CurrentCall.IsInCall) {
				CallRecordCore.Instance.Messages.Enqueue(new CStopCall());
				CallRecordCore.Instance.Messages.Enqueue(this);
			} else {
				DateTime currTime = DateTime.Now;
				string currTimeText = currTime.ToLongDateString();

				string report = currTimeText + "\n";

				report += "Calls: " + CallRecordCore.Instance.UIProperties.TotalCalls
					+ "; Dropped: " + CallRecordCore.Instance.ShiftCounter.TotalDropped
					+ "; Wrap: " + CallRecordCore.toShortTimeString(CallRecordCore.Instance.UIProperties.TotalWrap)
					+ "; MAE: " + CallRecordCore.Instance.UIProperties.TotalMAE
					+ "; \nTotal Call Time: " + CallRecordCore.toShortTimeString(CallRecordCore.Instance.ShiftCounter.TotalDuration)
					+ "; Total SME Time: " + CallRecordCore.toShortTimeString(CallRecordCore.Instance.ShiftCounter.TotalSMETime)
					+ "; Total Transfer Time: " + CallRecordCore.toShortTimeString(CallRecordCore.Instance.ShiftCounter.TotalTransferTime)
					+ "\n";

				// report += "MAE%: " + parseFloat(totalMAE/totalCalls*100).toFixed(2) + "%; Wrap%: " + Number(totalWrap / totalDuration).toLocaleString(undefined,{style: 'percent', minimumFractionDigits:2}) + "; AHT: " + toShortTimeString((totalDuration / (totalCalls))) + "\n";

				int i = 1;
				foreach (CallRecord r in CallRecordCore.Instance.UIProperties.CallRecordsList) {
					if (r.durationTicks == 0 && r.MAE == 0) {
						report += "Additional Notes: " + r.notes + "\n";
					} else {
						report += "Call " + i + " - " + CallRecordCore.toShortTimeString(r.duration) + " (" + CallRecordCore.toShortTimeString(r.wrap) + "): MAE = " + r.MAE + "; " + r.notes + "\n";
						i++;
					}
				}
				string callTypes = CallRecordCore.Instance.ShiftCounter.CallTypeCounter.ToString();
				if (!string.IsNullOrWhiteSpace(callTypes)) report += callTypes + "\n";

				CallRecordCore.Instance.UICallbacks?.SetClipboard(report);
            }

            /*
             * 
							
				report += "\nSurvey Records: " + currTimeText + "\n";
				for (var i = 1, row; row = tblSurveyRecords.rows[i]; i++) {
				    var number = row.cells[0].innerText;
				    var prompted = row.cells[1].innerText;
				    var detail = row.cells[2].innerText;
				    
				    var surveyRecord = number + ": " + prompted;
				    if (prompted == "No") surveyRecord += " ";
				    if ((detail != "") && (detail != "\n")) surveyRecord += " - " + detail;
				    report += surveyRecord + "\n";
				}
				report += "\n";
				
				navigator.clipboard.writeText(report);
				window.alert("Report copying to clipboard");
				
				if (txtEmailLog.value != "") {
				    doMailRecord(txtEmailLog.value, encodeURI(report), "Call Records: " + currTimeText);
			    }
			*/
        }
    }
}
