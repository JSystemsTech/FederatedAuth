using SharedServices.FederatedAuth.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedServices.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeGroupAttribute : Attribute
    {
        private IEnumerable<IEnumerable<string>> GroupSets { get; set; }
        //public virtual IEnumerable<string> Groups => groups;
        public AuthorizeGroupAttribute(params string[] groups)
        {
            this.GroupSets = groups.Select(g=> g.Split(','));
        }
        private bool IsInGroupSet(IFederatedIPUser user, IEnumerable<string> set) => set.Any(group => user.IsInGroup(group));
        public bool IsAuthorized(IFederatedIPUser user) => GroupSets.All(set => IsInGroupSet(user, set));
    }
} 