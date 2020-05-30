using SharedServices.Extensions;
using System.Collections.Specialized;

namespace SharedServices.Configuration.Models
{
    public class WebPageConfiguration
    {
        public string Title { get; private set; }
        public bool EnableSideMenu { get; private set; }
        public bool EnableWarningModal { get; private set; }
        public int Timeout { get; private set; }
        public int WarningModalDuration { get; private set; }
        public double ServerRefreshThreshold { get; private set; }


        public WebPageConfiguration(NameValueCollection settings)
        {
            Title = settings.GetValue("Title", "My App");
            EnableSideMenu = settings.GetValueBool("EnableSideMenu", true);
            EnableWarningModal = settings.GetValueBool("EnableWarningModal", true);
            Timeout = settings.GetValueInt("EnableWarningModal", 15);
            WarningModalDuration = settings.GetValueInt("WarningModalDuration", 2);
            ServerRefreshThreshold = settings.GetValueDouble("ServerRefreshThreshold", 0.5);
        }
        public WebPageConfiguration()
        {
            EnableSideMenu = true;
            EnableWarningModal = true;
            Timeout = 15;
            WarningModalDuration = 2;
            ServerRefreshThreshold = 0.5;
        }



    }
}
