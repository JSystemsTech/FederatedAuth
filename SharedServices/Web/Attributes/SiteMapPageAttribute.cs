using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedServices.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SiteMapPageAttribute : Attribute
    {
        private bool isPage;
        public virtual bool IsPage => isPage;
        public SiteMapPageAttribute(bool isPage = true)
        {
            this.isPage = isPage;
        }
    }
}