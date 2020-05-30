using SharedServices.Configuration;
using System.Reflection;
using System.Security.Principal;
using System.Web;

namespace SharedServices.Web.Application
{
    public abstract class HttpApplicationBase : HttpApplication, IIntegratedSharedService
    {
        public IConfigurationService ConfigurationService { get; private set; }
        public void UpdateUser(IPrincipal user) => HttpContext.Current.User = user;

        public Assembly ApplicationAssembly { get; private set; }
        public string ApplicationDir { get; private set; }
        public string ProjectName { get => ApplicationAssembly.FullName.Split(',')[0]; }

        protected abstract Assembly GetApplicationAssembly();
        protected abstract string GetApplicationDir();
        public override void Init()
        {
            base.Init();
            ApplicationAssembly = GetApplicationAssembly();
            ApplicationDir = GetApplicationDir();
            ConfigurationService = SharedServiceFactory.GetConfigurationService();
        }

    }
}
