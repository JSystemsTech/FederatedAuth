using Microsoft.IdentityModel.Tokens.Saml2;
using SharedServices.Configuration;
using SharedServices.Extensions;
using SharedServices.FederatedAuth;
using SharedServices.FederatedAuth.Principal;
using SharedServices.FederatedAuth.SecurityToken;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace SharedServices.Web.AuthNAuthZ
{
    public interface IHttpRequestHelper
    {
        IIntegratedSharedService Application { get; }
        IFederatedIPUser User { get; }
        bool IsAuthenticatedUser { get; }
        string AuthenticateUser(
            Uri consumingAppUri,
            string email,
            string name,
            IEnumerable<string> roles = null,
            IEnumerable<string> groups = null,
            IEnumerable<string> sites = null);
        void HandleAuthenticatedRequest(Func<IFederatedIPUser, IFederatedIPUser> Updater);
        void HandlePublicPageRequest();
        void HandleForbiddenRequest();
        string GetLogoutView(bool useDefaultRedirect = false);
    }
    public class HttpRequestHelper<TFederatedIPUser> : IHttpRequestHelper
        where TFederatedIPUser: FederatedIPUser
    {

        private HttpContextBase CurrentContext { get; set; }
        public IIntegratedSharedService Application { get; private set; }
        //public IPrincipal User { get => Application.Context.User; }
        //public IIdentity Identity { get => User.Identity; }



        private IConfigurationService ConfigurationService { get; set; }

        public IFederatedIPUser User { get; private set; }

        private FederatedIPSecurityToken<TFederatedIPUser> SecurityToken { get; set; }
        private FederatedIPAuthentication FederatedIPAuthentication { get => ConfigurationService.FederatedIPAuthentication; }
        public HttpRequestHelper(HttpContextBase currentContext)
        {
            CurrentContext = currentContext;            
            if (CurrentContext.ApplicationInstance is IIntegratedSharedService application)
            {
                Application = application;
                ConfigurationService = Application.ConfigurationService;
                SecurityToken = FederatedIPAuthentication.GetCredentials<TFederatedIPUser>(Application);
            }
            
            //else if (Request.HttpMethod == "POST" && Request.Form.Get("__WebAuthToken") is string token )
            //{
            //    Saml2SecurityToken saml2SecurityToken = FederatedIPAuthentication.Decrypt(token);
            //    SecurityToken = FederatedIPAuthentication.Deserialize<AppUserSecurityToken, IAppUser>(saml2SecurityToken);
            //    HasCookieFormToken = true;
            //}
            User = SecurityToken != null ? SecurityToken.User : FederatedIPUser.Public;
        }
        private string SignIn()
        {            
            SecurityToken.Update();
            return FederatedIPAuthentication.SignIn(Application, SecurityToken);            
        }

        
        public bool IsAuthenticatedUser { get => User.IsAuthenticated; }
        public bool ShouldRefreshData { get => SecurityToken.ShouldRefreshData(); }

        private void OnLogout()
        {
            if(IsAuthenticatedUser)
            {
                FederatedIPAuthentication.SignOut(Application, FederatedIPUser.Public);
            }            
            
        }
        public string AuthenticateUser(
            Uri consumingAppUri,
            string email,
            string name,
            IEnumerable<string> roles = null,
            IEnumerable<string> groups = null,
            IEnumerable<string> sites = null)
        {
            User = (TFederatedIPUser)Activator.CreateInstance(typeof(TFederatedIPUser), FederatedIPUserType.Authenticated, email, name, roles, groups, sites);
            SecurityToken = new FederatedIPSecurityToken<TFederatedIPUser>((TFederatedIPUser)User, FederatedIPAuthentication, Application);
            
            SignIn();
            string content = !string.IsNullOrWhiteSpace(FederatedIPAuthentication.LoginContent) ? FederatedIPAuthentication.LoginContent : "<h1>Signing In...</h1>";
            return GetPageRedirectTemplate(consumingAppUri.ToString(), content);
        }
        public void HandleAuthenticatedRequest(Func<IFederatedIPUser, IFederatedIPUser> Updater) {
            if (SecurityToken.ShouldRefreshData())
            {
                User = Updater(User);
                SecurityToken.UpdatePrincipal((TFederatedIPUser)User);
            }
            SignIn(); 
        }
        public void HandlePublicPageRequest()=> OnLogout();
        public void HandleForbiddenRequest()
        {

            OnLogout();
            if (Application.Context.Request.IsAjaxRequest())
            {
                Application.Context.Response.ClearContent();
                Application.Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }

        }
        public string GetLogoutView(bool useDefaultRedirect = false) {
            Uri LogoutUri = FederatedIPAuthentication.ProviderUri;
            Uri returnUri = useDefaultRedirect ? FederatedIPAuthentication.RedirectUri : Application.Context.Request.Url;
            string content = !string.IsNullOrWhiteSpace(FederatedIPAuthentication.LogoutContent) ? FederatedIPAuthentication.LogoutContent: "<h1>Logging Off...</h1>";
            /*Set WsFederation params to send from consuming application to provider */
            return GetPageRedirectTemplate(FederatedIPAuthentication.WsFederation.BuildWsFederationUrl(LogoutUri, returnUri), content);
        }
        private string PageRedirectHeader()
        {
            string path = HttpRuntime.AppDomainAppVirtualPath == "/" ? "" : HttpRuntime.AppDomainAppVirtualPath;
            return "<head>"
            + "<meta charset='utf-8'>"
            + "<meta name='viewport' content='width=device-width, initial-scale=1.0'>"
            + $"<link href='{path}/Content/site.css' rel='stylesheet'>"
            + "</head>";
        }
        public string GetPageRedirectTemplate(string redirectUrl, string content =null)
        => $"<html>{PageRedirectHeader()}<body><a href='{redirectUrl}' id='PageRedirectLink'></a>{content}<script type='text/javascript'>document.getElementById('PageRedirectLink').click();</script></body></html>";
        



    }
}
