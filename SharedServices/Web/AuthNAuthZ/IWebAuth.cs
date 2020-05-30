using SharedServices.FederatedAuth.Principal;
using SharedServices.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedServices.Web.AuthNAuthZ
{
    public interface IWebAuthResponse
    {
        bool AllowRequest { get; }
    }
    internal sealed class WebAuthResponse : IWebAuthResponse
    {
        public bool AllowRequest { get; private set; }
        internal static IWebAuthResponse Ok() => new WebAuthResponse() { AllowRequest = true };
        internal static IWebAuthResponse Forbidden() => new WebAuthResponse() { AllowRequest = false };
        internal static IWebAuthResponse Unauthorized() => new WebAuthResponse() { AllowRequest = false };
    }
    public interface IWebAuthService
    {
        string AuthenticateUser(
            HttpContextBase httpContext,
            Uri consumingAppUri, 
            string email,
            string name,
            IEnumerable<string> roles = null,
            IEnumerable<string> groups = null,
            IEnumerable<string> sites = null);
        IWebAuthResponse OnAuthentication(HttpContextBase httpContext, bool authenticationRequired);
        IWebAuthResponse OnAuthorization(HttpContextBase httpContext, IEnumerable<AuthorizeGroupAttribute> groupAuthorizations);
        string Logout(HttpContextBase httpContext, bool useDefaultRedirect = false);
    }
    internal class WebAuthServiceBase<TFederatedIPUser> : IWebAuthService 
        where TFederatedIPUser: FederatedIPUser
    {
        protected IHttpRequestHelper GetRequestHelper(HttpContextBase httpContext) => new HttpRequestHelper<TFederatedIPUser>(httpContext);
        public virtual string AuthenticateUser(
            HttpContextBase httpContext,
            Uri consumingAppUri,
            string email,
            string name,
            IEnumerable<string> roles = null,
            IEnumerable<string> groups = null,
            IEnumerable<string> sites = null) => null;
        public virtual string Logout(HttpContextBase httpContext, bool useDefaultRedirect = false) => null;
        public virtual IWebAuthResponse OnAuthentication(HttpContextBase httpContext, bool authenticationRequired) => WebAuthResponse.Ok();
        public virtual IWebAuthResponse OnAuthorization(HttpContextBase httpContext, IEnumerable<AuthorizeGroupAttribute> groupAuthorizations) => WebAuthResponse.Ok();
    }
    internal sealed class WebAuthService : WebAuthServiceBase<FederatedIPUser>
    {
        private FederatedIPUser UpdateUserData(IFederatedIPUser currentUser)
        {
            return (FederatedIPUser)currentUser;
        }
        public override string AuthenticateUser(
            HttpContextBase httpContext,
            Uri consumingAppUri,
            string email,
            string name,
            IEnumerable<string> roles = null,
            IEnumerable<string> groups = null,
            IEnumerable<string> sites = null) => GetRequestHelper(httpContext).AuthenticateUser(consumingAppUri, email, name, roles, groups, sites);
        public override IWebAuthResponse OnAuthentication(HttpContextBase httpContext, bool authenticationRequired)
        {
            IHttpRequestHelper requestHelper = GetRequestHelper(httpContext);
            if (authenticationRequired)
            {
                bool HasSiteAuthorization = requestHelper.Application.ConfigurationService.FederatedIPAuthentication.Mode == FederatedAuth.FederatedIPAuthenticationMode.Consumer
                && requestHelper.User.Sites.Any() ? requestHelper.User.Sites.Contains(requestHelper.Application.ConfigurationService.FederatedIPAuthentication.ApplicationId) : true;

                if (requestHelper.IsAuthenticatedUser && HasSiteAuthorization)
                {
                    requestHelper.HandleAuthenticatedRequest(UpdateUserData);
                    return WebAuthResponse.Ok();
                }
                requestHelper.HandleForbiddenRequest();
                return WebAuthResponse.Forbidden();
            }
            requestHelper.HandlePublicPageRequest();
            return WebAuthResponse.Ok();
        }
        public override IWebAuthResponse OnAuthorization(HttpContextBase httpContext, IEnumerable<AuthorizeGroupAttribute> groupAuthorizations)
        {
            IHttpRequestHelper requestHelper = GetRequestHelper(httpContext);
            bool HasGroupAuthorization = groupAuthorizations.All(auth => auth.IsAuthorized(requestHelper.User));
            bool HasSiteAuthorization = requestHelper.Application.ConfigurationService.FederatedIPAuthentication.Mode == FederatedAuth.FederatedIPAuthenticationMode.Consumer 
                && requestHelper.User.Sites.Any() ? requestHelper.User.Sites.Contains(requestHelper.Application.ConfigurationService.FederatedIPAuthentication.ApplicationId) : true;
            return HasGroupAuthorization && HasSiteAuthorization ? WebAuthResponse.Ok() : WebAuthResponse.Unauthorized();
        }
        public override string Logout(HttpContextBase httpContext, bool useDefaultRedirect = false) {
            IHttpRequestHelper requestHelper = GetRequestHelper(httpContext);
            requestHelper.HandlePublicPageRequest();
            return requestHelper.GetLogoutView(useDefaultRedirect);
        }
    }
}
