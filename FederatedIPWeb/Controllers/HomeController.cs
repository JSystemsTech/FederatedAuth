using SharedServices.Web.Attributes;
using System.Web.Mvc;
using SharedServices.Web.Extensions;
using FederatedIPWeb.Models;
using SharedServices.Web.helpers;
using System;
using SharedServices.FederatedAuth;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using SharedServices.Extensions;
using System.Configuration;

namespace FederatedIPWeb.Controllers
{
    public class HomeController : BaseController
    {
        private string GetReturnToFederatedUrl(string consumingApplicationReturnUrl)
        {
            //Application.ConfigurationService.FederatedIPAuthentication.RedirectUri
            UriBuilder uriBuilder = new UriBuilder(Application.ConfigurationService.FederatedIPAuthentication.RedirectUri);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.SetValue("ConsumingApplicationReturnUrl", consumingApplicationReturnUrl);
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();            
        }
        private string GetSendToExternalAuthUrl(string returnToFederatedUrl)
        {
            UriBuilder uriBuilder = new UriBuilder(Application.ConfigurationService.FederatedIPAuthentication.ProviderUri);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.SetValue("ReturnUrl", returnToFederatedUrl);
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
        private string GetSendToFlexAuthUrl(string returnToFederatedUrl, string ConsumingApplicationReturnUrl)
        {
            UriBuilder uriBuilder = new UriBuilder(ConfigurationManager.AppSettings.GetValue("FlexProviderUrl"));
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.SetValue("ReturnUrl", returnToFederatedUrl);
            query.SetValue("ConsumingApplicationReturnUrl", ConsumingApplicationReturnUrl);
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
        public ActionResult Index(string ConsumingApplicationReturnUrl)
        {
            string encodedConsumingApplicationReturnUrl = HttpUtility.UrlEncode(ConsumingApplicationReturnUrl);
            string returnToFederatedUrl = GetReturnToFederatedUrl(encodedConsumingApplicationReturnUrl);
            ViewBag.ConsumingApplicationReturnUrl = ConsumingApplicationReturnUrl;
            ViewBag.FlexUrl = GetSendToFlexAuthUrl(returnToFederatedUrl, ConsumingApplicationReturnUrl);
            ViewBag.ExternalUrl = GetSendToExternalAuthUrl(returnToFederatedUrl);
            return View();
        }
        public ActionResult RedirectFromConsumingApp()
        {
            /*
             * Get WsFederation params from inbound url sent from consuming application. 
             * See GetLogoutView in HttpRequestHelper.cs for common code that sets these values
             */
            string ConsumingApplicationReturnUrl = HttpUtility.UrlEncode(GetWsFederationRequest(ControllerContext).GetResponseUrl());
            return RedirectToAction("Index", new { ConsumingApplicationReturnUrl });
        }
        public ActionResult FlexAuth(string ReturnUrl, string ConsumingApplicationReturnUrl)
        {
            ViewBag.ConsumingApplicationReturnUrl = ConsumingApplicationReturnUrl;
            return View(new FlexLogin(FlexUser.FlexUsers, HttpUtility.UrlEncode(ReturnUrl)));
        }
        public ActionResult FlexAuthSignIn(Guid UserId, string ReturnUrl)
        {
            UriBuilder uriBuilder = new UriBuilder(new Uri(HttpUtility.UrlDecode(ReturnUrl)));
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.SetValue("Token", UserId);
            uriBuilder.Query = query.ToString();
            string returnUrl = HttpUtility.UrlDecode(uriBuilder.ToString());

            return Redirect(returnUrl);
        }
        public ActionResult AuthenticatonReturnEndpoint(Guid Token, string ConsumingApplicationReturnUrl)
        {
            //WsFederation consumingApplicationWsFederationRequest = new WsFederation(new Uri(ConsumingApplicationReturnUrl));//GetWsFederationRequest(ControllerContext);
            FlexUser user = FlexUser.FlexUsers.FirstOrDefault(u => u.UserId == Token);
            string decodedReturnUrl = HttpUtility.UrlDecode(ConsumingApplicationReturnUrl);
            return AuthenticateUser(new Uri(decodedReturnUrl), user.Email, user.Name, user.Roles, user.Groups, user.Sites);
        }
    }
}