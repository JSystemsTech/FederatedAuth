using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using SharedServices.Extensions;
using SharedServices.FederatedAuth.Principal;
using System;
using System.Collections.Generic;

namespace SharedServices.FederatedAuth.SecurityToken
{
    public class FederatedIPSecurityToken<TPrincipal> : Microsoft.IdentityModel.Tokens.SecurityToken
        where TPrincipal : FederatedIPUser
    {
        public TPrincipal User { get; private set; }
        private DateTime IssueDate { get; set; }
        private DateTime ExpirationDate { get; set; }
        private string IssuerName { get; set; }
        
        public override string Id { get; }

        public override string Issuer => IssuerName;

        public override SecurityKey SecurityKey { get; }

        public override SecurityKey SigningKey { get; set; }

        public override DateTime ValidFrom => IssueDate;

        public override DateTime ValidTo => ExpirationDate;

        public DateTime RefreshDate { get; internal set; }
        public bool ShouldRefreshData() => DateTime.UtcNow > RefreshDate;
        private void SetRefreshDate()
        {
            RefreshDate = DateTime.UtcNow.AddMinutes(FederatedIPAuthentication.DataTimeout);
        }

        public bool IsValidTokenDate()
        {
            DateTime now = DateTime.UtcNow;
            return now >= ValidFrom && now < ValidTo;
        }
        internal FederatedIPAuthentication FederatedIPAuthentication { get; set; }
        public FederatedIPSecurityToken() : base() { }
        public FederatedIPSecurityToken(TPrincipal principal, FederatedIPAuthentication federatedIPAuthentication, IIntegratedSharedService application) : base()
        {
            FederatedIPAuthentication = federatedIPAuthentication;
            User = principal;
            Update();
        }
        public void UpdatePrincipal(TPrincipal principal)
        {
            User = principal;;
        }
        public FederatedIPSecurityToken(
            DateTime issueDate, 
            DateTime expirationDate, 
            IEnumerable<Saml2Attribute> pricipalAttributes, 
            IEnumerable<Saml2Attribute> tokenAttributes, 
            FederatedIPAuthentication federatedIPAuthentication, 
            IIntegratedSharedService application) : base()
        {
            FederatedIPAuthentication = federatedIPAuthentication;
            User = (TPrincipal)Activator.CreateInstance(typeof(TPrincipal), pricipalAttributes);
            RefreshDate = tokenAttributes.GetValue<DateTime>("RefreshDate");
            SetData(issueDate, expirationDate, FederatedIPAuthentication.Issuer);
        }
        
        public void Update()
        {
            DateTime issueDate = DateTime.UtcNow;
            DateTime expirationDate = DateTime.UtcNow.AddMinutes(FederatedIPAuthentication.Timeout);
            SetRefreshDate();
            SetData(issueDate, expirationDate, FederatedIPAuthentication.Issuer);
        }
        internal void SetData(DateTime issueDate, DateTime expirationDate, string issuerName)
        {
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
            IssuerName = issuerName;
        }
    }
}
