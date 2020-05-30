using SharedServices.Web.Application;
using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApplication2
{
    public class MvcApplication : HttpApplicationBase
    {
        protected override Assembly GetApplicationAssembly()
        =>Assembly.GetExecutingAssembly();
        protected override string GetApplicationDir() => Server.MapPath("~");
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

       
    }
}
