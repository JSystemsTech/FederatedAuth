using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedServices.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthenticatedAttribute:Attribute
    {
        private bool authenticated;
        public virtual bool Authenticated => authenticated;
        public AuthenticatedAttribute(bool authenticated = true)
        {
            this.authenticated = authenticated;
        }
    }
}