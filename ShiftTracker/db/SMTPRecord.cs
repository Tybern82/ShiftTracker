using System;
using System.Collections.Generic;
using System.Text;

namespace com.tybern.ShiftTracker.db {
    public class SMTPRecord {

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
    }
}
