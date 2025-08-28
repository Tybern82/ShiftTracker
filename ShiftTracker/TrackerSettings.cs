using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using com.tybern.ShiftTracker.data;
using Newtonsoft.Json.Linq;
using SQLite.Net2;

namespace com.tybern.ShiftTracker {
    public class TrackerSettings {

        private static readonly string ConfigFile = "ShiftTracker.config";
        private static readonly string DefaultDBFile = "ShiftTracker.db";

        public static TrackerSettings Instance = new TrackerSettings();

        public string VersionString { get; } = "v2.1.5";

        private string _DBFile = DefaultDBFile;
        public string DBFile { 
            get {
                return _DBFile;
            }
            set {
                if (!string.Equals(_DBFile, value, StringComparison.InvariantCulture)) {    // only change if file has actually changed
                    _DBFile = value;
                    if (_DBConnection != null) {    // reset the active connection, will reconnect for next request from the new file
                        _DBConnection = null;       // should automatically close the old connection from Dispose 
                    }
                }
            } 
        }

        public SMTPRecord SMTP { get; set; } = new SMTPRecord();
        public WindowPosition MainWindowPosition { get; set; } = new WindowPosition();

        public string PDFPassword { get; set; } = string.Empty;

        private Dictionary<string, WindowPosition> savedWindows { get; set; } = new Dictionary<string, WindowPosition>();

        public TimeSpan MeetingTime { get; set; } = TimeSpan.Zero;

        private SQLiteConnection? _DBConnection = null;
        public SQLiteConnection DBConnection {
            get {
                if (_DBConnection == null) _DBConnection = new SQLiteConnection(DBFile, storeDateTimeAsTicks: false);
                return _DBConnection;
            }
        }

        public void saveWindow(string name, WindowPosition position) {
            savedWindows[name] = position;
        }

        public WindowPosition? loadWindow(string name) {
            try {
                return savedWindows[name];
            } catch (KeyNotFoundException) { return null; }
        }

        public void loadConfigFile() {
            using (FileStream configFile = File.Open(ConfigFile, FileMode.OpenOrCreate)) {
                using (StreamReader configReader = new StreamReader(configFile)) {
                    string configJSON = configReader.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(configJSON)) {
                        JObject configData = JObject.Parse(configJSON);
                        decodeJSON(configData);
                    }
                }
            }
        }

        public void saveConfigFile() {
            using (FileStream configFile = File.Open(ConfigFile, FileMode.Create)) {
                using (StreamWriter configWriter = new StreamWriter(configFile)) {
                    configWriter.Write(encodeJSON().ToString());
                    configWriter.Flush();
                    configWriter.Close();
                }
            }
        }

        private void decodeJSON(JObject configData) {

            string? dbFile = configData.ContainsKey(CONFIGKEY_DBFILE) ? configData.Value<string>(CONFIGKEY_DBFILE) : DefaultDBFile;
            if (dbFile != null) DBFile = dbFile;

            if (configData.ContainsKey(CONFIGKEY_SMTP)) {
                JObject? smtp = configData.Value<JObject>(CONFIGKEY_SMTP);
                if (smtp != null) SMTP = new SMTPRecord(smtp);
            }

            if (configData.ContainsKey(CONFIGKEY_WPOS)) {
                JObject? windowPosition = configData.Value<JObject>(CONFIGKEY_WPOS);
                if (windowPosition != null) MainWindowPosition = new WindowPosition(windowPosition);
            }

            if (configData.ContainsKey(CONFIGKEY_AWPS)) {
                JArray? additionalWindows = configData.Value<JArray>(CONFIGKEY_AWPS);
                if (additionalWindows != null) {
                    foreach (JObject wnd in additionalWindows) {
                        WindowPosition wPos = new WindowPosition(wnd);
                        string? name = wnd.Value<string>(CONFIGKEY_WPOS_NAME);
                        if (name != null) {
                            savedWindows[name] = wPos;
                        }
                    }
                }
            }

            if (configData.ContainsKey(CONFIGKEY_MEET)) {
                string? meetTime = configData.Value<string>(CONFIGKEY_MEET);
                if (meetTime != null) MeetingTime = TimeSpan.Parse(meetTime);
            }

            if (configData.ContainsKey(CONFIGKEY_EPWD)) {
                string? epwd = configData.Value<string>( CONFIGKEY_EPWD);
                if (epwd != null) PDFPassword = epwd;
            }
        }

        private JObject encodeJSON() {

            JObject _result = new JObject();

            _result.Add(CONFIGKEY_DBFILE, DBFile);
            _result.Add(CONFIGKEY_SMTP, SMTP.toJSON());
            _result.Add(CONFIGKEY_EPWD, PDFPassword);
            _result.Add(CONFIGKEY_WPOS, MainWindowPosition.toJSON());
            _result.Add(CONFIGKEY_MEET, MeetingTime.ToString(@"hh\:mm"));

            if (savedWindows.Count > 0) {
                JArray additionalWindows = new JArray();
                foreach (string s in savedWindows.Keys) {
                    WindowPosition wPos = savedWindows[s];
                    JObject wnd = wPos.toJSON();
                    wnd.Add(CONFIGKEY_WPOS_NAME, s);
                    additionalWindows.Add(wnd);
                }
                _result.Add(CONFIGKEY_AWPS, additionalWindows);
            }

            return _result;
        }

        private static readonly string CONFIGKEY_DBFILE = "DBFile";
        private static readonly string CONFIGKEY_SMTP = "SMTP";
        private static readonly string CONFIGKEY_WPOS = "WindowPosition";
        private static readonly string CONFIGKEY_AWPS = "AdditionalWindows";
        private static readonly string CONFIGKEY_WPOS_NAME = "Name";
        private static readonly string CONFIGKEY_MEET = "MeetingTime";
        private static readonly string CONFIGKEY_EPWD = "PDFPassword";
    }
}
