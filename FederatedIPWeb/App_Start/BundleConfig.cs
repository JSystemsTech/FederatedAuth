using System.Web;
using System.Web.Optimization;
using FederatedIPWeb.App_Start;

namespace FederatedIPWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("jquery").AddScript("jquery-{version}.js");
            bundles.AddScriptBundle("jqueryval").AddScript("jquery.validate*");

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.AddScriptBundle("modernizr").AddScript("modernizr-*");

            bundles.AddScriptBundle("bootstrap").AddScript("bootstrap.bundle.min.js");

            bundles.AddStyleBundle("css").AddStyle("site.css");

        }
    }
}
