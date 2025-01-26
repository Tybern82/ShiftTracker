using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace com.tybern.CallRecordCore.commands {
    public class CSendMail : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public static string FROM_ADDRESS { get; set; } = "email@host.com";
        public static string SMTP_HOST { get; set; } = "smtp.gmail.com";
        public static  int SMTP_PORT { get; set; } = 587;
        public static  string SMTP_USERNAME { get; set; } = "email@host.com";
        public static  string SMTP_PASSWORD { get; set; } = "Password";

        private string Subject { get;  }
        private string Address { get; }
        private string Report { get; }

        public CSendMail(string subject, string emailAddress, string report) {
            Subject = subject;
            Address = emailAddress;
            Report = report; 
        }

        public void Process() {
            LOG.Info("Send eMail");
            if (string.IsNullOrWhiteSpace(Address)) {
                LOG.Info("Skipping eMail");
                return;
            }
            try {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("CallRecordGUI", FROM_ADDRESS));
                message.To.Add(new MailboxAddress("", Address));    // Default to FROM_ADDRESS if not specified
                message.Subject = Subject;
                message.Body = new TextPart("plain") { Text = Report };

                using (var client = new SmtpClient()) {
                    client.Connect(SMTP_HOST, SMTP_PORT, SecureSocketOptions.SslOnConnect);
                    LOG.Info("SMTP Connected");
                    LOG.Info("Authenticating: <" + SMTP_USERNAME + ">:<" + SMTP_PASSWORD + ">");
                    client.Authenticate(SMTP_USERNAME, SMTP_PASSWORD);
                    LOG.Info("SMTP Authenticated");
                    client.Send(message);
                    LOG.Info("SMTP Sent...");
                    client.Disconnect(true);
                }
            } catch (Exception e) {
                LOG.Error(e.ToString());
            }
        }
    }
}
