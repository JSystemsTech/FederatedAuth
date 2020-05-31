using SharedServices.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Email
{
    internal class EmailServiceConfig
    {
        public string SenderName { get; private set; }
        public string SenderEmail { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool UseSSL { get; private set; }
        public bool UseAuthentication { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }

        public EmailServiceConfig(string senderName, string senderEmail, string host,  int port, bool useSSL, bool useAuthentication, string userName, string password)
        {
            SenderName = senderName;
            SenderEmail = senderEmail;
            Host = host;
            Port = port;
            UseSSL = useSSL;
            UseAuthentication = useAuthentication;
            UserName = userName;
            Password = password;
        }
        public EmailServiceConfig(NameValueCollection settings)
        {
            SenderName = settings.GetValue("SenderName");
            SenderEmail = settings.GetValue("SenderEmail");
            Host = settings.GetValue("Host");
            Port = settings.GetValueInt("Port",0);
            UseSSL = settings.GetValueBool("UseSSL");
            UseAuthentication = settings.GetValueBool("UseAuthentication");
            UserName = settings.GetValue("UserName");
            Password = settings.GetValue("Password");
        }
    }
}
