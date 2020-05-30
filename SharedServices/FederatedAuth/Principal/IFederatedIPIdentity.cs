using System.Collections.Generic;
using System.Security.Principal;

namespace SharedServices.FederatedAuth.Principal
{
    public interface IFederatedIPIdentity: IIdentity
    {
        FederatedIPUserType UserType { get; }
        IEnumerable<string> Roles { get; }
        IEnumerable<string> Groups { get;}
        IEnumerable<string> Sites { get; }
        string Email { get;}
        bool IsSystemAdmin();
        bool IsInRole(string role);
        bool IsInGroup(string group);
        bool IsInSite(string site);
        
    }
}
