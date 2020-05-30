using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.FederatedAuth.Principal
{
    public interface IFederatedIPUser:IPrincipal
    {
        FederatedIPUserType UserType { get; }
        string Name { get; }
        string Email { get; }
        IEnumerable<string> Roles { get; }
        IEnumerable<string> Groups { get; }
        IEnumerable<string> Sites { get; }
        bool IsAuthenticated { get; }
        bool IsInGroup(string group);
        bool IsInSite(string site);
    }
}
