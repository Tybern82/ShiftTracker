using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Newtonsoft.Json.Linq;

namespace com.tybern.ShiftTracker.data {
    public class SMTPRecord : EncodeJSON {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SenderAddress { get; set; }
        public string DestinationAddress { get; set; }

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

        public SMTPRecord() : this("") { }

        public SMTPRecord(JObject jsonData) {
            Host = jsonData.Value<string>(JKEY_HOST) ?? string.Empty;
            Port = jsonData.Value<int>(JKEY_PORT);
            Username = jsonData.Value<string>(JKEY_UNAME) ?? string.Empty;
            Password = jsonData.Value<string>(JKEY_PWORD) ?? string.Empty;
            SenderAddress = jsonData.Value<string>(JKEY_SEND) ?? string.Empty;
            DestinationAddress = jsonData.Value<string>(JKEY_DEST) ?? string.Empty;
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

        public void sendMail(string subject, string message) {
            LOG.Info("Sending email: <" + subject + ">");

            if (string.IsNullOrWhiteSpace(DestinationAddress)) {
                LOG.Info("No destination set...skipping");
                return;
            }
            try {
                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress("ShiftTracker", SenderAddress));
                msg.To.Add(new MailboxAddress("", DestinationAddress));
                msg.Subject = subject;
                msg.Body = new TextPart("plain") { Text = message };

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
            }
        }
    }
}
