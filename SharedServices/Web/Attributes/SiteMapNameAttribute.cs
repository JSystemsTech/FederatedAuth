using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedServices.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SiteMapNameAttribute : Attribute
    {
        private string name;
        public virtual string Name => name;
        public SiteMapNameAttribute(string name)
        {
            this.name = name;
        }
    }
}