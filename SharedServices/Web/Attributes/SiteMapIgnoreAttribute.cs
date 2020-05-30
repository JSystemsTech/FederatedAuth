using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedServices.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SiteMapIgnoreAttribute : Attribute
    {
        public SiteMapIgnoreAttribute() { }
    }
}