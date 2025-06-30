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

        private SQLiteConnection? _DBConnection = null;
        public SQLiteConnection DBConnection {
            get {
                if (_DBConnection == null) _DBConnection = new SQLiteConnection(DBFile, storeDateTimeAsTicks: false);
                return _DBConnection;
            }
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
        }

        private JObject encodeJSON() {

            JObject _result = new JObject();

            _result.Add(CONFIGKEY_DBFILE, DBFile);
            _result.Add(CONFIGKEY_SMTP, SMTP.toJSON());
            _result.Add(CONFIGKEY_WPOS, MainWindowPosition.toJSON());

            return _result;
        }

        private static readonly string CONFIGKEY_DBFILE = "DBFile";
        private static readonly string CONFIGKEY_SMTP = "SMTP";
        private static readonly string CONFIGKEY_WPOS = "WindowPosition";
    }
}
