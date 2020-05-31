using MimeKit;
using SharedServices.Email.Extensions;
using System;
using System.Linq;

namespace SharedServices.Email
{
    public interface IEmailServiceMessage : IDisposable
    {
        void Send();
        void AddSubject(string subject);
        void AddBodyMessage(string body);
        void AddBody(BodyBuilder body);
        void AddRecipient(string name, string emailAddress);
        void AddCc(string name, string emailAddress);
        void AddBcc(string name, string emailAddress);
    }
    internal class EmailServiceMessage: IEmailServiceMessage
    {
        private EmailServiceConfig Config { get; set; }
        private MimeMessage Message { get; set; }
        private bool Sent { get; set; }
        private bool Disposed { get; set; }
        public EmailServiceMessage(EmailServiceConfig config, MimeMessage message)
        {
            Config = config;
            Message = message;
        }
        private void OnBeforeSend()
        {
            if(!Message.From.Any() || !Message.To.Any() || string.IsNullOrWhiteSpace(Message.Subject) || Message.Body == null)
            {
                throw new Exception("Invalid Email Message");
            }
        }
        public void Send() {
            if (!Sent && !Disposed)
            {
                OnBeforeSend();
                try
                {
                    Message.SendMessage(Config);
                    Sent = true;
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to Send Message", e);
                }
            }                     
        } 
        public void AddSubject(string subject) => Message.AddSubject(subject);
        public void AddBodyMessage(string body) => Message.AddBodyMessage(body);
        public void AddBody(BodyBuilder body) => Message.AddBody(body);
        public void AddRecipient(string name, string emailAddress) => Message.AddRecipient(name, emailAddress);
        public void AddCc(string name, string emailAddress) => Message.AddCc(name, emailAddress);
        public void AddBcc(string name, string emailAddress) => Message.AddBcc(name, emailAddress);

        public void Dispose()
        {
            Config = null;
            Message = null;
            Disposed = true;
        }
    }
}
