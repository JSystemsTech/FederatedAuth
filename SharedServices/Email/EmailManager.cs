using MimeKit;
using SharedServices.Email.Extensions;

namespace SharedServices.Email
{
    public interface IEmailService
    {
		IEmailServiceMessage BeginSystemMessage(string senderName, string senderEmail);
		IEmailServiceMessage BeginMessage(string senderName, string senderEmail, string recipientName, string recipientEmail);
	}

	internal class EmailManager: IEmailService
	{
		private EmailServiceConfig Config { get; set; }
		private MailboxAddress Sender { get; set; }
		public EmailManager(EmailServiceConfig config)
        {
			Config = config;
			Sender = new MailboxAddress(Config.SenderName, Config.SenderEmail);
		}
		public IEmailServiceMessage BeginSystemMessage(string senderName, string senderEmail)
			=>  new EmailServiceMessage (Config, EmailServiceExtensions.BeginMessage(Sender, senderName, senderEmail));
		public IEmailServiceMessage BeginMessage(string senderName, string senderEmail, string recipientName, string recipientEmail)
			=> new EmailServiceMessage(Config, EmailServiceExtensions.BeginMessage(new MailboxAddress(senderName, senderEmail), new MailboxAddress(recipientName, recipientEmail)));



	}
}
