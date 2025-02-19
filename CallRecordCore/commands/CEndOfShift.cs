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

				report += "\n";
				foreach (SurveyRecord r in CallRecordCore.Instance.UIProperties.SurveyRecordList) {
					report += "Call " + r.AsString + "\n";
				}

				CallRecordCore.Instance.UICallbacks?.SetClipboard(report);

				CallRecordCore.Instance.Messages.Enqueue(new CSendMail(currTimeText, CallRecordCore.Instance.UIProperties.DestinationAddress, report));

                CallRecordCore.Instance.UICallbacks?.EnableButton(UICallbacks.UITriggerType.StartShiftButton);
                CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.StartBreakButton);
                CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.EndBreakButton);
                CallRecordCore.Instance.UICallbacks?.DisableButton(UICallbacks.UITriggerType.EndShiftButton);
            }
        }

        private DateTime? getNextShift() {
            DateTime currDay = CallRecordCore.fromCurrent(DateTime.Now, TimeSpan.Zero);
            BreakTimes _times = new BreakTimes();
			// Only check a limited number of days - may have no shifts recorded in future already
            for (int i = 1; i <= 14; i++) {
                DateTime testDay = currDay.AddDays(i);
                CallRecordCore.Instance.BreakTimesDB.LoadBreakTimes(testDay, _times);

                if (_times.ShiftStart != TimeSpan.Zero) {
					// Has a shift, return this date
					return testDay;
                }
            }
			return null;
        }
    }
}
