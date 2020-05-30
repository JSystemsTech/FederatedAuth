using Microsoft.IdentityModel.Tokens.Saml2;
using SharedServices.Extensions;
using System.Collections.Generic;
using System.Security.Principal;

namespace SharedServices.FederatedAuth.Principal
{
    public class FederatedIPUser : IFederatedIPUser
    {
        internal static FederatedIPUser Public => new FederatedIPUser();
        private IFederatedIPIdentity FederatedIPIdentity { get; set; }
        public IIdentity Identity => FederatedIPIdentity;
        public FederatedIPUserType UserType => FederatedIPIdentity.UserType;
        public string Name => FederatedIPIdentity.Name;
        public string Email => FederatedIPIdentity.Email;
        public IEnumerable<string> Roles => FederatedIPIdentity.Roles;
        public IEnumerable<string> Groups => FederatedIPIdentity.Groups;
        public IEnumerable<string> Sites => FederatedIPIdentity.Sites;
        public bool IsAuthenticated => FederatedIPIdentity.IsAuthenticated;

        public bool IsInRole(string role) => FederatedIPIdentity.IsInRole(role);
        public bool IsInGroup(string group) => FederatedIPIdentity.IsInGroup(group);
        public bool IsInSite(string site) => FederatedIPIdentity.IsInSite(site);

        public FederatedIPUser(
            FederatedIPUserType userType = FederatedIPUserType.Public, 
            string email = null, 
            string name = null, 
            IEnumerable<string> roles = null, 
            IEnumerable<string> groups = null, 
            IEnumerable<string> sites = null)
        {
            FederatedIPIdentity = new FederatedIPIdentity(userType, email, name, roles, groups, sites);            
        }
        public FederatedIPUser(IEnumerable<Saml2Attribute> attributes)
        {
            FederatedIPIdentity = new FederatedIPIdentity(
                attributes.GetValue<FederatedIPUserType>("UserType"),
                attributes.GetValue<string>("Email"),
                attributes.GetValue<string>("Name"),
                attributes.GetValue<IEnumerable<string>>("Roles"),
                attributes.GetValue<IEnumerable<string>>("Groups"),
                attributes.GetValue<IEnumerable<string>>("Sites")
                );
        }
    }
}
