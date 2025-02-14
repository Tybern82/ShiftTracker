using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using com.tybern.CMDProcessor;
using SQLite;

namespace com.tybern.CallRecordCore {
    /// <summary>
    /// Central class for program logic. This class contains the worker processing instance for application commands, as well
    /// as entries to access program state. 
    /// </summary>
    public class CallRecordCore {

        /// <summary>
        /// Generate the application-wide static instance - all requests should pass through a single instance.
        /// </summary>
        public static CallRecordCore Instance { get; } = new CallRecordCore();

        /// <summary>
        /// Single-threaded worker queue for processing application commands.
        /// </summary>
        public CommandProcessor Messages { get; } = new CommandProcessor();

        /// <summary>
        /// Central UI/GUI components - properties of this class will be linked to values on the actual UI which can 
        /// then be updated by application code to update the UI values. 
        /// </summary>
        public UIProperties UIProperties { get; } = new UIProperties();

        /// <summary>
        /// Combines various shift tracking counters.
        /// </summary>
        public ShiftCallCounter ShiftCounter { get; } = new ShiftCallCounter();

        /// <summary>
        /// Contains details on the currently active / last active call. 
        /// </summary>
        public CallDetails CurrentCall { get; } = new CallDetails();

        /// <summary>
        /// Contains the reference to the UI Callbacks instance used to initiate callbacks to the UI/GUI interface
        /// (ie to display dialogs, etc)
        /// </summary>
        public UICallbacks? UICallbacks { get; set; }

        public bool InBreak { get; set; } = false;
        public TimeSpan BreakStartTime { get; set; } = TimeSpan.Zero;

        private ApplicationClockTask _clockTask;


        private SQLiteConnection _conn { get; } = new SQLiteConnection("CallRecordGUI.db", true);

        private CallLog? _CallLog;
        public CallLog CallRecordLog {
            get { 
                if (_CallLog == null) _CallLog = new CallLog(_conn);
                return _CallLog;
            }
        }

        private SurveyLog? _SurveyLog;
        public SurveyLog SurveyRecordLog {
            get {
                if (_SurveyLog == null) _SurveyLog = new SurveyLog(_conn);
                return _SurveyLog;
            }
        }

        /// <summary>
        /// Private constructor - ensures access must be via the single, static instance in this class
        /// </summary>
        private CallRecordCore() {
            _clockTask = new ApplicationClockTask(this);
        }

        public static DateTime fromCurrent(DateTime currDay, TimeSpan timeOffset) => new DateTime(currDay.Year, currDay.Month, currDay.Day, timeOffset.Hours, timeOffset.Minutes, timeOffset.Seconds);
        public static string toShortTimeString(TimeSpan timeSpan) {
            if (timeSpan.TotalHours > 1)
                return timeSpan.ToString(@"hh\:mm\:ss");
            else 
                return timeSpan.ToString(@"mm\:ss");
        }
    }
}
