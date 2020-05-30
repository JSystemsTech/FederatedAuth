using SharedServices.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using System.Web;

namespace SharedServices
{
    public interface IIntegratedSharedService
    {
        HttpContext Context { get; }
        IConfigurationService ConfigurationService { get; }
        void UpdateUser(IPrincipal user);
        Assembly ApplicationAssembly { get;}
        string ProjectName { get;}
        string ApplicationDir { get; }
    }
}
