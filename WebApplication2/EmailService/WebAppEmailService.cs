using SharedServices.Email;
using SharedServices.FederatedAuth.Principal;

namespace WebApplication2.EmailService
{
    public class WebAppEmailService
    {
        private IEmailService Service { get; set; }

        public WebAppEmailService(IEmailService service)
        {
            Service = service;
        }
        private IEmailServiceMessage BeginMessage(IFederatedIPUser user) => Service.BeginSystemMessage(user.Name, user.Email);
        public void SendWelcomeMessage(IFederatedIPUser user)
        {
            using (IEmailServiceMessage message = BeginMessage(user))
            {
                message.AddSubject("Welcome To My Consuming App");
                message.AddBodyMessage($"{user.Name},\nOn behalf of all the staff at My Consuming App we would like to welcome you to the site!");
                message.Send();
            }
        }
        public void SendProfileChangeNotificationMessage(IFederatedIPUser user)
        {
            using (IEmailServiceMessage message = BeginMessage(user))
            {
                message.AddSubject("Profile Updated");
                message.AddBodyMessage($"{user.Name},\nYour Profile was recently updated.");
                message.Send();
            }
        }
        public void SendContactUsEmail(string email, string name, string message)
        {
            using (IEmailServiceMessage emailMessage = Service.BeginMessage(name, email, "My Consuming App Team", "my.consuming.app.team@myemail.com"))
            {
                emailMessage.AddSubject($"Customer Inquirey from {name}");
                emailMessage.AddBodyMessage(message);
                emailMessage.Send();
            }
        }
    }
}