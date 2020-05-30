using Microsoft.IdentityModel.Tokens.Saml2;
using SharedServices.Encryption;
using SharedServices.Extensions;
using SharedServices.FederatedAuth.Principal;
using SharedServices.FederatedAuth.SecurityToken;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Xml;

namespace SharedServices.FederatedAuth
{
    public enum FederatedIPAuthenticationMode
    {
        Provider,
        Consumer
    }
    public class FederatedIPAuthentication
    {
        private static string _DefaultEncryptionKey = "DefaultFederatedIPEncryptionKey";
        private static string _DefaultConfirmationMethod = "https://www.federated.ip.com";
        private static string _DefaultSubjectName = "name";
        private static int _DefaultTimeout = 15;
        private static int _DefaultDataTimeout = 15;
        private static Saml2SecurityTokenHandler SamlTokenHandler = new Saml2SecurityTokenHandler();
        internal FederatedIPAuthenticationMode Mode { get; private set; }
        public Uri RedirectUri { get; private set; }
        public Uri ProviderUri { get; private set; }
        internal Uri AudienceUri { get; private set; }
        internal string ApplicationId { get; set; }
        private string DomainId { get; set; }
        private string ProviderId { get; set; }
        public string CookieName { get; private set; }
        internal string Issuer { get; private set; }
        internal string ConfirmationMethod { get; private set; }
        internal string SubjectName { get; set; }
        internal bool AuthenticationRequired { get; private set; }
        public WsFederation WsFederation { get; set; }

        private string EncryptionKey { get; set; }
        internal int Timeout { get; private set; }
        internal int DataTimeout { get; private set; }
        internal string LoginContent { get; private set; }
        internal string LogoutContent { get; private set; }
        private NameValueCollection Settings{ get; set; }
        private static IEnumerable<string> RequiredProviderSettings = new string[4] { "DomainId", "ProviderId", "AudienceUri", "RedirectUri" };
        private static IEnumerable<string> RequiredConsumerSettings = new string[6] { "ApplicationId", "DomainId", "ProviderId", "AudienceUri", "RedirectUri", "ProviderUri" };
        private bool HasRequiredSetting(string propertyName)
        {
            if (!Settings.ContainsKey(propertyName))
            {
                throw new Exception($"Federated Authentication requires {propertyName}");
            }
            return true;
        }

        private bool HasRequiredSettings() {
            if (HasRequiredSetting("Mode"))
            {
                Mode = Settings.GetValue("Mode").ToLower() == "provider" ? FederatedIPAuthenticationMode.Provider : FederatedIPAuthenticationMode.Consumer;
                return Mode == FederatedIPAuthenticationMode.Provider ? RequiredProviderSettings.All(s => HasRequiredSetting(s)): RequiredConsumerSettings.All(s => HasRequiredSetting(s));
            }
            return false;
        }
        public FederatedIPAuthentication(NameValueCollection settings)
        {
            Settings = settings;
            if (HasRequiredSettings())
            {
                AudienceUri = Settings.GetValueUri("AudienceUri");
                if(AudienceUri == null)
                {
                    throw new Exception("Federated Authentication AudienceUri must be a valid URL");
                }
                RedirectUri = Settings.GetValueUri("RedirectUri");
                if (RedirectUri == null && Mode == FederatedIPAuthenticationMode.Consumer)
                {
                    throw new Exception("Federated Authentication RedirectUri must be a valid URL");
                }
                ProviderUri = Settings.GetValueUri("ProviderUri");
                if (ProviderUri == null && Mode == FederatedIPAuthenticationMode.Consumer)
                {
                    throw new Exception("Federated Authentication ProviderUri must be a valid URL");
                }
                ApplicationId = Settings.GetValue("ApplicationId");
                DomainId = Settings.GetValue("DomainId");
                ProviderId = Settings.GetValue("ProviderId");
                CookieName = $"{DomainId}{ProviderId}Token";
                EncryptionKey = Settings.GetValue("EncryptionKey", _DefaultEncryptionKey);
                Timeout = Settings.GetValueInt("Timeout", _DefaultTimeout);
                DataTimeout = Settings.GetValueInt("DataTimeout", _DefaultDataTimeout);

                AuthenticationRequired = Mode == FederatedIPAuthenticationMode.Consumer ? Settings.GetValueBool("AuthenticationRequired", false): false;

                Issuer = ProviderId;
                ConfirmationMethod = Settings.GetValue("ConfirmationMethod", _DefaultConfirmationMethod);
                SubjectName = Settings.GetValue("SubjectName",_DefaultSubjectName);

                LoginContent = Settings.GetValueHtml("LoginContent");
                LogoutContent = Settings.GetValueHtml("LogoutContent");

                WsFederation = new WsFederation
                {
                    //WtRealm = settings.GetValue("WtRealm"),
                    wtrealm = AudienceUri.ToString(),
                    wfresh = Settings.GetValueInt("wfresh"),
                    wauth = Settings.GetValue("wauth"),
                    wreq = Settings.GetValue("wreq"),
                    wreply = Settings.GetValue("wreply"),
                    wctx = Settings.GetValue("wctx"),
                    whr = Settings.GetValue("whr")
                };
            }            
        }

        private string GetTokenString(Saml2SecurityToken token) => token.Serialize().Encrypt(EncryptionKey);
        private Saml2SecurityToken ParseTokenString(string token)
        {            
            string tokenStr = token.Decrypt(EncryptionKey);
            if (string.IsNullOrWhiteSpace(tokenStr))
            {
                throw new Exception("Unable to parse token");
            }
            return tokenStr.Deserialize();
        }

        public WsFederation GetWsFederationRequest(HttpContextBase context) => new WsFederation(context.Request.Url);
        public FederatedIPSecurityToken<TPrincipal> GetCredentials<TPrincipal>(IIntegratedSharedService application)
            where TPrincipal : FederatedIPUser
        {
            if (application.Context.HasCookie(CookieName))
            {
                try
                {
                    HttpCookie cookie= application.Context.GetCookie(CookieName);
                    Saml2SecurityToken saml2SecurityToken = ParseTokenString(cookie.Value);                    
                    FederatedIPSecurityToken<TPrincipal> securityToken = saml2SecurityToken.ToSecurityToken<TPrincipal>(this, application);
                    if (securityToken.IsValidTokenDate())
                    {
                        return securityToken;
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
        public string SignIn<TPrincipal>(IIntegratedSharedService application, FederatedIPSecurityToken<TPrincipal> secutityToken)
            where TPrincipal : FederatedIPUser
        {            
            string data = GetTokenString(secutityToken.ToSaml2SecurityToken(application));
            HttpCookie updateCookie = application.Context.CreateCookie(CookieName, data, secutityToken.ValidTo);
            if (application.Context.HasCookie(CookieName))
            {
                application.Context.SetCookie(updateCookie);
            }
            else
            {
                application.Context.AddCookie(updateCookie);
            }
            application.UpdateUser(secutityToken.User);
            return data;
        }
        public void SignOut(IIntegratedSharedService application, IPrincipal user)
        {
            application.UpdateUser(user);
            application.Context.RemoveCookie(CookieName);
        }
    }
}
