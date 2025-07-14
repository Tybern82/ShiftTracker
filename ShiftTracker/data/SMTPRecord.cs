using System;
using System.Collections.Generic;
using System.ComponentModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Newtonsoft.Json.Linq;

namespace com.tybern.ShiftTracker.data {
    public class SMTPRecord : EncodeJSON,INotifyPropertyChanged {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private string _Host = string.Empty;
        public string Host { 
            get { return _Host; }
            set { _Host = value; onPropertyChanged(nameof(Host)); }
        }

        private int _Port = 25;
        public int Port {
            get => _Port;
            set {
                _Port = value; onPropertyChanged(nameof(Port));
            }
        }

        private string _Username = string.Empty;
        public string Username {
            get => _Username; 
            set {
                _Username = value; onPropertyChanged(nameof(Username));
            }
        }

        private string _Password = string.Empty;
        public string Password {
            get => _Password;
            set {
                _Password = value; onPropertyChanged(nameof(Password));
            }
        }

        private string _SenderAddress = string.Empty;
        public string SenderAddress {
            get => _SenderAddress;
            set {
                _SenderAddress = value; onPropertyChanged(nameof(SenderAddress));
            }
        }

        private string _DestinationAddress = string.Empty;
        public string DestinationAddress {
            get => _DestinationAddress;
            set {
                _DestinationAddress = value; onPropertyChanged(nameof(DestinationAddress));
            }
        }

        public SMTPRecord(
            string host = "", 
            int port = 25, 
            string username = "", 
            string password = "", 
            string senderAddress = "", 
            string destinationAddress = ""
            ) {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            SenderAddress = senderAddress;
            DestinationAddress = destinationAddress;
        }

        public SMTPRecord(SMTPRecord rCopy) : this(
            rCopy.Host,
            rCopy.Port,
            rCopy.Username,
            rCopy.Password,
            rCopy.SenderAddress,
            rCopy.DestinationAddress) { }

        public SMTPRecord() : this("") { }

        public SMTPRecord(JObject jsonData) {
            Host = jsonData.Value<string>(JKEY_HOST) ?? string.Empty;
            Port = jsonData.Value<int>(JKEY_PORT);
            Username = jsonData.Value<string>(JKEY_UNAME) ?? string.Empty;
            Password = jsonData.Value<string>(JKEY_PWORD) ?? string.Empty;
            SenderAddress = jsonData.Value<string>(JKEY_SEND) ?? string.Empty;
            DestinationAddress = jsonData.Value<string>(JKEY_DEST) ?? string.Empty;
        }

        public void copyFrom(SMTPRecord rCopy) {
            Host = rCopy.Host;
            Port = rCopy.Port;
            Username = rCopy.Username;
            Password = rCopy.Password;
            SenderAddress = rCopy.SenderAddress;
            DestinationAddress = rCopy.DestinationAddress;
        }

        public JObject toJSON() {
            JObject _result = new JObject();

            _result.Add(JKEY_HOST, Host);
            _result.Add(JKEY_PORT, Port);
            _result.Add(JKEY_UNAME, Username);
            _result.Add(JKEY_PWORD, Password);
            _result.Add(JKEY_SEND, SenderAddress);
            _result.Add(JKEY_DEST, DestinationAddress);

            return _result;
        }

        private static readonly string JKEY_HOST = "Host";
        private static readonly string JKEY_PORT = "Port";
        private static readonly string JKEY_UNAME = "Username";
        private static readonly string JKEY_PWORD = "Password";
        private static readonly string JKEY_SEND = "FromAddress";
        private static readonly string JKEY_DEST = "ToAddress";

        public SMTPSendResponse sendMail(string subject, string message, List<MimeEntity> attachments) {
            LOG.Info("Sending email: <" + subject + ">");

            if (string.IsNullOrWhiteSpace(DestinationAddress)) {
                LOG.Info("No destination set...skipping");
                return new SMTPSendResponse() {
                    Success = true,
                    Error = "No destination set...skipping email send"
                };
            }
            SMTPSendResponse _result = new SMTPSendResponse();
            try {
                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress("ShiftTracker", SenderAddress));
                msg.To.Add(new MailboxAddress("", DestinationAddress));
                msg.Subject = subject;

                var builder = new BodyBuilder();
                builder.TextBody = message;
                foreach (MimeEntity a in attachments) builder.Attachments.Add(a);

                msg.Body = builder.ToMessageBody();

                using (var client = new SmtpClient()) {
                    client.Connect(Host, Port, SecureSocketOptions.SslOnConnect);
                    LOG.Info("SMTP Connected");
                    client.Authenticate(Username, Password);
                    LOG.Info("SMTP Authenticated");
                    client.Send(msg);
                    LOG.Info("SMTP Message sent");
                    client.Disconnect(true);
                }
            } catch (Exception e) {
                LOG.Error(e.ToString());
                _result.Success = false;
                _result.Error = e.ToString();
            }
            return _result;
        }

        public SMTPSendResponse sendMail(string subject, string message) => sendMail(subject, message, new List<MimeEntity>());

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void onPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public class SMTPSendResponse {
            public bool Success { get; set; } = true;

            public string Error { get; set; } = string.Empty;
        }
    }
}
