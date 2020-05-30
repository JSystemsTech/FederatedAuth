using SharedServices.Configuration;
using System.Web;

namespace SharedServices
{
    public class SharedServiceFactory
    {
        public static IConfigurationService GetConfigurationService() => new ConfigurationService();
    }
}
