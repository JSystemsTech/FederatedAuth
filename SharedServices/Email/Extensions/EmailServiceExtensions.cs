using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Email.Extensions
{
    internal static class EmailServiceExtensions
    {
        public static MimeMessage BeginMessage() => new MimeMessage();
        public static MimeMessage BeginMessage(MailboxAddress sender) {
            var message = BeginMessage();
            message.AddSender(sender);
            return message;
        }
        public static MimeMessage BeginMessage(MailboxAddress sender, MailboxAddress recipient)
        {
            var message = BeginMessage(sender);
            message.AddRecipient(recipient);
            return message;
        }
        public static MimeMessage BeginMessage(MailboxAddress sender, string name, string emailAddress)
        {
            var message = BeginMessage(sender);
            message.AddRecipient(name, emailAddress);
            return message;
        }
        private static void AddIfDoesNotExist(this InternetAddressList list, MailboxAddress address)
        {
            if(!list.Where(m=> m is MailboxAddress).Select(m=> m as MailboxAddress).Any(m=> m.Address == address.Address))
            {
                list.Add(address);
            }
        }
        public static void AddSender(this MimeMessage message, MailboxAddress sender) => message.From.AddIfDoesNotExist(sender);
        public static void AddRecipient(this MimeMessage message, MailboxAddress sender) => message.To.AddIfDoesNotExist(sender);
        public static void AddRecipient(this MimeMessage message, string name, string emailAddress) => message.AddRecipient(new MailboxAddress(name, emailAddress));

        public static void AddCc(this MimeMessage message, MailboxAddress sender) => message.Cc.AddIfDoesNotExist(sender);
        public static void AddCc(this MimeMessage message, string name, string emailAddress) => message.AddCc(new MailboxAddress(name, emailAddress));

        public static void AddBcc(this MimeMessage message, MailboxAddress sender) => message.Bcc.AddIfDoesNotExist(sender);
        public static void AddBcc(this MimeMessage message, string name, string emailAddress) => message.AddBcc(new MailboxAddress(name, emailAddress));

        public static void AddSubject(this MimeMessage message, string subject) => message.Subject = subject;
        public static void AddBodyMessage(this MimeMessage message, string bodyMessage)
        {
            message.Body = new TextPart("plain"){Text = bodyMessage };
        }
        public static void AddBody(this MimeMessage message, BodyBuilder builder)
        {
            message.Body = builder.ToMessageBody();
        }

        public static void SendMessage(this MimeMessage message, EmailServiceConfig config) {
            using (var client = new SmtpClient())
            {
                client.Connect(config.Host, config.Port, config.UseSSL);

                // Note: only needed if the SMTP server requires authentication
                if (config.UseAuthentication)
                {
                    client.Authenticate(config.UserName, config.Password);
                }

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
