using System;

namespace SharedServices.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal class LogoutEndointAttribute : Attribute
    {
       public LogoutEndointAttribute() { }
    }
}