using SharedServices.Web.Attributes;
using System.Web.Mvc;
using SharedServices.Web.Extensions;
using ExternalAuthenticationWeb.Models;
using SharedServices.Web.helpers;
using System;
using System.Collections.Specialized;
using System.Web;
using System.Linq;

namespace ExternalAuthenticationWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string ReturnUrl)
        {
            return View(new UserAuth() { ReturnUrl = HttpUtility.UrlEncode(ReturnUrl) } );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserAuth vm)
        {
            if (ModelState.IsValid)
            {
                AuthUser user = AuthUser.Users.FirstOrDefault(u=> u.Email == vm.Email);
                if(user != null)
                {
                    UriBuilder uriBuilder = new UriBuilder(new Uri(HttpUtility.UrlDecode(vm.ReturnUrl)));
                    NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
                    query["Token"] = user.UserId.ToString();
                    uriBuilder.Query = query.ToString();
                    string returnUrl = uriBuilder.ToString();

                    return Redirect(returnUrl);
                }                
            }
            return RedirectToAction("Index", vm);
        }
    }
}