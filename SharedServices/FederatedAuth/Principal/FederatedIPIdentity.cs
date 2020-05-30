using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.FederatedAuth.Principal
{
    internal class FederatedIPIdentity : IFederatedIPIdentity
    {
        public FederatedIPUserType UserType { get; private set; }
        public IEnumerable<string> Roles { get; private set; }
        public IEnumerable<string> Groups { get; private set; }
        public IEnumerable<string> Sites { get; private set; }        
        public string Name { get; private set; }
        public string Email { get; private set; }
        public bool IsAuthenticated => UserType == FederatedIPUserType.Authenticated || IsSystemAdmin();
        public string AuthenticationType { get => UserType.ToString(); }

        public virtual bool IsSystemAdmin() => UserType == FederatedIPUserType.SystemAdmin;
        public bool IsInRole(string role) => Roles.Contains(role) || IsSystemAdmin();
        public bool IsInGroup(string group) => Groups.Contains(group) || IsSystemAdmin();
        public bool IsInSite(string site) => Sites.Contains(site) || IsSystemAdmin();
        
        private IEnumerable<string> ResolveEnumerable(IEnumerable<string> list = null) => list == null ? new string[0] { } : list;
        private string ResolveEmail(string email)
            => !string.IsNullOrWhiteSpace(email)
            && UserType != FederatedIPUserType.Public ? email : "federatedip.public.user@federatedip.com";
        private string ResolveName(string name)
            => !string.IsNullOrWhiteSpace(name)
            && UserType != FederatedIPUserType.Public ? name : "Public User";

        public FederatedIPIdentity(FederatedIPUserType userType = FederatedIPUserType.Public, string email=null, string name=null, IEnumerable<string> roles = null, IEnumerable<string> groups = null, IEnumerable<string> sites = null)
        {
            UserType = userType;
            Email = ResolveEmail(email);
            Name = ResolveName(name);
            Roles = ResolveEnumerable(roles);
            Groups = ResolveEnumerable(groups);
            Sites = ResolveEnumerable(sites);
        }
    }
}
